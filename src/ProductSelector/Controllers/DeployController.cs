//using Microsoft.AspNetCore.Mvc;
//using PulumiInfra; // reference the PulumiInfra project
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;

//[Route("api/[controller]")]
//[ApiController]
//public class DeployController : ControllerBase
//{
//    private readonly PulumiAutomationService _pulumiService;

//    public DeployController()
//    {
//        // No constructor args needed anymore
//        _pulumiService = new PulumiAutomationService();
//    }

//    [HttpPost("{userId}/deploy")]
//    public async Task<IActionResult> Deploy(string userId, [FromBody] List<string> productIds)
//    {
//        // Map productIds (like "ProductA", "ProductB") → folder paths
//        var productPaths = new List<string>();
//        foreach (var id in productIds)
//        {
//            var path = Path.GetFullPath(
//                Path.Combine(Directory.GetCurrentDirectory(), $"../src/{id}")
//            );
//            productPaths.Add(path);
//        }

//        // Unique stack per user
//        var stackName = $"user-{userId}-stack";

//        var result = await _pulumiService.DeployForUserAsync(stackName, productPaths, userId);

//        // Collect outputs into a dictionary for response
//        var outputs = new Dictionary<string, object?>();
//        foreach (var kvp in result.Outputs)
//        {
//            outputs[kvp.Key] = kvp.Value.Value;
//        }

//        return Ok(new
//        {
//            message = "Deployment finished",
//            outputs
//        });
//    }
//}
