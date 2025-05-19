using Hyland.Unity;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using UCBS.BRY.Client;

namespace BLY_DLL
{
    internal class Program
    {
        
        static int Main(string[] args)
        {
            List<Participante> participantes = new List<Participante>();
            Participante participante = new Participante();
            Documento documento = new Documento();

            OnBase onbase = new OnBase();
            onbase.GetApplication();
            Application app = onbase.app;

            bool resposta = true;

            participante = new Participante();
            Request request = new Request();
            Settings.IsProduct = true;
            request.GetToken();
            Document doc = onbase.GetDocumentByiD(21933065); //args.Document;
            Rendition rendition = doc.DefaultRenditionOfLatestRevision;
            NativeDataProvider ndp = app.Core.Retrieval.Native;
            int paginaAssinatura = 1;

            using (PageData pageData = ndp.GetDocument(rendition))
            {
                using (Stream stream = pageData.Stream)
                {
                    app.Diagnostics.Write("Copiando o pagedata do documento para MemoryStream");
                    documento.DocumentStream = new MemoryStream();
                    stream.CopyTo(documento.DocumentStream);
                    documento.DocumentStream.Flush();
                    documento.DocumentStream.Seek(0, SeekOrigin.Begin);
                    if (rendition.NumberOfPages > 1)
                    {
                        paginaAssinatura = (int)rendition.NumberOfPages;
                    }
                }
            }
            #region configuração da Assinatura
            Config_Assinatura conf_assi = new Config_Assinatura()
            {
                paginas = paginaAssinatura.ToString(),
                x = "66.02456597223012",
                y = "62.91012013754247"
            };
            Config_Rubrica conf_rubrica = null;
            #endregion

            request.GetToken();
            request.CriaAssinatura(participante);
            request.CriaRublica(participante);

            #region Salva e recupera o documento Novamente
            string NomeArquivo = doc.Name;
            NomeArquivo = NomeArquivo.Replace("'", " ");
            NomeArquivo = NomeArquivo.Replace("/", " ");
            NomeArquivo = NomeArquivo.Replace("\\", " ");
            NomeArquivo = NomeArquivo.Replace(":", " ");
            File.WriteAllBytes(@"C:\Temp\AssinaturaDocumentos\" + NomeArquivo + ".pdf", documento.DocumentStream.ToArray());
            string filePath = @"C:\Temp\AssinaturaDocumentos\" + NomeArquivo + ".pdf";
            documento.DocumentData = File.ReadAllBytes(filePath);
            #endregion

            documento.evento = string.Empty;
            BryResponse response = request.AssinarDocumento(participante, conf_assi, conf_rubrica, documento, returnDocumet: true).GetAwaiter().GetResult();
            app.Diagnostics.Write(response.JsonResponse);

            if (response.Status.Equals("OK"))
            {
                app.Diagnostics.Write("Documento Assinado com sucesso");
                string temp = extrairBase64(response.JsonResponse);
                //string NomeArquivo = doc.Name.Replace("/", "-");
                NomeArquivo = NomeArquivo.Replace("\\", "-");
                File.WriteAllBytes(@"C:\Temp\AssinaturaDocumentos\" + NomeArquivo + ".pdf", Convert.FromBase64String(temp));
                //CreateNewRevision(app, doc, @"C:\Temp\AssinaturaDocumentos\" + NomeArquivo + ".pdf");
                resposta = true;
            }
            else
            {
                Console.WriteLine("Erro na assinatura Eletronica do documento");
                Console.WriteLine(response.Message);
                resposta = false;
            }

            onbase.Disconnect();
            return 1;
        }

        #region Corrige Documento Assinado
        public static void CorrigeDoc()
        {
            OnBase onbase = new OnBase();
            onbase.GetApplication();

            Document doc = onbase.GetDocumentByiD(14306012);
            string NomeDocumento = doc.Name;
            NomeDocumento = NomeDocumento.Replace("'", " ");
            //CreateNewRevision(onbase.app, doc, @"C:\Temp\" + NomeDocumento + ".pdf");
            onbase.Disconnect();
        }
        #endregion

        #region Create new Revision
        public static void CreateNewRevision(Application app, Document currentDoc, string filepath)
        {
            Storage storage = app.Core.Storage;

            FileType fileType = app.Core.FileTypes.Find(currentDoc.DefaultFileType.ID);

            StoreRevisionProperties storeRevisionProperties = storage.CreateStoreRevisionProperties(currentDoc, fileType);

            storeRevisionProperties.Comment = "Documento assinado pela BRY";
            List<string> listDocument = new List<string>
            {
                filepath
            };
            Document newDocument = storage.StoreNewRevision(listDocument, storeRevisionProperties);
        }
        #endregion

        #region Save Stream As File - Metodos
        public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
        {
            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists)
            {
                info.Create();
            }
            string path = Path.Combine(filePath, fileName);
            using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            {
                Console.WriteLine(outputFileStream.Length.ToString());
                inputStream.CopyTo(outputFileStream);
            }
        }
        public static void SaveStreamAsFile(string filePath, byte[] inputStream, string fileName)
        {
            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists)
            {
                info.Create();
            }
            File.WriteAllBytes(filePath + fileName, inputStream);
        }
        public static void SaveStreamAsFile(string filePath, string base64File, string fileName)
        {
            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists)
            {
                info.Create();
            }
            File.WriteAllBytes(@"C:\Temp\" + fileName + ".pdf", Convert.FromBase64String(base64File));
        }
        #endregion



        #region ExtrairBase64
        public static string extrairBase64(string response)
        {
            int startIndex = response.IndexOf("'evento': '") + "'evento': '".Length;
            int endIndex = response.IndexOf("'", startIndex);
            string evento = response.Substring(startIndex, endIndex - startIndex);
            return evento;
        }
        #endregion

        public static List<Participante> PreencheParticipantes()
        {
            List<Participante> tempParticipantes = new List<Participante>()
            {
                new Participante()
                {
                    nome = "Lucas Vinicius da Silva Camilo",
                    iniciais = "LC",
                    text = "",
                    tipo = TIPOPARTICIPANTE.MEDICO,
                    trail = "Documento assinado via E-mail em: 07/03/2025 12:00",
                    geolocation = "-22.984780;-47.477479",
                    ip = "200.236.225.131",
                    email = "lucas.camilo@unimedcbs.com.br",
                    cpf = "479.176.008-58"
                },

                new Participante()
                {
                    nome = "Lucas Vinicius da Silva Camilo",
                    iniciais = "LC",
                    text = "",
                    tipo = TIPOPARTICIPANTE.BENEFICIARIO,
                    trail = "Documento assinado via E-mail em: 07/03/2025 12:00",
                    geolocation = "-22.984780;-47.477479",
                    ip = "200.236.225.131",
                    email = "lucas.camilo@unimedcbs.com.br",
                    cpf = "479.176.008-58"
                },

                new Participante()
                {
                    nome = "Lucas Vinicius da Silva Camilo",
                    iniciais = "LC",
                    text = "",
                    tipo = TIPOPARTICIPANTE.INTERMEDIARIO,
                    trail = "Documento assinado via E-mail em: 07/03/2025 12:00",
                    geolocation = "-22.984780;-47.477479",
                    ip = "200.236.225.131",
                    email = "lucas.camilo@unimedcbs.com.br",
                    cpf = "479.176.008-58"
                }
            };
            return tempParticipantes;
        }

    }
}
