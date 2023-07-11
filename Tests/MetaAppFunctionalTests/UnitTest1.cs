using System.Diagnostics;

namespace MetaAppFunctionalTests;

public class UnitTest1
{
    [Fact]
    public async Task GivenApplication_WhenStartFetchingWeatherData_ThenShouldFetchAndDisplayWeatherData()
    {
        Process application = null;

        try
        {
            // Arrange
            application = StartApplication();

            var expectedOutputs = new[] {
                "City: Vilnius",
                "Temperature: 5.1",
                "Description: Test",
                "City: Riga",
                "Temperature: 5.1",
                "Description: Test"};

            var mockConsoleOutput = new MockConsoleOutput();
            Console.SetOut(new StringWriter());

            // Act
            await SendCommandToApplication(application, $"weather --city Vilnius,Riga");

            // Allow some time for the weather data retrieval to complete
            Thread.Sleep(10000);

            Console.Out.Flush(); // Flush the console output

            // Assert
            var actualOutput = mockConsoleOutput.GetOutput();

            Console.WriteLine( actualOutput );
            //Assert.Equal(expectedOutputs.Length, actualOutputs.Length);

            //for (int i = 0; i < expectedOutputs.Length; i++)
            //{
            //    Assert.Contains(expectedOutputs[i], actualOutputs[i]);
            //}
        }
        finally
        {
            if (application != null)
            {
                StopApplication(application);
            }
        }
    }

    private async Task SendCommandToApplication(Process application, string command)
    {
        // Send the command to the application's standard input
        await application.StandardInput.WriteLineAsync(command);
    }

    private Process StartApplication()
    {
        // TODO: update to find path automatically.
        // Adjust the path to your application's executable
        var applicationPath = "C:\\_Build\\MetasiteDotnetTask\\Source\\Console\\bin\\Debug\\net7.0\\metaapp.exe";

        // Start the application process
        var startInfo = new ProcessStartInfo
        {
            FileName = applicationPath,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        var applicationProcess = new Process { StartInfo = startInfo };
        applicationProcess.Start();

        // Allow some time for the application to start up
        Thread.Sleep(2000);

        return applicationProcess;
    }

    private void StopApplication(Process applicationProcess)
    {
        applicationProcess.CloseMainWindow();
        applicationProcess.WaitForExit();
        applicationProcess.Dispose();
    }
}