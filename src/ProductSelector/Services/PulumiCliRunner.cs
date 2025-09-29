using System.Diagnostics;

public class PulumiCliRunner
{
    private string infraDir = Path.Combine("..", "..", "..", "..", "infra", "ProductHost");

    private ProcessResult Run(string args)
    {
        var psi = new ProcessStartInfo("pulumi", args)
        {
            WorkingDirectory = infraDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var p = Process.Start(psi)!;
        string output = p.StandardOutput.ReadToEnd();
        string error = p.StandardError.ReadToEnd();
        p.WaitForExit();
        return new ProcessResult(p.ExitCode, output, error);
    }

    public void Deploy(string stackName, string userSelectionsJson)
    {
        Run($"stack select {stackName} || pulumi stack init {stackName}");
        Run($"config set producthost:userSelections \"{userSelectionsJson}\" --stack {stackName}");
        Run($"up --yes --stack {stackName}");
    }
}

public record ProcessResult(int ExitCode, string StdOut, string StdErr);
