namespace Crypto.Hash
{
    public class SHA3_512
    {
        public static byte[] Process(byte[] data)
        {
            using (var sha = new SHA3.SHA3Managed(512))
            {
                return sha.ComputeHash(data);    
            }
        }
    }
}