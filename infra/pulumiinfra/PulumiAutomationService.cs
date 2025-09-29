
using Pulumi;
using Pulumi.Automation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PulumiInfra
{
    public class PulumiAutomationService
    {
        private readonly string projectName = "ProductDeployer";

        public async Task<UpResult> DeployForUserAsync(string stackName, List<string> productPaths, string userId, List<UserProductPort> prodPorts, bool destroy = false)
        {
            var program = PulumiProgram.CreateProgram(productPaths, userId, prodPorts);

            var stackArgs = new InlineProgramArgs(projectName, stackName, program)
            {
                WorkDir = Directory.GetCurrentDirectory(),

                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["PULUMI_CONFIG_PASSPHRASE"] = "",
                    ["PULUMI_SECRETS_PROVIDER"] = "plaintext",
                    ["PULUMI_BACKEND_URL"] = "file://./" 
                }
            };

            var stack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);

            Console.WriteLine($"✅ Stack '{stackName}' ready. Deploying...");

            if (destroy)
            {
                Console.WriteLine("🗑 Destroying resources...");
                await stack.DestroyAsync(new DestroyOptions
                {
                    OnStandardOutput = Console.WriteLine,
                    OnStandardError = Console.Error.WriteLine
                });
                return null; // no UpResult for destroy
            }
            else
            {
                await stack.RefreshAsync(new RefreshOptions { OnStandardOutput = Console.WriteLine });

                var result = await stack.UpAsync(new UpOptions
                {
                    OnStandardOutput = Console.WriteLine,
                    OnStandardError = Console.Error.WriteLine
                });

                return result;
            }                
        }



        private static int GetFreePort()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
