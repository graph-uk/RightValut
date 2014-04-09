using Crypto.Hash;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;

namespace Crypto.Asym
{
    public class ECDH
    {
        public static byte[] CalculateCommonSecret(ICipherParameters privateKey, ICipherParameters publicKey)
        {
            var agreement = new ECDHBasicAgreement();
            agreement.Init(privateKey);
            return SHA3_256.Process(agreement.CalculateAgreement(publicKey).ToByteArrayUnsigned());
        }
    }
}