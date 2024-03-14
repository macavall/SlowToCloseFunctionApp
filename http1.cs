using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SlowToClose
{
    public class http1
    {
        private readonly ILogger<http1> _logger;

        public http1(ILogger<http1> logger)
        {
            _logger = logger;
        }

        [Function("http1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string executablePath = @"deadlock.exe";

            // Create process start info
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = executablePath;

            // Optionally, you can set other properties of startInfo like arguments, working directory, etc.

            // Create and start the child process

            _ =Task.Factory.StartNew(() =>
            {
                using (Process? childProcess = Process.Start(startInfo))
                {
                    // Wait for the child process to exit
                    childProcess?.WaitForExit();

                    // Output exit code
                    Console.WriteLine("Child process exited with code: " + childProcess?.ExitCode);
                }
            });

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
