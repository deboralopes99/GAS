using GAS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace GAS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {

            return View();

        }

        public async Task<IActionResult> LogIn()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> LogIn(User user)
        {
            bool isUserAuthenticated = false;

            //obter token
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/sessions");
            request.Headers.Add("Authorization", "Basic d2ViOiNwYXJhZGlzZUA=");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            // Lê o conteúdo da resposta como uma string JSON
            string jsonStr = response.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse(jsonStr);
            string token = json["response"]["token"].ToString();
            GetToken model = new GetToken { Token = token };

            //obter tabelas
            var clientT = new HttpClient();
            var requestT = new HttpRequestMessage(HttpMethod.Get, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utilizadores_API/records");
            requestT.Headers.Add("Authorization", "Bearer " + token);
            var responseT = await clientT.SendAsync(requestT);
            responseT.EnsureSuccessStatusCode();

            string jsonStrT = responseT.Content.ReadAsStringAsync().Result;
            JObject jsonObj = JObject.Parse(jsonStrT);

            foreach (var utilizador in jsonObj["response"]["data"])
            {
                string name = utilizador["fieldData"]["UTL_nome"].ToString();
                string pass = utilizador["fieldData"]["UTL_senha"].ToString();

                if (name == user.Username && pass == user.Password)
                {
                    isUserAuthenticated = true;
                    break;
                }

            }

            if (isUserAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(user);
            }


            // Check if the username and password are valid
            /*if (user.Username == "admin" && user.Password == "password")
            {
                // Set an authentication cookie and redirect to the home page
                //FormsAuthentication.SetAuthCookie(user.Username, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Display an error message
                ModelState.AddModelError("", "Invalid username or password");
                return View(user);
            }*/
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}