using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SlowToClose
{
    public class thread
    {
        private readonly ILogger<thread> _logger;

        public thread(ILogger<thread> logger)
        {
            _logger = logger;
        }

        [Function("thread")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Thread thread = new Thread(ThreadMethod);
                    thread.Start(i); // Pass some parameter if needed
                }
            });

            return new OkObjectResult("Welcome to Azure Functions!");
        }

        static void ThreadMethod(object obj)
        {
            int threadNumber = (int)obj;
            Console.WriteLine($"Thread {threadNumber} started.");

            Thread.Sleep(60000);
            // Implement your thread logic here
        }
    }
}
