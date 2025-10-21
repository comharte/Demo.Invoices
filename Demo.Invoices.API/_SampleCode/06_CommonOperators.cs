namespace Demo.Invoices.API.SampleCode;

public static class CommonOperators
{
    public static void RunCode()
    {
        var emailClass1 = new EmailClass { Address = "john.doe", Domain = "example.com" };
        var emailClass2 = new EmailClass { Address = "john.doe", Domain = "example.com" };
        var areClassesSame = emailClass1 == emailClass2; // false, reference equality
        var areClassesEqual = emailClass1.Equals(emailClass2); // false, reference equality

        var emailRecord1 = new EmailRecord { Address = "jane.doe", Domain = "example.com" };
        var emailRecord2 = new EmailRecord { Address = "jane.doe", Domain = "example.com" };
        var areRecordsSame = emailRecord1 == emailRecord2; // true, value equality
        var areRecordsEqual = emailRecord1.Equals(emailRecord2); // true, value equality

        var emailMaster1 = new EmailMasterClass { Address = "jim.beam", Domain = "example.com" };
        var emailMaster2 = new EmailMasterClass { Address = "jim.beam", Domain = "example.com" };
        var areMasterClassesSame = emailMaster1 == emailMaster2; // false, reference equality
        var areMasterClassesEqual = emailMaster1.Equals(emailMaster2); // true, overridden to value equality

        var emailAddress = "alice.smith@example.com";

        EmailSuperMasterClass emailSuperMaster1 = emailAddress; // implicit conversion from string to EmailSuperMasterClass
        EmailSuperMasterClass emailSuperMaster2 = emailAddress; // implicit conversion from string to EmailSuperMasterClass
        var areSuperMasterClassesSame = emailSuperMaster1 == emailSuperMaster2; // true, overridden to value equality
        var areSuperMasterClassesEqual = emailSuperMaster1.Equals(emailSuperMaster2); // true, overridden to value equality

    
        var className = nameof(EmailSuperMasterClass); // "EmailSuperMasterClass"
        var classType = typeof(EmailSuperMasterClass); // gets the Type object for EmailSuperMasterClass
        var classFullyQualifiedName = classType.FullName; // "DemoAPI.SampleCode.EmailSuperMasterClass"
    }
}


public class EmailClass
{
    public string Address { get; init; } = null!; //Project has <enable> set to true. This means non-nullable reference types are enabled. null! is used to suppress the warning.
    public string Domain { get; init; } = null!;
}

public record EmailRecord
{
    public string Address { get; init; } = null!;
    public string Domain { get; init; } = null!;
}

public class EmailMasterClass : EmailClass
{
    public override bool Equals(object? obj)
        => obj is EmailMasterClass other && // 'is' operator verifies type match and performs safe cast to 'other'. It also handles null check.
           Address.Equals(other.Address) &&
           Domain.Equals(other.Address);

    public override int GetHashCode()
        => HashCode.Combine(Address, Domain);
}

public class EmailSuperMasterClass : EmailMasterClass
{
    public static implicit operator EmailSuperMasterClass(string email) // implicit cast
        => FromString(email);

    public static implicit operator string(EmailSuperMasterClass email)
        => ToString(email);

    public static bool operator ==(EmailSuperMasterClass left, EmailSuperMasterClass right) // operator overload
        => left.Equals(right);

    public static bool operator !=(EmailSuperMasterClass left, EmailSuperMasterClass right)
        => !left.Equals(right);

    private static EmailSuperMasterClass FromString(string email)
        => new EmailSuperMasterClass()
        {
            Address = email.Split('@')[0],
            Domain = email.Split('@')[1]
        };

    private static string ToString(EmailSuperMasterClass email)
        => $"{email.Address}@{email.Domain}";

    public override string ToString() // override base 'object' method
        => ToString(this);

    public override bool Equals(object? obj) // override base 'object' method
        => base.Equals(obj);

    public override int GetHashCode() // override base 'object' method. Should be overridden when Equals is overridden.
        => base.GetHashCode();
}