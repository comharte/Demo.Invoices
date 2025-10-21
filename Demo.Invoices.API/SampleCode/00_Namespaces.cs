// defines that all types from System namespace are available in whole project without using fullyqualified names
// no need to add "using System;" in each file
// useful in contained projects (e.g., Frameworks)
global using System; 

// defines that all types from System.Collections.Generic namespace are available in current file without using fullyqualified names
using System.Collections.Generic;

// defines file namespace
namespace Demo.Invoices.API.SampleCode;

/// <summary>
/// This summary will show up in IntelliSense as documentation for the Matrix class
/// </summary>
public class Matrix
{
    public void BestNumber()
    {        
        //Does not require any using statements as we access Math class with fully qualified name
        double pi1 = System.Math.PI;
        
        //Requires using statement for System namespace
        double pi2 = Math.PI;
    }
}