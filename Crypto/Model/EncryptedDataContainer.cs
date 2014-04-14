namespace Crypto.Model
{
    public class EncryptedDataContainer
    {
        public byte[] OwnerHash { get; set; }
        public byte[] DataIv { get; set; }
        public byte[] KeyIv { get; set; }
        public byte[] EncryptedData { get; set; }
        public byte[] EncryptedDataKey{ get; set; }
        public byte[] TempPublicKey{ get; set; }
        
    }
}