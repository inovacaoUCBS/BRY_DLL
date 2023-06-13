using System;
using System.IO; 
using System.Collections.Generic;
using UCBS.BRY.Client;
using System.Threading.Tasks;
using Hyland.Unity;
using NLog;
using Newtonsoft.Json;
using System.Text;

namespace BLY_DLL
{
    internal class Program
    {
        private static Logger nLog = LogManager.GetCurrentClassLogger();
        static int Main(string[] args)
        {
            #region Fluxo de Assinatura EQ - Entrevista Qualificada
            /**/
            #region Cria Variaveis de Controle
            bool resposta = true;
            Request request = new Request();
            request.GetToken();

            #region Lista de Participantes
            List<Participante> participantes = new List<Participante>();
            #endregion

            OnBase onbase = new OnBase();
            onbase.GetApplication();
            if (onbase.app == null)
            {
                return -1;
            }
            Document doc = onbase.GetDocumentByiD(13018300);

            nLog.Debug($"Carregado o documento '{doc.Name}' do onbase. ");
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
                if(request.CriaRublica(participantes))
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
                            if(rendition.NumberOfPages > 15)
                            {
                                documento.comDLP = true;
                            }
                        }
                    }
                }
            }
            #endregion

            //File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", documento.DocumentStream.ToArray());

            #region Fluxo de assinatura Completo
            documento.comDLP = false;
            if (documento.comDLP)
            {
                #region Fluco de documento com DLP

                #region Adiciona assinatura
                string rublicaMedico = participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_iniciais;
                participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_iniciais = rublicaMedico.Split(',')[1];

                string assinaMedico = participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_assinatura;
                participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_assinatura = assinaMedico.Split(',')[1];

                BryResponse response = request.AssinaturaDigitalDupla(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO), documento).Result;
                Console.WriteLine(response.Status);
                #endregion
                if (!(response.Status == "OK"))
                {
                    Console.WriteLine(response.Message);
                    Console.WriteLine("1) Erro na assinatura Digital");
                    resposta = false;
                }
                else
                {
                    Console.WriteLine("Documento assinado com sucesso: Assinatura dupla do Médico");
                    #region Salva Documento e recupera assinado
                    File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", Convert.FromBase64String(response.JsonResponse));
                    using (FileStream file = new FileStream("C:\\Temp\\" + doc.Name + ".pdf", FileMode.Open))
                    {
                        documento.DocumentStream = new MemoryStream();
                        file.CopyTo(documento.DocumentStream);
                        //documento.DocumentStream.Flush();
                        //documento.DocumentStream.Seek(0, SeekOrigin.Begin);
                    }
                    #endregion

                    #region Adiciona imagem da Rublica Rublica
                    for (int i = 2; i < 13; i++)
                    {
                        response = request.AdicionarRublica(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO), documento, i).Result;
                        if (response.Status == "OK")
                        {
                            File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", Convert.FromBase64String(response.JsonResponse));
                            File.WriteAllBytes(@"C:\Temp\" + doc.Name + "-CustomMize.pdf", documento.DocumentStream.ToArray());

                            using (FileStream file = new FileStream("C:\\Temp\\" + doc.Name + ".pdf", FileMode.Open))
                            {
                                documento.DocumentStream = new MemoryStream();
                                file.CopyTo(documento.DocumentStream);
                                //documento.DocumentStream.Flush();
                                //documento.DocumentStream.Seek(0, SeekOrigin.Begin);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Erro na Assinatura");
                            Console.WriteLine("1");
                            Console.WriteLine(response.Message);
                            Console.WriteLine(response.JsonResponse);
                            resposta = false;
                            break;
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                #region Fluxo de documento sem DLP
                string rublicaMedico = participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_iniciais;
                participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_iniciais = rublicaMedico.Split(',')[1];

                string assinaMedico = participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_assinatura;
                participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO).imagem_assinatura = assinaMedico.Split(',')[1];

                string filePath = "C:\\Temp\\" + doc.Name + ".pdf";
                documento.DocumentData = File.ReadAllBytes(filePath);

                #region Adiciona imagem da rublica
                /**
                for (int i = 2; i < 13; i++)
                {
                    using (FileStream file = new FileStream("C:\\Temp\\" + doc.Name + ".pdf", FileMode.Open))
                    {
                        documento.DocumentStream = new MemoryStream();
                        file.CopyTo(documento.DocumentStream);
                        documento.DocumentStream.Flush();
                        documento.DocumentStream.Seek(0, SeekOrigin.Begin);
                    }
                    filePath = "C:\\Temp\\" + doc.Name + ".pdf";
                    documento.DocumentData = File.ReadAllBytes(filePath);

                    BryResponse response1 = request.AdicionarRublica(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO), documento, i).GetAwaiter().GetResult();
                    if (response1.Status == "OK")
                    {
                        File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", Convert.FromBase64String(response1.JsonResponse));
                        //File.WriteAllBytes(@"C:\Temp\" + doc.Name + "-CustomMize.pdf", Convert.FromBase64String(response1.JsonResponse));

                        using (FileStream file = new FileStream("C:\\Temp\\" + doc.Name + ".pdf", FileMode.Open))
                        {
                            documento.DocumentStream = new MemoryStream();
                            file.CopyTo(documento.DocumentStream);
                            //documento.DocumentStream.Flush();
                            //documento.DocumentStream.Seek(0, SeekOrigin.Begin);
                        }
                        filePath = "C:\\Temp\\" + doc.Name + ".pdf";
                        documento.DocumentData = File.ReadAllBytes(filePath);
                    }
                    else
                    {
                        Console.WriteLine("Erro na Assinatura");
                        Console.WriteLine("2");
                        Console.WriteLine(response1.Message);
                        Console.WriteLine(response1.JsonResponse);
                        resposta = false;
                        break;
                    }
                }
                /**/
                #endregion

                #endregion
            }
            if (resposta)
            {
                #region Adiciona assinatura Eletronica Intermediario
                BryResponse bryResponse = request.SignIntermedirio(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.INTERMEDIARIO), documento).GetAwaiter().GetResult();
                if (bryResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Erro na chamada Signin");
                    Console.WriteLine(bryResponse.JsonResponse);
                    resposta = false;
                }
                else
                {
                    #region Captura de documento
                    
                    string filePath = "C:\\Temp\\" + doc.Name + ".pdf";
                    documento.DocumentData = File.ReadAllBytes(filePath);
                   

                    #endregion

                    bryResponse = request.AdicionaAssinatura(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.BENEFICIARIO), documento, output: true).GetAwaiter().GetResult();
                    if(bryResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        #region Salva o documento e recupera Novamente
                        if (bryResponse.ReturnType == ReturnType.BASE64)
                        {
                            BryEventoResponse eventResponse = JsonConvert.DeserializeObject<BryEventoResponse>(bryResponse.JsonResponse);
                            if (eventResponse != null && !string.IsNullOrEmpty(eventResponse.Evento))
                            {
                                Console.WriteLine(eventResponse.Evento);
                                File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", Convert.FromBase64String(eventResponse.Evento));
                                //File.WriteAllBytes(@"C:\Temp\" + doc.Name + "-Eletronica.pdf", Convert.FromBase64String(eventResponse.Evento));
                            }
                            filePath = "C:\\Temp\\" + doc.Name + ".pdf";
                            documento.DocumentData = File.ReadAllBytes(filePath);
                        }
                        
                        #endregion

                        #region Adiciona assinatura Digital
                        BryResponse response = request.AssinaturaDigitalUnica(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.MEDICO), documento).GetAwaiter().GetResult();
                        if (!(response.Status == "OK"))
                        {
                            Console.WriteLine("2) Erro na assinatura Digital");
                            Console.WriteLine(response.Message);
                            resposta = false;
                        }
                        else
                        {
                            Console.WriteLine("Documento Assinado com sucesso");
                            File.WriteAllBytes(@"C:\Temp\" + doc.Name + ".pdf", Convert.FromBase64String(response.JsonResponse));
                            //File.WriteAllBytes(@"C:\Temp\" + doc.Name + "-Digital.pdf", Convert.FromBase64String(response.JsonResponse));
                            Console.WriteLine("Documento Salvo em Pasta temporária");
                            resposta = true;
                            Console.WriteLine("Criando Nova revisão");
                            //CreateNewRevision(onbase.app, doc, @"C:\Temp\" + doc.Name + ".pdf");
                        }
                        #endregion
                    }
                }
                #endregion
            }
            #endregion
            /**/
            #endregion

            #region Fluxo de Assinatura Generica
            /**
            Participante part = new Participante("LUCAS CAMILO", "", TIPOPARTICIPANTE.CANDIDATO);
            part.cpf = "47917600858";
            part.email = "lucas.camilo@unimedcbs.com.br";
            part.trail = "Assinado via E-mail";
            #region Cria imagem da rubria e assinatura
            using (FileStream fileStream = new FileStream("C:\\Users\\lucas.camilo\\Desktop\\Assinatura.png", FileMode.Open))
            {
                byte[] bytes = new byte[fileStream.Length];

                fileStream.Read(bytes, 0, (int)fileStream.Length);

                part.imagem_assinatura = ("data:image/png;base64," + Convert.ToBase64String(bytes));
                part.imagem_iniciais = ("data:image/png;base64,"+Convert.ToBase64String(bytes));
            }
            #endregion
            Config_Assinatura conf_assi = new Config_Assinatura()
            {
                paginas = "1",
                x = 10,
                y = 10
            };
            Config_Rubrica conf_rubrica = new Config_Rubrica()
            {
                paginas = "1",
                x = 50,
                y = 50
            };
            #region Busca documento dentro do Onbase
            OnBase onbase = new OnBase();
            onbase.GetApplication();
            if (onbase.app == null)
            {
                return -1;
            }
            Document doc = onbase.GetDocumentByiD(12663554);
            nLog.Debug($"Carregado odocumento '{doc.Name}' do onbase. ");
            Documento documento = new Documento();
            Rendition rendition = doc.DefaultRenditionOfLatestRevision;
            NativeDataProvider ndp = onbase.app.Core.Retrieval.Native;
            using (PageData pageData = ndp.GetDocument(rendition))
            {
                using (Stream stream = pageData.Stream)
                {
                    nLog.Debug("Copiando o pagedata do documento para MemoryStream");
                    documento.DocumentStream = new MemoryStream();
                    stream.CopyTo(documento.DocumentStream);
                    documento.DocumentStream.Flush();
                    documento.DocumentStream.Seek(0, SeekOrigin.Begin);
                }
            }
            #endregion
            #region Ciclo de Assinatura
            Request request = new Request();
            request.GetToken();
            request.CriaAssinatura(part);
            BryResponse resposta = request.AssinarDocumento(part, conf_assi, conf_rubrica, documento, returnDocumet:true).GetAwaiter().GetResult();
            Console.WriteLine(resposta.Status);
            #endregion
            /**/
            #endregion

            //Console.WriteLine("Resposta final: "+ resposta.ToString());
            //metodoAssinaturaEletonica();
            return 0;
        }

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
            if(!info.Exists)
            {
                info.Create();
            }
            string path = Path.Combine(filePath, fileName);
            using(FileStream outputFileStream = new FileStream(path, FileMode.Create))
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
            if(onbase.app == null)
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
            if(response.Status != "OK")
            {
                Console.WriteLine("Erro na chamada Sign do Médico");
                Console.WriteLine(response.JsonResponse);
                resposta = false;
            }
            else
            {
                response = request.AdicionaAssinatura(participantes.Find(x => x.tipo == TIPOPARTICIPANTE.INTERMEDIARIO), documento).GetAwaiter().GetResult();
                if(response.Status != "OK")
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
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if(response.ReturnType == ReturnType.BASE64)
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
    }
}
