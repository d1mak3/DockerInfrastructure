using System.Net;
using System.Net.Http;
using System.Text;

namespace Client
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
    }

    public static class Program
    {
        private static int countOfThreadsFinished = 0;       

        static async Task ExecutePostRequest()
        {      
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new Message() { Id = 0, Content = "Hello world!", Sender = "Dima" });
            var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:8000/api/producer";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(countOfThreadsFinished);
            countOfThreadsFinished += 1;
        }

        static async Task ExecuteGetRequest()
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://localhost:8000/api/producer");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            countOfThreadsFinished += 1;
        }

        public async static Task Main(string[] args)
        {
            for (int i = 0; i < 1000; ++i)
            {
                //await ExecuteGetRequest();
                await ExecutePostRequest();
            }
        }
    }
}