using Crypto.Asym;
using Crypto.Hash;
using Crypto.Model;
using Crypto.Sym;
using Org.BouncyCastle.Crypto.Parameters;

namespace Crypto.Utils
{
    public class Utils
    {
        public static EncryptedDataContainer EncryptDataForPublicKey(ECPublicKeyParameters recieversPublicKey, byte[] data)
        {
            var rnd = Random.GetSecureRandom();

            var randomKey = rnd.GenerateSeed(32); //256 random bits
            var randomIv = rnd.GenerateSeed(16); //128 random bits
            var encryptedData = AES.Process(data, randomKey, randomIv, true);

            var tempPair = KeyGen.GenerateKeyPair();
            var commonSecret = ECDH.CalculateCommonSecret(tempPair.Private, recieversPublicKey);

            var randomIvForKey = rnd.GenerateSeed(16); //128 random bits for key encryption
            var encryptedDataKey = AES.Process(randomKey, commonSecret, randomIvForKey, true);

            return new EncryptedDataContainer()
            {
                DataIv = randomIv,
                EncryptedData = encryptedData,
                EncryptedDataKey = encryptedDataKey,
                KeyIv = randomIvForKey,
                TempPublicKey = EcKeySerializer.SerializeEcPublicKey(tempPair.Public),
                OwnerHash = SHA3_512.Process(EcKeySerializer.SerializeEcPublicKey(recieversPublicKey))
            };
        }
    }
}