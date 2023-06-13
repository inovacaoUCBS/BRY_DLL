using System.IO;

namespace UCBS.BRY.Client
{
    public class Documento
    {
        public MemoryStream DocumentStream { get; set; }
        public byte[] DocumentData { get; set; }
        public string documento_assinado { get; set; }
        public string evento { get; set; }
        public bool comDLP { get; set; }
    }
}
