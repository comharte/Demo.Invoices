namespace Demo.Invoices.API.SampleCode;

public static class SampleCodeExecution
{
    public static void Run()
    {
        AddLogSeperation("TypesFieldsProperties");
        TypesFieldsProperties.RunCode();

        AddLogSeperation("MethodsDelegates");
        MethodsDelegates.RunCode();

        AddLogSeperation("Loops");
        Loops.RunCode();

        AddLogSeperation("InheritanceAndOverriding");
        InheritanceAndOverriding.RunCode();

        AddLogSeperation("ExceptionHandling");
        ExceptionHandling.RunCode();

        AddLogSeperation("CommonOperators");
        CommonOperators.RunCode();

        AddLogSeperation("Collections");
        Collections.RunCode();

        AddLogSeperation("MemoryManagementAndDisposal");
        MemoryManagementAndDisposal.RunCode();

        AddLogSeperation("Asynchronous");
        Asynchronous.RunCode();

        AddLogSeperation("SynchronizationPrimitives");
        SynchronizationPrimitives.RunCode();
    }

    private static void AddLogSeperation(string sectionName)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"{new string('-', 10)} {sectionName}");
        Console.WriteLine(new string('=', 70));
    }
}
