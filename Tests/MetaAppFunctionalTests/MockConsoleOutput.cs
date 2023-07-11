namespace MetaAppFunctionalTests;

public interface IConsoleOutput
{
    void WriteLine(string value);
}

public class ConsoleOutput : IConsoleOutput
{
    public void WriteLine(string value)
    {
        Console.WriteLine(value);
    }
}

public class MockConsoleOutput : IConsoleOutput
{
    private readonly StringWriter _stringWriter;

    public MockConsoleOutput()
    {
        _stringWriter = new StringWriter();
    }

    public void WriteLine(string value)
    {
        _stringWriter.WriteLine(value);
    }

    public string GetOutput()
    {
        return _stringWriter.ToString();
    }
}
