using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FoodDelivery.Services
{
    public class ChapaInitResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public ChapaInitData Data { get; set; }
    }

    public class ChapaInitData
    {
        [JsonProperty("checkout_url")]
        public string CheckoutUrl { get; set; }
    }

    public class ChapaVerifyResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public ChapaVerifyData Data { get; set; }
    }

    public class ChapaVerifyData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("tx_ref")]
        public string TxRef { get; set; }
    }

    public class ChapaService
    {
        private readonly string _secretKey;
        private readonly string _baseUrl;

        public ChapaService()
        {
            _secretKey = ConfigurationManager.AppSettings["Chapa:SecretKey"];
            _baseUrl = ConfigurationManager.AppSettings["Chapa:BaseUrl"];
        }

        public async Task<ChapaInitResponse> InitializePaymentAsync(
            string txRef, decimal amount, string email,
            string firstName, string lastName,
            string callbackUrl, string returnUrl)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _secretKey);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var payload = new
                {
                    amount = amount.ToString("F2"),
                    currency = "ETB",
                    email = email,
                    first_name = firstName,
                    last_name = lastName,
                    tx_ref = txRef,
                    callback_url = callbackUrl,
                    return_url = returnUrl
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/transaction/initialize", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ChapaInitResponse>(responseBody);
            }
        }

        public async Task<ChapaVerifyResponse> VerifyPaymentAsync(string txRef)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _secretKey);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"{_baseUrl}/transaction/verify/{txRef}");
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ChapaVerifyResponse>(responseBody);
            }
        }
    }
}
