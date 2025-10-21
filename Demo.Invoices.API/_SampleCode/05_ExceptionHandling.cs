namespace Demo.Invoices.API.SampleCode;

public static class ExceptionHandling
{
    public static void RunCode()
    {
        var examples = new ExceptionHandlingExamples();

        examples.ThrowAndHandle(ExceptionHandlingExamples.ExceptionType.ArgumentNull);
        examples.ThrowAndHandle(ExceptionHandlingExamples.ExceptionType.ArgumentOutOfRange);
        examples.ThrowAndHandle(ExceptionHandlingExamples.ExceptionType.InvalidOperation);
        examples.ThrowAndHandle(ExceptionHandlingExamples.ExceptionType.Custom);

        try
        {
            examples.ThrowAndRethrow();
        }
        catch
        {
            Console.WriteLine("RunCode| Caught ThrowAndRethrow");
        }

        try
        {
            examples.ThrowAndFinally();
        }
        catch
        {
            Console.WriteLine("RunCode| Caught ThrowAndFinally");
        }
    }
}

public class CustomException : Exception
{
    public CustomException() : base("Custom exception")
    {
    }
}

public class ExceptionHandlingExamples
{
    public enum ExceptionType
    {
        ArgumentNull,
        ArgumentOutOfRange,
        InvalidOperation,
        Custom
    }

    private Exception CreateException(ExceptionType type)
        => type switch
        {
            ExceptionType.ArgumentNull => new ArgumentNullException("param"),
            ExceptionType.ArgumentOutOfRange => new ArgumentOutOfRangeException("param", "out of range"),
            ExceptionType.InvalidOperation => new InvalidOperationException("invalid operation"),
            ExceptionType.Custom => new CustomException(),
            _ => new Exception("Unknown exception type")
        };

    public void ThrowAndHandle(ExceptionType type)
    {
        try
        {
            throw CreateException(type);
        }
        catch (CustomException ex)
        {
            Console.WriteLine($"ThrowNextAndHandle| CustomException: {ex.Message}");
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
        {
            Console.WriteLine($"ThrowNextAndHandle| Caught an argument exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ThrowNextAndHandle| General exception: {ex.Message}");
        }
    }

    public void ThrowAndRethrow()
    {
        try
        {
            throw new CustomException();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ThrowAndRethrow| Logging: " + ex.Message);
            throw; // Rethrow the original exception
        }
    }

    public void ThrowAndFinally()
    {
        try
        {
            throw new InvalidOperationException("invalid operation");
        }
        finally
        {
            Console.WriteLine("ThrowAndFinally| Disposing object.");
        }
    }
}
