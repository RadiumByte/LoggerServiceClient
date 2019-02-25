using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace LoggerServiceClient
{
    public class Log
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }
        public string SenderApp { get; set; }
        public string BInfo { get; set; }
        public string DInfo { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();

        static void ShowLog(Log log)
        {
            Console.WriteLine($"Date: {log.Date}\tType: " + $"{log.Type}\tUsername: {log.UserName}" +
                $"SenderApp: {log.SenderApp}\t");
        }

        static async Task<Uri> CreateLogAsync(Log log)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/log", log);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<Log> GetLogAsync(string path)
        {
            Log product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Log>();
            }
            return product;
        }

        static void Main()
        {

            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://localhost:5001/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Token", "199719741969");

            try
            {
                // Create a new product
                Log log = new Log
                {
                    Date = "2019-02-22 17:02:50",
                    Type = "telemetry_msg",
                    UserName = "Test",
                    SenderApp = "Autopilot v1.2",
                    BInfo = "Movement data",
                    DInfo = "FORWARD 20, LEFT 10, RIGHT 20"
                };

                for (long i = 0; i < 10000000; i++)
                {
                    log.DInfo = i.ToString();
                    var url = await CreateLogAsync(log);
                    Console.WriteLine($"Created {i} at {url}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
