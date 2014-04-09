namespace Crypto.Hash
{
    public class SHA3_256
    {
        public static byte[] Process(byte[] data)
        {
            using (var sha = new SHA3.SHA3Managed(256))
            {
                return sha.ComputeHash(data);    
            }
        }
    }
}