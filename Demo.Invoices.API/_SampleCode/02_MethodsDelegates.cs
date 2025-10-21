namespace Demo.Invoices.API.SampleCode;
public static class MethodsDelegates
{
    public static void RunCode()
    {
        (var greeting, var entity) = ReturnMultipleValues();

        var printer1 = new MessagePrinter(useTimestamp: true, printToConsole: true);
        printer1.Print($"{greeting} {entity} with timestamp!");

        var printer2 = new MessagePrinter(useTimestamp: false, printToConsole: false);
        printer2.Print("This message will not be printed.");

        var lambdaPrinter = new MessagePrinter();
        lambdaPrinter.Print($"{greeting} {entity} from lambda!");
    }

    public static (string, string) ReturnMultipleValues()
    {
        return ("Hello", "World");
    }
}


//Extension method example
public static class MessagePrinterExtensionMethods
{
    //Extension members enable you to "add" methods to existing types without creating a new derived type, recompiling, or otherwise modifying the original type.
    //Can be declared for both value and reference types
    //Must be declared in static class
    //First parameter must be preceded by 'this' keyword and specifies the type the method extends
    //Can access public members only
    public static string AddMessagePrefix(this string message, string prefix)
    {
        return $"{prefix}: {message}";
    }

    public static string WithTimestamp(this string message)
    {
        return $"[{DateTime.Now}] {message}";
    }
}

public class MessagePrinter
{
    //Func is delegate that takes parameters and returns a value
    //Field allows to store reference to any method that matches the signature (string -> string)
    private Func<string, string> _messageBuilder;

    //Action is delegate that takes parameters and does not return a value
    //Field allows to store reference to any method that matches the signature (string -> void)
    private Action<string> _messagePrinter;

    public MessagePrinter(bool useTimestamp, bool printToConsole)
    {
        //Assign appropriate methods to delegates based on constructor parameters
        _messageBuilder = useTimestamp
            ? BuildMessageWithTimestamp
            : BuildMessage;

        _messagePrinter = printToConsole
            ? PrintMessageToConsole
            : PrintMessageToVoid;
    }

    // Lambda expression examples
    public MessagePrinter()
    {
        _messageBuilder = (message) => $"Lambda action: {message}";
        _messagePrinter = (message) => Console.WriteLine($"Lambda print: {message}");
    }

    public void Print(string message)
    {
        //Use the delegates to build and print the message
        string builtMessage = _messageBuilder(message);

        var isValid = ValidateMessage(builtMessage)
            && ValidateMessage(builtMessage, true)
            && ValidateMessage(builtMessage, 10);

        _messagePrinter(builtMessage);
    }

    // Example of method with optional parameter
    private bool ValidateMessage(string message, bool throwIfInvalid = false)
    {
        var isValid = !string.IsNullOrEmpty(message);

        if (!isValid && throwIfInvalid)
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        return isValid;
    }

    // Example of method overloading
    private bool ValidateMessage(string message, int minLength)
    {
        return message.Length >= minLength;
    }

    private string BuildMessage(string message)
        => message.AddMessagePrefix("Message"); // Expression-bodied method (It is just diffrent syntaxt)

    private string BuildMessageWithTimestamp(string message)
        => message.AddMessagePrefix("Message").WithTimestamp();

    private void PrintMessageToConsole(string message)
    {
        Console.WriteLine(message);
    }

    private void PrintMessageToVoid(string message)
    { }
}