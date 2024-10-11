using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentGateway.Models;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;


[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [HttpPost]
    public async Task<object> Post(string start_date, string end_date)
    {
        const SecurityProtocolType tls13 = (SecurityProtocolType)12288;
        ServicePointManager.SecurityProtocol = tls13 | SecurityProtocolType.Tls12;

        TokenJson Token = new TokenJson();
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en_US"));

            var clientId = "Ac10XGW4xw6tTKtoixcVNGxYwAsWNCCD41Ko2GJd3NxRelQ4_ANvqWyKe-N53x7YRG_6SuKo6_6u5SGl";
            var clientSecret = "EBx2lLOI0hj2GMC-29VVnHLYpNGmYKrpz181uOfc_8RmL7_AHkxPkP7ThC29Z9LDa_vcPzSBfkxtq8Sz";
            var bytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            var responseMessage = await client.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", new FormUrlEncodedContent(keyValues));
            var response = await responseMessage.Content.ReadAsStringAsync();
            Token = JsonConvert.DeserializeObject<TokenJson>(response);
        }
        if(Token != null)
        {
            var transcationHistoryUrl = "https://api-m.sandbox.paypal.com/v1/reporting/transactions?start_date=2014-07-12T00:00:00-0700&end_date=2014-07-12T23:59:59-0700&transaction_id=9GS80322P28628837&fields=all";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en_US"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.access_token);
                var responseMessage = await client.GetAsync(transcationHistoryUrl);
                var response = await responseMessage.Content.ReadAsStringAsync();
                var Transaction = JsonConvert.DeserializeObject(response);
                return Transaction;
            }
        }
        return "Please try again something getting problem";
    }
}



