using System;
using System.Security.Cryptography.X509Certificates;
using Crypto.Asym;
using Crypto.Hash;
using Crypto.Model;
using Crypto.Sym;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Crypto.Utils
{
    public class Utils
    {

        public static EncryptedDataContainer EncryptData(byte[] data, byte[] key, SecureRandom random)
        {
            if (key == null || key.Length != Consts.SYMMETRIC_KEY_SIZE)
            {
                throw new ArgumentException("Key size must be 256 bits");
            }
            var randomIv = random.GenerateSeed(Consts.SMALL_SALT_SIZE); //128 random bits
            var encryptedData = AES.Process(data, key, randomIv, true);

            return new EncryptedDataContainer()
            {
                Data = encryptedData,
                IV = randomIv
            };
        }

        public static DataKeyContainer EncryptDataKeyForPublicKey(AsymmetricKeyParameter recieversPublicKey, byte[] key, SecureRandom random)
        {
            if (key == null || key.Length != Consts.SYMMETRIC_KEY_SIZE)
            {
                throw new ArgumentException("Key size must be 256 bits");
            }

            var tempPair = KeyGen.GenerateKeyPair();
            var commonSecret = ECDH.CalculateCommonSecret(tempPair.Private, recieversPublicKey);
            var randomIvForKey = random.GenerateSeed(Consts.SMALL_SALT_SIZE); //128 random bits for key encryption
            var encryptedDataKey = AES.Process(key, commonSecret, randomIvForKey, true);

            return new DataKeyContainer()
            {
                EncryptedDataKey = encryptedDataKey,
                KeyIv = randomIvForKey,
                TempPublicKey = EcKeySerializer.SerializeEcPublicKey(tempPair.Public),
                OwnerHash = SHA3_512.Process(EcKeySerializer.SerializeEcPublicKey(recieversPublicKey))
            };
        }

        public static byte[] DecryptDataKeyWithPrivateKey(AsymmetricKeyParameter privateKey, DataKeyContainer encryptedKey)
        {
            var commonSecret = ECDH.CalculateCommonSecret(privateKey, EcKeySerializer.DeserializeEcPublicKey(encryptedKey.TempPublicKey));
            var decryptedKey = AES.Process(encryptedKey.EncryptedDataKey, commonSecret, encryptedKey.KeyIv, false);
            return decryptedKey;
        }

        public static byte[] DecryptData(EncryptedDataContainer data, byte[] key)
        {
            if (key == null || key.Length != Consts.SYMMETRIC_KEY_SIZE)
            {
                throw new ArgumentException("Key size must be 256 bits");
            }

            return AES.Process(data.Data, key, data.IV, false);
        }
    }
}