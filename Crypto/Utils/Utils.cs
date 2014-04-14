using System;
using System.Security.Cryptography.X509Certificates;
using Crypto.Asym;
using Crypto.Hash;
using Crypto.Model;
using Crypto.Sym;
using Org.BouncyCastle.Crypto.Parameters;

namespace Crypto.Utils
{
    public class Utils
    {

        public static EncryptedDataContainer EncryptedData(byte[] key, byte[] data)
        {
            if (key == null || key.Length != Consts.SYMMETRIC_KEY_SIZE)
            {
                throw new ArgumentException("Key size must be 256 bits");
            }
            var rnd = Random.GetSecureRandom();
            var randomIv = rnd.GenerateSeed(Consts.SMALL_SALT_SIZE); //128 random bits
            var encryptedData = AES.Process(data, key, randomIv, true);

            return new EncryptedDataContainer()
            {
                Data = encryptedData,
                IV = randomIv
            };
        }

        public static DataKeyContainer EncryptDataKeyForPublicKey(ECPublicKeyParameters recieversPublicKey, byte[] key)
        {
            if (key == null || key.Length != Consts.SYMMETRIC_KEY_SIZE)
            {
                throw new ArgumentException("Key size must be 256 bits");
            }
            var rnd = Random.GetSecureRandom();

            var tempPair = KeyGen.GenerateKeyPair();
            var commonSecret = ECDH.CalculateCommonSecret(tempPair.Private, recieversPublicKey);
            var randomIvForKey = rnd.GenerateSeed(Consts.SMALL_SALT_SIZE); //128 random bits for key encryption
            var encryptedDataKey = AES.Process(key, commonSecret, randomIvForKey, true);

            return new DataKeyContainer()
            {
                EncryptedDataKey = encryptedDataKey,
                KeyIv = randomIvForKey,
                TempPublicKey = EcKeySerializer.SerializeEcPublicKey(tempPair.Public),
                OwnerHash = SHA3_512.Process(EcKeySerializer.SerializeEcPublicKey(recieversPublicKey))
            };
        }
    }
}