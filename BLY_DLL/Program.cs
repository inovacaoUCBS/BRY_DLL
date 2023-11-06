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
        private static Logger nLog = LogManager.GetCurrentClassLogger();
        public static Participante participante = new Participante();
        public static Participante diretor = new Participante();
        public static Participante viceDiretor = new Participante();
        public static Documento documento = new Documento();
        static int Main(string[] args)
        {
            #region Declaração de Variaveis
            participante = new Participante();
            OnBase onbase = new OnBase();
            onbase.GetApplication();
            Application app = onbase.app;
            bool resposta = true;
            Request request = new Request();
            request.GetToken();
            Console.Write(Settings.Token);
            Document doc = onbase.GetDocumentByiD(14179445);

            string NomeDocumento = doc.Name;
            NomeDocumento = NomeDocumento.Replace("'", " ");
            NomeDocumento = NomeDocumento.Replace("/", " ");
            NomeDocumento = NomeDocumento.Replace("\\", " ");

            Rendition rendition = doc.DefaultRenditionOfLatestRevision;
            NativeDataProvider ndp = app.Core.Retrieval.Native;

            #endregion

            
            
            return 1;
        }

        #region Criar Participantes do processo
        public static void CriaParticipante()
        {
            #region Informações Candidato
            participante = new Participante("LUCA VINICIUS DA SILVA CAMILO", "", TIPOPARTICIPANTE.CANDIDATO);
            participante.cpf = "479.176.008-58";
            participante.email = "lucas.camilo@unimedcbs.com.br";
            participante.trail = "Assinado via E-mail";
            #endregion

            #region Informações Diretor
            diretor = new Participante("LUIZ ANTÔNIO BERETA", "", TIPOPARTICIPANTE.CANDIDATO);
            diretor.cpf = "147.771.000-06"; //CPF FALSO
            diretor.email = "luiz.bereta@unimedcbs.com.br";
            diretor.trail = "Assinado via E-mail";
            #endregion

            #region Informações Vice-Diretor
            viceDiretor = new Participante("SÉRGIO PASCHOALICK CATHERINO", "", TIPOPARTICIPANTE.CANDIDATO);
            viceDiretor.cpf = "147.771.000-06"; //CPF FALSO
            viceDiretor.email = "sergio.catherino@unimedcbs.com.br";
            viceDiretor.trail = "Assinado via E-mail";
            #endregion

        }
        #endregion

        #region Corrige Documento Assinado
        public static void CorrigeDoc()
        {
            OnBase onbase = new OnBase();
            onbase.GetApplication();

            Document doc = onbase.GetDocumentByiD(14306012);
            string NomeDocumento = doc.Name;
            NomeDocumento = NomeDocumento.Replace("'", " ");
            CreateNewRevision(onbase.app, doc, @"C:\Temp\" + NomeDocumento + ".pdf");
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
            List<string> listDocument = new List<string>();
            listDocument.Add(filepath);
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

        #region Fluxo de assinatura Eletronica
        public static bool metodoAssinaturaEletonica()
        {
            #region Cria Variaveis
            bool resposta = true;
            Request request = new Request();
            request.GetToken();
            #endregion

            #region Lista de Participantes
            List<Participante> participantes = new List<Participante>();
            #endregion

            #region Conexão com Onbase
            OnBase onbase = new OnBase();
            onbase.GetApplication();
            if (onbase.app == null)
            {
                throw new Exception("Erro ao Conectar ao Onbase");
            }
            Document doc = onbase.GetDocumentByiD(13018300); // LUCAS VINICIUS DA SILVA CAMILO
            Documento documento = new Documento();

            Rendition rendition = doc.DefaultRenditionOfLatestRevision;
            NativeDataProvider ndp = onbase.app.Core.Retrieval.Native;
            #endregion

            #region Adicionando Beneficiario
            Participante participante = new Participante("DENIS PEREIRA DA SILVA", "", TIPOPARTICIPANTE.BENEFICIARIO);
            participante.cpf = "16602627473";
            participante.ip = "45.7.109.20";
            participante.email = "DENIS.PEREIRA15@HOTMAIL.COM";
            participante.geolocation = "-9.4110947,-36.6321633";
            participante.trail = "Assinado via E-mail";
            participantes.Add(participante);
            #endregion

            #region Adicionando Intermediario
            participante = new Participante("ROSEANNY MARY ARAUJO SILVA", "", TIPOPARTICIPANTE.INTERMEDIARIO);
            participante.cpf = "86150987434";
            participante.ip = "45.7.109.18";
            participante.email = "ROSEANNY.VENDAS@UNIMEDPALMEIRA.COM.BR";
            participante.geolocation = "-9.4109696,-36.6411776";
            participante.trail = "Assinado via E-mail";
            participantes.Add(participante);
            #endregion

            #region Adicionando Medico
            participante = new Participante("FERNANDO RIOS FONSECA", "", TIPOPARTICIPANTE.MEDICO);
            participante.cpf = "23746";
            participante.ip = "177.137.62.161";
            participante.email = "FERNANDO.RIOS@UNIMEDCBS.COM.BR";
            participante.geolocation = "-26.2302113,-52.6858399";
            participante.trail = "Assinado via E-mail";
            participantes.Add(participante);
            #endregion

            #region Criar assinatura e Rublica
            if (request.CriaAssinatura(participantes))
            {
                if (request.CriaRublica(participantes))
                {
                    using (PageData pageData = ndp.GetDocument(rendition))
                    {
                        using (Stream stream = pageData.Stream)
                        {
                            nLog.Debug("Copiando o pagedata do documento para MemoryStream");
                            documento.DocumentStream = new MemoryStream();
                            documento.comDLP = false;
                            stream.CopyTo(documento.DocumentStream);
                            documento.DocumentStream.Flush();
                            documento.DocumentStream.Seek(0, SeekOrigin.Begin);
                            if (rendition.NumberOfPages > 15)
                            {
                                documento.comDLP = true;
                            }
                        }
                    }
                }
            }
            #endregion

            File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", documento.DocumentStream.ToArray());

            #region Fluxo de assinatura Completo
            documento.comDLP = false;
            BryResponse response = new BryResponse();

            string filePath = "C:\\Temp\\" + doc.Name + ".pdf";
            documento.DocumentData = File.ReadAllBytes(filePath);

            response = request.SignMedico(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO), documento).GetAwaiter().GetResult();
            if (response.Status != "OK")
            {
                Console.WriteLine("Erro na chamada Sign do Médico");
                Console.WriteLine(response.JsonResponse);
                resposta = false;
            }
            else
            {
                response = request.AdicionaAssinatura(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.INTERMEDIARIO), documento).GetAwaiter().GetResult();
                if (response.Status != "OK")
                {
                    Console.WriteLine("Erro na chamada Sign do Médico");
                    Console.WriteLine(response.JsonResponse);
                    resposta = false;
                }
                else
                {
                    filePath = "C:\\Temp\\" + doc.Name + ".pdf";
                    documento.DocumentData = File.ReadAllBytes(filePath);

                    response = request.AdicionaAssinatura(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.BENEFICIARIO), documento, output: true).GetAwaiter().GetResult();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (response.ReturnType == ReturnType.BASE64)
                        {
                            BryEventoResponse eventResponse = JsonConvert.DeserializeObject<BryEventoResponse>(response.JsonResponse);
                            if (eventResponse != null && !string.IsNullOrEmpty(eventResponse.Evento))
                            {
                                File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", Convert.FromBase64String(eventResponse.Evento));
                                resposta = true;
                            }
                        }
                    }
                }
            }
            #endregion

            return resposta;
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

    }
}
