namespace Demo.Invoices.API.SampleCode;

public class MemoryManagementAndDisposal
{
    //Advence disposal patterns: https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    public class UnmanagedResourceHolder
        : IDisposable //Provides a mechanism for releasing unmanaged resources.
    {
        public void Dispose()
        {
            // Clean up unmanaged resources here
        }
    }

    public static void RunCode()
    {
        var resourceHolder = new UnmanagedResourceHolder();
        resourceHolder.Dispose();

        //Better approach is to use "using" statement which ensures Dispose is called even if exception is thrown
        using (var anotherResourceHolder = new UnmanagedResourceHolder())
        {
            // Use the resource
        } // Dispose is called automatically here

        using var thirdResourceHolder = new UnmanagedResourceHolder(); // This one will be disposed at the end of the scope
    }
}