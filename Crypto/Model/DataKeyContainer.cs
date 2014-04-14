namespace Crypto.Model
{
    public class DataKeyContainer
    {
        public int Id { get; set; }
        public int DataId { get; set; }
        public byte[] OwnerHash { get; set; }
        public byte[] KeyIv { get; set; }
        public byte[] EncryptedDataKey{ get; set; }
        public byte[] TempPublicKey{ get; set; }
    }
}