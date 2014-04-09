using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;

namespace Crypto.Utils
{
    public class Random
    {

        public static SecureRandom GetSecureRandom()
        {
            var threaded = new ThreadedSeedGenerator();
            var seed = threaded.GenerateSeed(1024, false);
            var rgen = new VmpcRandomGenerator();
            rgen.AddSeedMaterial(seed);
            return new SecureRandom(rgen);
        }
    }
}