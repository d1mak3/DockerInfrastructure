using System.Net.Http;

namespace Client
{
    public static class Program
    {
        private static async Task PostThread()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                
            };

            HttpContent content = new FormUrlEncodedContent(values);

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://localhost:5000/api/producer?sender=dima&content=hello", content);
            
        }

        private static async Task ExecuteKafkaConsumer()
        {            
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://localhost:5000/api/consumer");

        }

        private async static Task Main()
        {
            await ExecuteKafkaConsumer();
            //await PostThread();
        }        
    }
}