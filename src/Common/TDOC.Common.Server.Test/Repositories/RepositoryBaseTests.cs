using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using TDOC.EntityFramework.DbContext;

namespace TDOC.Common.Server.Test.Repositories;

[Collection(nameof(ParallelTestCollectionDefinition))]
public class RepositoryBaseTests
{
    private static bool firstRun = true;
    private const int sqlPort = 1433;
    private static string _connectionString;

    public static IConfiguration InitConfiguration() =>
        new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    private static string InitConnectionString()
    {
        if (!string.IsNullOrEmpty(_connectionString))
            return _connectionString;
        _connectionString = InitConfiguration().GetConnectionString("TDocContext");
        return _connectionString;
    }

    public RepositoryBaseTests()
    {
        if (firstRun)
        {
            firstRun = false;
            var builder = new SqlConnectionStringBuilder(InitConnectionString());
            string[] hostnameAndPort = builder.DataSource.Split(',');
            int port = hostnameAndPort.Length == 2 ? int.Parse(hostnameAndPort[1]) : sqlPort;

            // Remove Docker.
            Console.WriteLine("Removing Docker image");
            var processDockerRemoveStartInfo = new ProcessStartInfo()
            {
                Arguments = "rm roadrunner-database --force",
                FileName = "docker",
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            var processDockerRemove = Process.Start(processDockerRemoveStartInfo);
            processDockerRemove.WaitForExit();
            Console.WriteLine(processDockerRemove.StandardOutput.ReadToEnd());
            Console.WriteLine(processDockerRemove.StandardError.ReadToEnd());

            // Start Docker.
            Console.WriteLine("Building Docker image");
            var processDockerBuildStartInfo = new ProcessStartInfo()
            {
                Arguments = "build -t roadrunner-database -f Docker/Dockerfile .",
                FileName = "docker",
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            var processDockerBuild = Process.Start(processDockerBuildStartInfo);
            processDockerBuild.WaitForExit();
            Console.WriteLine(processDockerBuild.StandardOutput.ReadToEnd());
            Console.WriteLine(processDockerBuild.StandardError.ReadToEnd());

            Console.WriteLine("Running Docker image");
            var processDockerRunStartInfo = new ProcessStartInfo()
            {
                Arguments = $"run --name roadrunner-database -p {port}:{sqlPort} roadrunner-database",
                FileName = "docker",
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            var processDockerRun = Process.Start(processDockerRunStartInfo);
            while (!processDockerRun.StandardOutput.EndOfStream)
            {
                string line = processDockerRun.StandardOutput.ReadLine();
                Console.WriteLine(line);

                // Check database initialization is completed.
                if (line.Contains("setup.sql completed"))
                    break;
            }
        }
    }

    protected TDocEFDbContext ConfigureContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDocEFDbContext>();
        optionsBuilder.UseSqlServer(InitConnectionString(),
            optionsBuilder => { optionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); });
        return new TDocEFDbContext(optionsBuilder.Options);
    }
}