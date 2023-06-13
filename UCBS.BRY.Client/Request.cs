using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace UCBS.BRY.Client
{
    public class Request
    {
        public string erro { get; set; }
        public string data_temp { get; set; }
        public string jsonMontado { get; set; }
        public void GetToken()
        {
            var parametro = new Dictionary<string, string>();
            parametro.Add("grant_type", "client_credentials");
            parametro.Add("client_id", "5ee6c821-48ea-41e1-8a58-e23decc2d8d6");
            parametro.Add("client_secret", "+WILHQ/52upJiNinmkMamh7ZrsqwM7hACRl9Li1J2EhIlVzw/2UVRQ==");

            using (HttpClient request = new HttpClient())
            {
                request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                HttpResponseMessage response = request.PostAsync((Settings.BASE_URL + "/token-service/jwt"), new FormUrlEncodedContent(parametro)).Result;
                var token = response.Content.ReadAsStringAsync().Result;
                Settings.Token = JsonConvert.DeserializeObject<TOKEN>(token);
            }

        }
        public bool CriaAssinatura(List<Participante> participantes)
        {
            if (ValidarParticipantes(participantes))
            {
                foreach (Participante participante in participantes)
                {
                    var requisicao = WebRequest.CreateHttp(Settings.BASE_URL_IMAGE + "/api/v5/image");
                    requisicao.Method = "POST";
                    requisicao.ContentType = "application/json";
                    requisicao.Headers.Add("Authorization", "Bearer " + Settings.Token.access_token);
                    requisicao.UserAgent = "ECM_WEB_Agent";

                    string json = "";

                    using (var streamWriter = new StreamWriter(requisicao.GetRequestStream())) //Criar json do Body
                    {
                        json = @"{{
                            ""template"": ""IMAGE-001"",
                            ""name"": ""{0}"",
                            ""text"": ""{1}"",
                            ""font"": ""bilbo""
                    }}";

                        json = string.Format(json, participante.nome, participante.text);
                        jsonMontado = json;
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    try
                    {
                        var httpResponse = (HttpWebResponse)requisicao.GetResponse();
                        data_temp = httpResponse.ToString();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var data = streamReader.ReadToEnd();
                            participante.imagem_assinatura = data;
                            streamReader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        erro = ex.Message;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                erro = "Lista de Participantes Inválida";
                return false;
            }
        }
        public bool CriaAssinatura(Participante participante)
        {
            var requisicao = WebRequest.CreateHttp(Settings.BASE_URL_IMAGE + "/api/v5/image");
            requisicao.Method = "POST";
            requisicao.ContentType = "application/json";
            requisicao.Headers.Add("Authorization", "Bearer " + Settings.Token.access_token);
            requisicao.UserAgent = "ECM_WEB_Agent";

            string json = "";

            using (var streamWriter = new StreamWriter(requisicao.GetRequestStream())) //Criar json do Body
            {
                json = @"{{
                            ""template"": ""IMAGE-001"",
                            ""name"": ""{0}"",
                            ""text"": ""{1}"",
                            ""font"": ""bilbo""
                    }}";

                json = string.Format(json, participante.nome, participante.text);
                jsonMontado = json;
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            bool retorno = true;
            try
            {
                var httpResponse = (HttpWebResponse)requisicao.GetResponse();
                data_temp = httpResponse.ToString();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var data = streamReader.ReadToEnd();
                    participante.imagem_assinatura = data;
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                retorno = false;
            }
            return retorno;
        }
        public bool CriaRublica(List<Participante> participantes)
        {
            if (ValidarParticipantes(participantes))
            {
                foreach (Participante participante in participantes)
                {
                    var requisicao = WebRequest.CreateHttp(Settings.BASE_URL_IMAGE + "/api/v5/image");
                    requisicao.Method = "POST";
                    requisicao.ContentType = "application/json";
                    requisicao.Headers.Add("Authorization", "Bearer " + Settings.Token.access_token);
                    requisicao.UserAgent = "ECM_WEB_Agent";

                    string json = "";

                    using (var streamWriter = new StreamWriter(requisicao.GetRequestStream())) //Criar json do Body
                    {
                        json = @"{{
                            ""template"": ""IMAGE-001"",
                            ""name"": ""{0}"",
                            ""text"": ""{1}"",
                            ""font"": ""bilbo""
                    }}";

                        json = string.Format(json, participante.iniciais, participante.text);

                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    try
                    {
                        var httpResponse = (HttpWebResponse)requisicao.GetResponse();

                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var data = streamReader.ReadToEnd();
                            participante.imagem_iniciais = data;
                            streamReader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        erro = ex.Message;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                erro = "Lista de Participantes Inválida";
                return false;
            }
        }
        public bool CriaRublica(Participante participante)
        {

            var requisicao = WebRequest.CreateHttp(Settings.BASE_URL_IMAGE + "/api/v5/image");
            requisicao.Method = "POST";
            requisicao.ContentType = "application/json";
            requisicao.Headers.Add("Authorization", "Bearer " + Settings.Token.access_token);
            requisicao.UserAgent = "ECM_WEB_Agent";

            string json = "";

            using (var streamWriter = new StreamWriter(requisicao.GetRequestStream())) //Criar json do Body
            {
                json = @"{{
                    ""template"": ""IMAGE-001"",
                    ""name"": ""{0}"",
                    ""text"": ""{1}"",
                    ""font"": ""bilbo""
                }}";

                json = string.Format(json, participante.iniciais, participante.text);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            bool retorno = true;
            try
            {
                var httpResponse = (HttpWebResponse)requisicao.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var data = streamReader.ReadToEnd();
                    participante.imagem_iniciais = data;
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                retorno =  false;
            }
            return retorno;
        }
        public async Task<BryResponse> SignMedico(Participante medico, Documento documento)
        {
            BryResponse bryResponse = new BryResponse();
            #region Json Assinatura
            string jsonAssinatura = String.Format(@"
                    {{
                        ""config"":[
                                        {{
                                            ""x"":111.59694993857701,
			                                ""y"":204.4577901934419, 
			                                ""scale"":0.3,
			                                ""pages"": ""13""
                                        }}
	                                ],
	                                ""data"":""{0}""
                    }}", medico.imagem_assinatura.ToString());
            #endregion

            #region Json Inicials
            string jsonInicial = String.Format(@"
                    {{
	                    ""config"":[
                            {{
                                ""x"":187.76339089320695,
			                    ""y"":153.2214649809956,
			                    ""pages"": ""2-12"",
			                    ""scale"": 0.5
                            }}
	                    ],
	                    ""data"":""{0}""
                    }}", medico.imagem_iniciais);
            #endregion

            if (documento.comDLP)
            {
                jsonAssinatura = String.Format(@"
                    {{
                        ""config"":[
                                        {{
                                            ""x"":110.55541087964286,
			                                ""y"":205.37418904323442, 
			                                ""scale"":0.3,
			                                ""pages"": ""14""
                                        }},
                                        {{
                                            ""x"":28.093605324077437,
			                                ""y"":159.81254822533015, 
			                                ""scale"":0.3,
			                                ""pages"": ""18""
                                        }}
	                                ],
	                                ""data"":""{0}""
                    }}", medico.imagem_assinatura.ToString());
            }

            var requestContent = new MultipartFormDataContent();
            MemoryStream streamMem = new MemoryStream();
            documento.DocumentStream.CopyTo(streamMem);
            documento.DocumentStream.Flush();
            documento.DocumentStream.Seek(0, SeekOrigin.Begin);

            var fileStream = documento.DocumentData;
            var streamContentDocument = new StreamContent(new MemoryStream(fileStream));

            requestContent.Add(new StringContent(medico.nome), "name");
            requestContent.Add(new StringContent(medico.email), "email");
            requestContent.Add(new StringContent(medico.cpf), "cpf");
            requestContent.Add(new StringContent(medico.ip), "ip");
            requestContent.Add(new StringContent(medico.geolocation), "geolocation");
            requestContent.Add(new StringContent("password"), "authentication");
            requestContent.Add(new StringContent("Documento Assinado via e-mail"), "trail");
            requestContent.Add(new StringContent(jsonAssinatura), "signature");
            requestContent.Add(new StringContent(jsonInicial), "initials");
            requestContent.Add(streamContentDocument, "document", "document.pdf");

            HttpClient request = new HttpClient();
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/form-data"));
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);

            var response = await request.PostAsync((Settings.BASE_URL_IMAGE + "/api/v5/sign"), requestContent).ConfigureAwait(false);
            bryResponse.StatusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string evento = response.Content.ReadAsStringAsync().Result;
                documento.evento = evento;
                bryResponse.JsonResponse = $"'evento': '{evento}' ";
                bryResponse.Status = "OK";
                bryResponse.ReturnType = ReturnType.STRING;
            } else
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                documento.evento = null;
                bryResponse.Status = response.StatusCode.ToString();
            }
           // var evento = response.Content.ReadAsStringAsync().Result;

            //streamContentDocument?.Dispose();

            return bryResponse;
        }
        public async Task<BryResponse> SignIntermedirio(Participante intermediario, Documento documento)
        {
            BryResponse bryResponse = new BryResponse();
            #region Json Assinatura
            string jsonAssinatura = String.Format(@"
                    {{
                        ""config"":
                        [
                            {{
                                ""x"": 105.84511944170622,
                                ""y"": 138.9084743923801, 
                                ""scale"":0.3,
                                ""pages"": ""15""
                            }}
                        ],
                        ""data"":""{0}""
                }}", intermediario.imagem_assinatura.ToString());
            #endregion

            #region Json Inicials
            string jsonInicial = String.Format(@"
                    {{
	                    ""config"":[
                            {{
                                ""x"":187.76339089320695,
			                    ""y"":153.82616432518003,
			                    ""pages"": ""2-12"",
			                    ""scale"": 0.5
                            }}
	                    ],
	                    ""data"":""{0}""
                    }}", intermediario.imagem_iniciais);
            #endregion

            if (documento.comDLP)
            {
                jsonAssinatura = String.Format(@"
                    {{
	                    ""config"":
	                    [
		                    {{
 			                    ""x"": 107.41960841050668,
			                    ""y"": 134.81863348767047, 
			                    ""scale"":0.3,
			                    ""pages"": ""16""
		                    }}
	                    ],
 	                    ""data"":""{0}""
                    }}", intermediario.imagem_assinatura);
            }
            Console.WriteLine(jsonAssinatura);
            Console.WriteLine(jsonInicial);
            var requestContent = new MultipartFormDataContent();
            //MemoryStream streamMem = new MemoryStream();
            //documento.DocumentStream.CopyTo(streamMem);
            //documento.DocumentStream.Flush();
            //documento.DocumentStream.Seek(0, SeekOrigin.Begin);

            var fileStream = documento.DocumentData;
            var streamContentDocument = new StreamContent(new MemoryStream(fileStream));

            requestContent.Add(new StringContent(intermediario.nome), "name");
            requestContent.Add(new StringContent(intermediario.email), "email");
            requestContent.Add(new StringContent(intermediario.cpf), "cpf");
            requestContent.Add(new StringContent(intermediario.ip), "ip");
            requestContent.Add(new StringContent(intermediario.geolocation), "geolocation");
            requestContent.Add(new StringContent("link enviado via E-mail"), "authentication");
            requestContent.Add(new StringContent("Documento Assinado via e-mail"), "trail");
            requestContent.Add(new StringContent(jsonAssinatura), "signature");
            requestContent.Add(new StringContent(jsonInicial), "initials");
            requestContent.Add(streamContentDocument, "document", "document.pdf");

            HttpClient request = new HttpClient();
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/form-data"));
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);

            var response = await request.PostAsync((Settings.BASE_URL_IMAGE + "/api/v5/sign"), requestContent).ConfigureAwait(false);
            bryResponse.StatusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string evento = response.Content.ReadAsStringAsync().Result;
                documento.evento = evento;
                bryResponse.JsonResponse = $"'evento': '{evento}' ";
                bryResponse.Status = "OK";
                bryResponse.ReturnType = ReturnType.STRING;
            }
            else
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                documento.evento = null;
                bryResponse.Status = response.StatusCode.ToString();
            }

            return bryResponse;
        }
        private bool ValidarParticipantes(List<Participante> participantes)
        {
            bool ParticipantesValidos = true;
            bool hasMedico, hasIntermediario, hasBeneficiario;
            hasMedico = hasIntermediario = hasBeneficiario = false;

            if (participantes == null)
            {
                ParticipantesValidos = false;
            }
            else
            {
                if (participantes.Count < 3)
                {
                    ParticipantesValidos = false;
                }
                else
                {
                    hasMedico = (participantes.FindAll(x => x.tipo == TIPOPARTICIPANTE.MEDICO)).Count == 1;
                    hasIntermediario = (participantes.FindAll(x => x.tipo == TIPOPARTICIPANTE.INTERMEDIARIO)).Count == 1;
                    hasBeneficiario = (participantes.FindAll(x => x.tipo == TIPOPARTICIPANTE.BENEFICIARIO)).Count == 1;

                    if (!hasMedico || !hasIntermediario || !hasBeneficiario)
                    {
                        ParticipantesValidos = false;
                    }
                }
            }

            return ParticipantesValidos;
        }
        public async Task<BryResponse> AdicionaAssinatura(Participante participante, Documento documento, bool output = false)
        {
            BryResponse bryResponse = new BryResponse();
            #region Json de resposta
            string jsonOutput = String.Format(@"
            {{
	            ""format"":""base64"",
                ""language"":""pt-br"",
	            ""includes"": 
                [
                        {{
                            ""type"": ""document""
                        }},
		                {{
                            ""type"":""report"",
			                ""template"":""ES-002"",
			                ""qrCode"":""texto de test"",
			                ""title"": ""Documento de Padronização: Representação Assinatura Eletrônica"",
			                ""key"": ""chave indicada pelo cliente""
                        }}
	            ]
            }}");
            #endregion

            #region Json Assinatura
            string jsonAssinatura = "";
            string jsonInicial = "";
            #region Assinatura Beneficiario
            if (participante.tipo == TIPOPARTICIPANTE.BENEFICIARIO)
            {
                jsonAssinatura = String.Format(@"
                {{
	                ""config"":
	                [
		                {{
 			                ""x"": 15.22043185764071,
			                ""y"":206.09472656252703, 
			                ""scale"":0.3,
			                ""pages"": ""13""
		                }},
		                {{
			                ""x"":22.182130396312804,
			                ""y"":138.9084743923801, 
			                ""scale"":0.3,
			                ""pages"": ""15""
		                }}
	                ],
 	                ""data"":""{0}""
                }}", participante.imagem_assinatura);

                jsonInicial = String.Format(@"
                    {{
	                    ""config"":[
                            {{
                                ""x"":187.76339089320695,
			                    ""y"":165.0917329691483,
			                    ""pages"": ""2-12"",
			                    ""scale"": 0.5
                            }}
	                    ],
	                    ""data"":""{0}""
                    }}", participante.imagem_iniciais);

                if (documento.comDLP)
                {
                    jsonAssinatura = String.Format(@"
                    {{
	                    ""config"":
	                    [
		                    {{
 			                    ""x"": 12.953559027779328,
			                    ""y"": 205.37418904323442, 
			                    ""scale"":0.3,
			                    ""pages"": ""14""
		                    }},
		                    {{
			                    ""x"": 20.793065200619772,
			                    ""y"": 134.81863348767047, 
			                    ""scale"":0.3,
			                    ""pages"": ""16""
		                    }},
                            {{
			                    ""x"":29.661506558645527,
			                    ""y"":194.17571694961407, 
			                    ""scale"":0.3,
			                    ""pages"": ""18""
                            }}
	                    ],
 	                    ""data"":""{0}""
                    }}", participante.imagem_assinatura);
                }
            }

            #endregion

            #region Assinatura Intermediario
            if (participante.tipo == TIPOPARTICIPANTE.INTERMEDIARIO)
            {
                jsonAssinatura = String.Format(@"
                {{
	                ""config"":
	                [
		                {{
 			                ""x"": 105.84511944170622,
			                ""y"": 134.3900368679249, 
			                ""scale"":0.3,
			                ""pages"": ""15""
		                }}
	                ],
 	                ""data"":""{0}""
                }}", participante.imagem_assinatura);

                jsonInicial = String.Format(@"
                    {{
	                    ""config"":[

                            {{
                                ""x"":187.76339089320695,
			                    ""y"":144.00,
			                    ""pages"": ""2-12"",
			                    ""scale"": 0.5
                            }}
	                    ],
	                    ""data"":""{0}""
                    }}", participante.imagem_iniciais);

                if (documento.comDLP)
                {
                    jsonAssinatura = String.Format(@"
                    {{
	                    ""config"":
	                    [
		                    {{
 			                    ""x"": 107.41960841050668,
			                    ""y"": 134.81863348767047, 
			                    ""scale"":0.3,
			                    ""pages"": ""16""
		                    }}
	                    ],
 	                    ""data"":""{0}""
                    }}", participante.imagem_assinatura);
                }
            }
            #endregion

            #endregion

            #region Processo de assinatura
            var requestContent = new MultipartFormDataContent();
            //Console.WriteLine(Convert.ToBase64String(documento.DocumentData));
            var fileStream = documento.DocumentData;
            var streamContentDocument = new StreamContent(new MemoryStream(fileStream));

            requestContent.Add(new StringContent(participante.nome), "name");
            requestContent.Add(new StringContent(participante.email), "email");
            requestContent.Add(new StringContent(participante.cpf), "cpf");
            requestContent.Add(new StringContent(participante.ip), "ip");
            requestContent.Add(new StringContent(participante.geolocation), "geolocation");
            requestContent.Add(new StringContent("password"), "authentication");
            requestContent.Add(new StringContent("Documento Assinado via e-mail"), "trail");
            requestContent.Add(new StringContent(jsonAssinatura), "signature");
            requestContent.Add(new StringContent(jsonInicial), "initials");
            requestContent.Add(new StringContent(documento.evento), "event");
            
            if (output)
            {
                requestContent.Add(streamContentDocument, "document", "document.pdf");
                //await Console.WriteLine(Convert.ToBase64String( streamContentDocument.ReadAsByteArrayAsync());
                //byte[] contentAsString = await streamContentDocument.ReadAsByteArrayAsync();
                //Console.WriteLine(Convert.ToBase64String(contentAsString));
                requestContent.Add(new StringContent(jsonOutput), "output");
            }

            using (HttpClient request = new HttpClient(new HttpClientHandler() { UseProxy = false}))
            {
                request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/form-data"));
                request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);

                using (var response = await request.PostAsync((Settings.BASE_URL_IMAGE + "/api/v5/sign"), requestContent))
                {
                    bryResponse.StatusCode = response.StatusCode;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //var seriResponse = await response.Content.ReadAsStringAsync();
                        string evento = response.Content.ReadAsStringAsync().Result;
                        documento.evento = evento;
                        bryResponse.JsonResponse = $"{{'evento': '{evento}' }}";
                        bryResponse.Status = "OK";
                        if (output)
                            bryResponse.ReturnType = ReturnType.BASE64;
                        else
                            bryResponse.ReturnType = ReturnType.STRING;
                    }
                    else
                    {
                        bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                        documento.evento = null;
                        bryResponse.Status = response.StatusCode.ToString();
                        bryResponse.ReturnType = ReturnType.STRING;
                    }

                    //var seriResponse = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("Status: " + response.StatusCode.ToString());
                    //Console.WriteLine("Resposta da Requisição: " + seriResponse.ToString());
                    response.Content.Dispose();
                    return bryResponse;
                }
            }
            #endregion
        }
        public async Task<BryResponse> AssinaturaDigitalUnica(Participante participante, Documento documento)
        {
            if (!(participante.tipo == TIPOPARTICIPANTE.MEDICO))
            {
                throw new Exception("Assinante não é um Médico");
            }

            BryResponse bryResponse = new BryResponse();
            DadosAssinatura dadosAssinatura = new DadosAssinatura();
            Config_Imagem config_imagem = new Config_Imagem()
            {
                altura = "30",
                largura = "50",
                coordenadaX = "111.59694993857701", 
                coordenadaY = "-70",
                posicaoBorda = "",
                numeroPagina = "13",
                posicao = "SUPERIOR_ESQUERDO"
            };
            if(documento.comDLP)
            {
                config_imagem = new Config_Imagem()
                {
                    altura = "30",
                    largura = "50",
                    coordenadaX = "48.20",
                    coordenadaY = "-108",
                    posicaoBorda = "",
                    numeroPagina = "18",
                    posicao = "SUPERIOR_ESQUERDO"
                };
            }

            KmsData kmsData = new KmsData();
            kmsData.pin = "VW5pbWVkQ0JTQDIwMjI="; 
            kmsData.uuid_cert = "426b5460-f972-4c8a-889b-fbbbbb7088b8";
            var requestContent = new MultipartFormDataContent();

            dadosAssinatura.kms_data = kmsData;

            string JsonDadosAssinatura = JsonConvert.SerializeObject(dadosAssinatura);
            string JsonConfi_imagem = JsonConvert.SerializeObject(config_imagem);

            Stream streamAssinatura = new MemoryStream(Convert.FromBase64String(participante.imagem_assinatura));

            string kms_data = JsonConvert.SerializeObject(kmsData);

            var streamContentDocument = new StreamContent(new MemoryStream(documento.DocumentData)); // Documento a ser assinado
            var streamContentImage = new StreamContent(streamAssinatura); // Imagem da Assinatura

            requestContent.Add(new StringContent(kms_data), "kms_data");
            requestContent.Add(new StringContent(JsonDadosAssinatura), "dados_assinatura");
            requestContent.Add(new StringContent(JsonConfi_imagem), "configuracao_imagem");
            requestContent.Add(streamContentImage, "imagem", "imagem.jpg");
            requestContent.Add(streamContentDocument, "documento", "documento.pdf");
            //requestContent.Add(new StringContent("false"), "restructure");

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);
            client.DefaultRequestHeaders.Add("kms_type", "BRYKMS");

            var response = await client.PostAsync(Settings.URI_HUB + "/fw/v1/pdf/kms/lote/assinaturas/", requestContent).ConfigureAwait(false);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                bryResponse.Status = response.StatusCode.ToString();
                bryResponse.ReturnType = ReturnType.BASE64;

                bryResponse.JsonResponse = bryResponse.JsonResponse.Replace("[\"", "");
                bryResponse.JsonResponse = bryResponse.JsonResponse.Replace("\"]", "");
            }
            else
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                bryResponse.Status = response.StatusCode.ToString();
                bryResponse.Message  = response.Content.ReadAsStringAsync().Result;
            }
            byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            Console.WriteLine("Resposta da requisição");
            Console.WriteLine(bytes);
            return bryResponse;
        }
        public async Task<BryResponse> AssinaturaDigitalDupla(Participante participante, Documento documento, int config = 1)
        {
            if(!(participante.tipo == TIPOPARTICIPANTE.MEDICO))
            {
                throw new Exception("Assinante não é um Médico");
            }

            BryResponse bryResponse = new BryResponse();
            //DadosAssinatura dodosAssinatura = new DadosAssinatura();
            Config_Imagem config_imagem = new Config_Imagem()
            {
                altura = "30",
                largura = "50",
                coordenadaX = "111.59694993857700",
                coordenadaY = "-70",
                posicaoBorda = "",
                numeroPagina = "14",
                posicao = "SUPERIOR_ESQUERDO"
            };

            Stream streamAssinatura = new MemoryStream(Convert.FromBase64String(participante.imagem_assinatura));
            //Stream StreamRublica = new MemoryStream(Convert.FromBase64String(participante.imagem_iniciais));

            string JsonConfi_imagem = JsonConvert.SerializeObject(config_imagem);

            var requestContent = new MultipartFormDataContent();
            var streamContentDocument = new StreamContent(documento.DocumentStream); // Documento a ser assinado
            var streamContentImage = new StreamContent(streamAssinatura); // Imagem da Assinatura

            requestContent.Add(streamContentDocument, "documento[0]", "documento.pdf");
            requestContent.Add(new StringContent(JsonConfi_imagem), "configuracao_imagem[0]");
            requestContent.Add(streamContentImage, "imagem[0]", "imagem.jpg");
            requestContent.Add(new StringContent("BASE64"), "returnType");
            requestContent.Add(new StringContent("true"), "restructure");


            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);

            var response = await client.PostAsync(Settings.URI_HUB + "/pdf/v1/customize", requestContent).ConfigureAwait(false);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                bryResponse.Status = response.StatusCode.ToString();
                bryResponse.ReturnType = ReturnType.BASE64;

                bryResponse.JsonResponse = bryResponse.JsonResponse.Replace("[\"", "");
                bryResponse.JsonResponse = bryResponse.JsonResponse.Replace("\"]", "");
            }
            else
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                bryResponse.Status = response.StatusCode.ToString();
                bryResponse.Message = response.Content.ReadAsStringAsync().Result;
            }
            byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            Console.WriteLine("Resposta da requisição");
            Console.WriteLine(bytes);
            return bryResponse;
        }
        public async Task<BryResponse> AdicionarRublica(Participante participante, Documento documento, int pagina)
        {
            if (!(participante.tipo == TIPOPARTICIPANTE.MEDICO))
            {
                throw new Exception("Assinante não é um Médico");
            }

            BryResponse bryResponse = new BryResponse();
            //DadosAssinatura dodosAssinatura = new DadosAssinatura();
            Config_Imagem config_imagem = new Config_Imagem()
            {
                altura = "20",
                largura = "20",
                coordenadaX = "188", //188
                coordenadaY = "-104",
                posicaoBorda = "",
                numeroPagina = pagina.ToString(),
                posicao = "SUPERIOR_ESQUERDO"
            };

            Stream streamAssinatura = new MemoryStream(Convert.FromBase64String(participante.imagem_iniciais));
            //Stream StreamRublica = new MemoryStream(Convert.FromBase64String(participante.imagem_iniciais));

            string JsonConfi_imagem = JsonConvert.SerializeObject(config_imagem);

            var requestContent = new MultipartFormDataContent();
            var streamContentDocument = new StreamContent(new MemoryStream(documento.DocumentData)); // Documento a ser assinado
            var streamContentImage = new StreamContent(streamAssinatura); // Imagem da Assinatura

            requestContent.Add(streamContentDocument, "documento[0]", "documento.pdf");
            requestContent.Add(new StringContent(JsonConfi_imagem), "configuracao_imagem[0]");
            requestContent.Add(streamContentImage, "imagem[0]", "imagem.jpg");
            requestContent.Add(new StringContent("BASE64"), "returnType");
            requestContent.Add(new StringContent("true"), "restructure");

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);

            var response = await client.PostAsync(Settings.URI_HUB + "/pdf/v1/customize", requestContent).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                bryResponse.Status = response.StatusCode.ToString();
                bryResponse.ReturnType = ReturnType.BASE64;

                bryResponse.JsonResponse = bryResponse.JsonResponse.Replace("[\"", "");
                bryResponse.JsonResponse = bryResponse.JsonResponse.Replace("\"]", "");
            }
            else
            {
                bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                bryResponse.Status = response.StatusCode.ToString();
                bryResponse.Message = response.Content.ReadAsStringAsync().Result;
            }
            byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            //Console.WriteLine("Resposta da requisição");
            //Console.WriteLine(bytes);
            return bryResponse;
        }
        public async Task<BryResponse> AssinarDocumento(Participante participante, Config_Assinatura configuracaoAssinatura, 
            Config_Rubrica configuracaoRubrica, Documento documento, bool returnDocumet = false)
        {
            BryResponse bryResponse = new BryResponse();
            #region Json Assinatura
            string jsonAssinatura = String.Format(@"
                    {{
                        ""config"":[
                                        {{
                                            ""x"":{3},
			                                ""y"":{2}, 
			                                ""scale"":0.5,
			                                ""pages"": ""{1}""
                                        }}
	                                ],
	                                ""data"":""{0}""
                    }}", participante.imagem_assinatura.ToString(), configuracaoAssinatura.paginas, configuracaoAssinatura.x, configuracaoAssinatura.y);
            #endregion

            #region Json Inicials
            string jsonInicial = String.Format(@"
                    {{
	                    ""config"":[
                            {{
                                ""x"":{3},
			                    ""y"":{2},
			                    ""pages"": ""{1}"",
			                    ""scale"": 0.5
                            }}
	                    ],
	                    ""data"":""{0}""
                    }}", participante.imagem_iniciais.ToString(), configuracaoRubrica.paginas, configuracaoRubrica.x, configuracaoRubrica.y);
            #endregion

            #region Json de resposta
            string jsonOutput = String.Format(@"
            {{
	            ""format"":""base64"",
                ""language"":""pt-br"",
	            ""includes"": 
                [
                        {{
                            ""type"": ""document""
                        }},
		                {{
                            ""type"":""report"",
			                ""template"":""ES-002"",
			                ""qrCode"":""teste"",
			                ""title"": ""Documento de Padronização: Representação Assinatura Eletrônica"",
			                ""key"": ""chave indicada pelo cliente""
                        }}
	            ]
            }}");
            #endregion

            var requestContent = new MultipartFormDataContent();
            /*
            MemoryStream streamMem = new MemoryStream();
            documento.DocumentStream.CopyTo(streamMem);
            documento.DocumentStream.Flush();
            documento.DocumentStream.Seek(0, SeekOrigin.Begin);
            */
            var fileStream = documento.DocumentStream;
            var streamContentDocument = new StreamContent(fileStream);

            requestContent.Add(new StringContent(participante.nome), "name");
            requestContent.Add(new StringContent(participante.email), "email");
            requestContent.Add(new StringContent(participante.cpf), "cpf");
            requestContent.Add(new StringContent("password"), "authentication");
            requestContent.Add(new StringContent("Documento Assinado via e-mail"), "trail");
            requestContent.Add(new StringContent(jsonAssinatura), "signature");
            requestContent.Add(new StringContent(jsonInicial), "initials");
            requestContent.Add(streamContentDocument, "document", "document.pdf");
            if(!(string.IsNullOrEmpty(documento.evento)))
            {
                requestContent.Add(new StringContent(documento.evento), "event");
            }
            if (returnDocumet)
            {
                requestContent.Add(new StringContent(jsonOutput), "output");
            }
            if (!returnDocumet)
            {
                HttpClient request = new HttpClient();
                request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/form-data"));
                request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);

                var response = await request.PostAsync((Settings.BASE_URL_IMAGE + "/api/v5/sign"), requestContent).ConfigureAwait(false);
                bryResponse.StatusCode = response.StatusCode;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string evento = response.Content.ReadAsStringAsync().Result;
                    documento.evento = evento;
                    bryResponse.JsonResponse = $"'evento': '{evento}' ";
                    bryResponse.Status = "OK";
                    bryResponse.ReturnType = ReturnType.STRING;
                }
                else
                {
                    bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                    documento.evento = null;
                    bryResponse.Status = response.StatusCode.ToString();
                }
            }
            else
            {
                using (HttpClient request = new HttpClient(new HttpClientHandler() { UseProxy = false }))
                {
                    request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/form-data"));
                    request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token.access_token);

                    using(var response = await request.PostAsync((Settings.BASE_URL_IMAGE + "/api/v5/sign"), requestContent))
                    {
                        bryResponse.StatusCode = response.StatusCode;
                        if(response.StatusCode == HttpStatusCode.OK)
                        {
                            string evento = response.Content.ReadAsStringAsync().Result;
                            documento.evento= evento;
                            bryResponse.JsonResponse = $"{{'evento': '{evento}' }}";
                            bryResponse.Status = "OK";
                            if (returnDocumet)
                                bryResponse.ReturnType = ReturnType.BASE64;
                            else
                                bryResponse.ReturnType = ReturnType.STRING;
                        }
                        else
                        {
                            bryResponse.JsonResponse = response.Content.ReadAsStringAsync().Result;
                            documento.evento = null;
                            bryResponse.Status = response.StatusCode.ToString();
                            bryResponse.ReturnType = ReturnType.STRING;
                        }
                        response.Content.Dispose();
                    }
                }
            }
            return bryResponse;
        }
    }
}