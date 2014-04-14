namespace Crypto.Model
{
    public class EncryptedDataContainer
    {
        public int Id { get; set; }
        public byte[] IV { get; set; }
        public byte[] Data { get; set; }
    }
}