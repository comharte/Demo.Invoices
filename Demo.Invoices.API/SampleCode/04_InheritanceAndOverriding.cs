namespace Demo.Invoices.API.SampleCode;

public class InheritanceAndOverriding
{
    public static void RunCode()
    {
        var employee = new Employee
        {
            DisplayName = "John Doe",
            Id = 1,
            BaseSalary = 50000m
        };
        var manager = new Manager
        {
            DisplayName = "Jane Smith",
            Id = 2,
            BaseSalary = 80000m,
            SuccessFeeBonus = 15000m
        };

        // Polymorphism: Employee reference can point to a Manager object
        Employee polymorphicManager = manager;


        Console.WriteLine($"Employee: {employee.DisplayName}, Salary: {employee.CalculateSalary()}");
        //Employee: John Doe, Salary: 55000.0

        Console.WriteLine($"Manager: {manager.DisplayName}, Salary: {manager.CalculateSalary()}");
        //Manager: Boss: Jane Smith, Salary: 103000.0

        Console.WriteLine($"Polymorphic Manager: {polymorphicManager.DisplayName}, Salary: {polymorphicManager.CalculateSalary()}");
        //Polymorphic Manager: Jane Smith, Salary: 103000.0
        // Note: DisplayName does not show "Boss:" prefix here because the property is hidden, not overridden.
    }
}

public abstract class Person
{
    public string DisplayName { get; set; }

    public abstract decimal CalculateSalary();

    public void Introduce()
    {
        Console.WriteLine($"Hello, my name is {DisplayName}.");
    }
}

public class Employee : Person
{
    public int Id { get; set; }

    public decimal BaseSalary { get; set; }

    // Virtual method that can be overridden in derived classes
    public virtual decimal CalculateBonus()
        => BaseSalary * 0.1m;

    public override decimal CalculateSalary()
        => BaseSalary + CalculateBonus();
}

public class Manager : Employee
{
    // Hides the DisplayName property from Person class

    public new string DisplayName { get => $"Boss: {base.DisplayName}"; set => base.DisplayName = value; }

    public decimal SuccessFeeBonus { get; set; }

    // Override the CalculateSalary method to include bonus
    public override decimal CalculateBonus()
        => base.CalculateBonus() + SuccessFeeBonus;
}