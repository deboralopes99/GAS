using GAS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace GAS.Controllers
{
    public class UtentesController : Controller
    {
        public async Task<IActionResult> Index()
        {
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
            var requestT = new HttpRequestMessage(HttpMethod.Get, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utentes_API/records");
            requestT.Headers.Add("Authorization", "Bearer " + token);
            var responseT = await clientT.SendAsync(requestT);
            responseT.EnsureSuccessStatusCode();

            string jsonStrT = responseT.Content.ReadAsStringAsync().Result;
            JObject jsonObj = JObject.Parse(jsonStrT);

            List<Utentes> Utentes = new List<Utentes>();

            foreach (var ut in jsonObj["response"]["data"])
            {
                Utentes utente = new Utentes();

                utente.Nome = ut["fieldData"]["UTE_nome"].ToString();
                utente.CodigoUtente = ut["fieldData"]["UTE_codUTE"].ToString();
                utente.ID = ut["fieldData"]["UTE_nrID"].ToString();
                utente.Data = ut["fieldData"]["UTE_data"].ToString();
                utente.Localidade = ut["fieldData"]["UTE_localidade"].ToString();
                utente.Telemovel = ut["fieldData"]["UTE_telemovel"].ToString();
                utente.NIF = ut["fieldData"]["UTE_nif"].ToString();
                utente.SNS = ut["fieldData"]["UTE_nr_SNS"].ToString();

                Utentes.Add(utente);

            }

            return View(Utentes);
        }
    }
}
