using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities.Encoders;
using Random = Crypto.Utils.Random;

namespace Crypto.Asym
{
    public class KeyGen
    {
        /*
      * sect571k1
      */
        private const int M = 571;
        private const int K1 = 2;
        private const int K2 = 5;
        private const int K3 = 10;

        private static readonly BigInteger a = BigInteger.ValueOf(0);
        private static readonly BigInteger b = BigInteger.ValueOf(1);
        private static readonly BigInteger n = new BigInteger(1, Hex.Decode("020000000000000000000000000000000000000000000000000000000000000000000000131850E1F19A63E4B391A8DB917F4138B630D84BE5D639381E91DEB45CFE778F637C1001"));
        private static readonly BigInteger h = BigInteger.ValueOf(4);

        private static readonly ECCurve Curve = new F2mCurve(M, K1, K2, K3, a, b, n, h);

        private static readonly ECPoint G = Curve.DecodePoint(Hex.Decode("04"
                                                                         + "026EB7A859923FBC82189631F8103FE4AC9CA2970012D5D46024804801841CA44370958493B205E647DA304DB4CEB08CBBD1BA39494776FB988B47174DCA88C7E2945283A01C8972"
                                                                         + "0349DC807F4FBF374F4AEADE3BCA95314DD58CEC9F307A54FFC61EFC006D8A2C9D4979C0AC44AEA74FBEBBB9F772AEDCB620B01A7BA7AF1B320430C8591984F601CD4C143EF1C7A3"));

        public static readonly ECDomainParameters DomParams = new ECDomainParameters(Curve, G, n, h);

        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var gen = new ECKeyPairGenerator();
            var param = new ECKeyGenerationParameters(DomParams, Random.GetSecureRandom());
            gen.Init(param);
            return gen.GenerateKeyPair();
        }
    }
}
