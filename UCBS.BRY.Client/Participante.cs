using System;
using System.Collections.Generic;
namespace UCBS.BRY.Client
{
    public class Participante
    {
        #region Variaveis
        public string nome { get; set; }
        public string iniciais { get; set; }
        public string email { get; set; }
        public string cpf { get; set; }
        public string text { get; set; }
        public string trail { get; set; }
        public string ip { get; set; }
        public string geolocation { get; set; }
        public TIPOPARTICIPANTE tipo { get; set; }
        public string imagem_assinatura { get; set; }
        public string imagem_iniciais { get; set; }
        #endregion

        public Participante()
        {

        }
        public Participante(string _nome, string _text, TIPOPARTICIPANTE _tipo)
        {
            nome = Capitalize(_nome.ToLower());
            text = _text;
            tipo = _tipo;

            CriaIniciais();
        }
        private void CriaIniciais()
        {
            string primeira_letra = nome.Split(' ')[0].ToString().Substring(0, 1);
            string ultima_letra = nome.Split(' ')[0].ToString().Substring(1, 2);

            if (nome.Split(' ').Length > 1)
            {
                primeira_letra = nome.Split(' ')[0].ToString().Substring(0, 1);
                ultima_letra = nome.Split(' ')[nome.Split(' ').Length - 1].ToString().Substring(0, 1);
            }

            iniciais = primeira_letra + ultima_letra;
        }

        private string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            string[] words = input.Split(' ');
            List<string> lowercaseWords = new List<string> { "da", "de", "do", "dos" };

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    if (i == 0 || !lowercaseWords.Contains(words[i].ToLower()))
                    {
                        char[] chars = words[i].ToCharArray();
                        chars[0] = char.ToUpper(chars[0]);
                        words[i] = new string(chars);
                    }
                }
            }
            string retorno = string.Join(" ", words);
            return retorno;
        }
    }
}

public enum TIPOPARTICIPANTE
{
    INTERMEDIARIO,
    BENEFICIARIO,
    MEDICO,
    CANDIDATO
}