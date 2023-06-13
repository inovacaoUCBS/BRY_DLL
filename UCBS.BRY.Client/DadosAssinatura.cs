using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCBS.BRY.Client
{
    public class DadosAssinatura
    {
        public KmsData kms_data { get; set; }
        public string perfil { get; set; }
        public string algoritmoHash { get; set; }
        public string tipoRetorno { get; set; }
        public string tipoRestricao { get; set; }
        public string signatario { get; set; }

        public DadosAssinatura()
        {
            kms_data = new KmsData();
            perfil = "ADRB";
            algoritmoHash = "SHA256";
            tipoRetorno = "Base64";
            tipoRestricao = "NENHUMA_RESTRICAO";
        }
        
    }
}
