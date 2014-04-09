using System;
using System.Linq;
using Crypto.Utils;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Crypto.Sym
{
    public class AES
    {
        public static byte[] Process(byte[] message, byte[] key, byte[] salt, bool encrypt)
        {
            if (key.Length != Consts.SYMMETRIC_KEY_SIZE || salt.Length != Consts.SMALL_SALT_SIZE)
            {
                throw new ArgumentException("Password must be " + Consts.SYMMETRIC_KEY_SIZE + " bytes long and salt " + Consts.SMALL_SALT_SIZE + " bytes long!");
            }
            var paramz = new ParametersWithIV(new KeyParameter(key), salt);
            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesFastEngine()), new Pkcs7Padding());
            cipher.Init(encrypt, paramz);
            var processed = new byte[cipher.GetOutputSize(message.Length)];
            var outputLength = cipher.ProcessBytes(message, 0, message.Length, processed, 0);
            outputLength += cipher.DoFinal(processed, outputLength);

            if (outputLength == processed.Length)
            {
                return processed;
            }
            else
            {
                return processed.Take(outputLength).ToArray();
            }
        }
    }
}