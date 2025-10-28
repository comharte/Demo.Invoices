namespace Demo.Invoices.API.Hosting.Security;

public class SecurityAuthenticationSchemes
{
    public const string AllConcatenated = Basic + "," + BearerAsymetric + "," + BearerSymetric;

    public static readonly string[] All = [Basic, BearerAsymetric, BearerSymetric];

    public const string Basic = "Basic";

    public const string BearerAsymetric = "BearerAsymetric";

    public const string BearerSymetric = "BearerSymetric";
}
