using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SlowToClose
{
    public class http1
    {
        static object lock1 = new object();
        static object lock2 = new object();
        private readonly ILogger<http1> _logger;

        public http1(ILogger<http1> logger)
        {
            _logger = logger;
        }

        [Function("http1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            _ =Task.Factory.StartNew(() =>
            {
                Thread thread1 = new Thread(Method1);
                Thread thread2 = new Thread(Method2);

                thread1.Start();
                thread2.Start();

                thread1.Join();
                thread2.Join();
            });

            return new OkObjectResult($"Welcome to Azure Functions! {GetThisProcessId()}");
        }

        static void Method1()
        {
            lock (lock1)
            {
                Console.WriteLine("Thread 1 acquired lock1.");
                Thread.Sleep(1000);

                lock (lock2)
                {
                    Console.WriteLine("Thread 1 acquired lock2.");
                }
            }
        }

        static string GetThisProcessId()
        {
            // Get the current process
            Process currentProcess = Process.GetCurrentProcess();

            // Get the process ID
            int processId = currentProcess.Id;

            // Convert process ID to string
            string processIdString = processId.ToString();

            return processIdString;
        }

        static void Method2()
        {
            lock (lock2)
            {
                Console.WriteLine("Thread 2 acquired lock2.");
                Thread.Sleep(1000);

                lock (lock1)
                {
                    Console.WriteLine("Thread 2 acquired lock1.");
                }
            }
        }
    }
}
