using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http;
using RestSharp;
using System.Text.Json.Serialization;
using System.IO;

namespace PruebaTecnica
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Variables a utilizar
            var url = "https://candidates-exam.herokuapp.com/api/v1/ping";
            var url2 = "https://candidates-exam.herokuapp.com/api/v1/auth/login";
            var url3 = "https://candidates-exam.herokuapp.com/api/v1/usuarios";
            var urlfinal = "https://candidates-exam.herokuapp.com/api/v1/usuarios/mostrar_cv";
            int opc = 0;
            bool validate = false;
            string capturaurl = "";

            using (var httpClient = new HttpClient()) { 
            do
            {
                Console.WriteLine("Prueba Tecnica, Candidato: Justin Zacarias");
                Menu();
                Console.Write("Ingrese la opcion a trabajar: ");

                opc = Convert.ToInt32(Console.ReadLine());

                switch (opc)
                {
                    case 1:

                            var response = await httpClient.GetAsync(url);
                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                Root myDeserializedClass1 = JsonConvert.DeserializeObject<Root>(content);
                                Console.WriteLine(myDeserializedClass1.tipo + " " + myDeserializedClass1.respuesta);
                                System.Threading.Thread.Sleep(2000);
                            }
                            else
                                Console.WriteLine("Error");

                        Console.Clear();
                        break;
                    case 2:
                            string rnombre = "";
                            string remail = "";
                            string rpassword = "";
                            string rpassword_confirmation = "";
                            bool iguala = false;
                            Console.Clear();
                            Console.WriteLine("Ingrese su nombre");
                            rnombre= Console.ReadLine();
                            Console.WriteLine("Ingrese su email");
                            remail= Console.ReadLine();
                            do
                            {
                                Console.WriteLine("Ingrese su password");
                                rpassword = Console.ReadLine();
                                Console.WriteLine("Confirme su password");
                                rpassword_confirmation = Console.ReadLine();
                                if(rpassword.Equals(rpassword_confirmation))
                                    iguala = true;
                                else
                                {
                                    iguala = false;
                                    Console.WriteLine("Las contraseñas deben ser iguales, intente nuevamente");
                                }
                                    
                            } while (iguala == false);

                            var registro = new Registro()
                            {
                                nombre = rnombre,
                                email = remail,
                                password = rpassword,
                                password_confirmation = rpassword_confirmation
                            };

                            var respuesta = await httpClient.PostAsJsonAsync(url3, registro);
                            Console.WriteLine(await respuesta.Content.ReadAsStringAsync());
                            Console.WriteLine("iNGRESASTE TUS DATOS CORRECTAMENTE");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                        break;
                        case 3:
                            Console.Clear();
                            string nemail = "";
                            string npassword = "";

                            Console.Write("Ingrese su email: ");
                            nemail=Console.ReadLine();
                            Console.Write("Ingrese su password: ");
                            npassword=Console.ReadLine();
                            var ingreso = new Ingreso()
                            {
                                email = nemail,
                                password = npassword
                            };

                            var mensaje = await httpClient.PostAsJsonAsync(url2, ingreso);
                            string requesttoken = await mensaje.Content.ReadAsStringAsync();
                            Token reg = JsonConvert.DeserializeObject<Token>(requesttoken);
                            string token = reg.token;
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                            Console.WriteLine("Token adquirido");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            break;

                        case 4:
                            var response2 = await httpClient.GetAsync(url3);

                            var content2 = await response2.Content.ReadAsStringAsync();
                            DevuelveToken myDeserializedClass2 = JsonConvert.DeserializeObject<DevuelveToken>(content2);
                            Console.WriteLine(myDeserializedClass2.email + " " + myDeserializedClass2.estado + " " + myDeserializedClass2.nombre + " " +
                                myDeserializedClass2.url);
                            capturaurl = myDeserializedClass2.url;
                            Console.WriteLine("Accediste correctamente");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            break;

                        case 5:
                            var urlarchivo = "https://candidates-exam.herokuapp.com/api/v1/usuarios/" + capturaurl + "/cargar_cv";
                            var rutaarchivo = @"C:\Users\DELL-PC\Desktop\CV\Curriculum_Justin_Zacarias .pdf";
                            var nombrearchivo = Path.GetFileName(rutaarchivo);

                            var requestContent = new MultipartFormDataContent();
                            var fileStream = File.OpenRead(rutaarchivo);
                            requestContent.Add(new StreamContent(fileStream), "curriculum", nombrearchivo);
                            await httpClient.PostAsync(urlarchivo, requestContent);
                            Console.WriteLine("CV cargado correctamente");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            break;

                        case 6:
                            var geturl = await httpClient.GetAsync(urlfinal);

                                var contentidourl = await geturl.Content.ReadAsStringAsync();
                                CvURL myDeserializedClass = JsonConvert.DeserializeObject<CvURL>(contentidourl);
                                Console.WriteLine(myDeserializedClass.url);
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            break;

                    case 0:
                        validate = true;
                        break;

                }
            } while (validate == false);
        }
        }

        static void Menu()
        {
            Console.WriteLine("1. GET Ping");
            Console.WriteLine("2. Registro de usuario");
            Console.WriteLine("3. Ingreso de Usuario/Obtencion Token");
            Console.WriteLine("4. GET datos de ingreso");
            Console.WriteLine("5. Carga CV");
            Console.WriteLine("6. Obtener URL CV");
            Console.WriteLine("0. Salir");
        }
    }
}
