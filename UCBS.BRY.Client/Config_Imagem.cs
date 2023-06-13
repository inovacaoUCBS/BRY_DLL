using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCBS.BRY.Client
{
    public class Config_Imagem
    {
        public string altura;
        public string largura;
        public string coordenadaX;
        public string coordenadaY;
        public string posicaoBorda;
        public string pagina;
        public string numeroPagina;
        public string posicao;
    }
    public class Config_Anotacao : Config_Imagem
    {
        public Tipo tipo;
        public Valor valor;
    }
    public enum Tipo
    {
        IMAGEM,
        TEXTO
    }
    public class Valor
    {
        public string nome;
    }

}
