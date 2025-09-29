
using Pulumi;
using Pulumi.Automation;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PulumiInfra
{
    public class UserProductPort
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Port { get; set; }
        public string ProjectPath { get; set; }
    }
    

    public static class PulumiProgram
    {
        public static PulumiFn CreateProgram(List<string> productPaths, string userId, List<UserProductPort> prodPorts)
        {
            return PulumiFn.Create(() =>
            {
                var outputs = new Dictionary<string, object?>();

                string safeUser = userId
                    .Replace("@", "-")
                    .Replace(" ", "-")
                    .ToLowerInvariant();

                foreach (var path in productPaths)
                {
                    var productName = Path.GetFileName(Path.GetFullPath(path)).ToLowerInvariant();

                    var imageName = $"localhost/{userId.ToLower()}-{productName}:latest";

                    var image = new Image(imageName, new ImageArgs
                    {
                        Build = new DockerBuildArgs { Context = path },
                        ImageName = imageName,
                        SkipPush = true
                    });

                    var containerName = $"{safeUser}-{productName}-container";
                    var Externalport = prodPorts.Where(t => t.UserId.ToString() == userId && t.ProjectPath == path).Select(p => p.Port).FirstOrDefault();   // host port (random free port)
                    var container = new Container(containerName, new ContainerArgs
                    {
                        Image = image.ImageName,
                        Name = containerName,
                        Ports =
                        {
                            new Pulumi.Docker.Inputs.ContainerPortArgs
                            {
                                Internal = 80,
                                External = Convert.ToInt32(Externalport)
                            }
                        }
                    });
                    outputs[$"{productName}-containerName"] = container.Name;
                }
                return outputs;
            });
        }
    }
}