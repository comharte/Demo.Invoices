namespace Demo.Invoices.API.SampleCode;

// This class demonstrates init field behavior and generic attribute usage
public static class TypesFieldsProperties
{
    public static void RunCode()
    {
        var blueprint1 = new Blueprint();
        //blueprint1.InitializedProperty = true; // Throws compile-time error 

        //{} is object initializer syntax
        //allows setting init properties at the time of object creation
        //allows to set propertes which are not part of constructor in single elegatnt statement
        var blueprint2 = new Blueprint
        {
            InitializedProperty = true,
            PropertyWithBackingField = new Random().Next(1, 100)
        };

        var bluePrintWithGeneric = new Blueprint<string>
        {
            GenericProperty = "Hello Generic"
        };

        var bluePrintWithGeneric2 = new Blueprint<int>
        {
            GenericProperty = 123
        };
    }
}


//Enum examples with imilicitly assigned values 
public enum BlueprintState
{
    Draft = 1,
    Review = 2,
    Completed = 3,
    Unknown = 4,
}

public enum BlueprintDisplayMode
{
    Default,
    Dark
}

//Standard class definition
public class Blueprint
{
    public Blueprint()
        : this("Hello World")
    {
        StringProperty = "asd";
    }

    public Blueprint(string stringPropertyValue)
    {
        //Most .NET base type define a static Parse method
        _field = int.Parse("123");

        //Most .NET base type define a TryParse method
        if (Guid.TryParse("invalid_guid", out Guid parsedGuid))
        {
            _assignableOnlyInConstructorField = parsedGuid;
        }
        else
        {
            _assignableOnlyInConstructorField = Guid.Empty;
        }

        //Above could be simplified with ternary operator
        _assignableOnlyInConstructorField = Guid.TryParse("invalid_guid", out parsedGuid)
            ? parsedGuid
            : Guid.NewGuid(); //Let's use random instead of empty

        //Standard switch statement
        switch (_field % 10)
        {
            case 1:
                _state = BlueprintState.Draft;
                break;
            case 2:
                _state = BlueprintState.Review;
                break;
            case 3:
                _state = BlueprintState.Completed;
                break;
            default:
                _state = BlueprintState.Unknown;
                break;
        }


        //simplified switch expression
        _state = (_field % 10) switch
        {
            0 => BlueprintState.Draft,
            1 => BlueprintState.Review,
            2 => BlueprintState.Completed,
            _ => BlueprintState.Unknown,
        };

        DateTimeProperty = new DateTime(2025, 10, 11);

        TimeSpan additionalTime = TimeSpan.FromHours(1);
        additionalTime += TimeSpan.FromMinutes(30); //add 30 minutes

        DateTimeProperty.Add(additionalTime);

        StringProperty = stringPropertyValue;
        StringProperty = string.Empty;
        StringProperty = Guid.NewGuid().ToString();
        StringProperty = "Hello" + " " + "World"; //String concatenation
        StringProperty = $"Current Time: {DateTime.Now}"; //String interpolation

        //var allows to define type implicitly
        //compiler will infer the type of the variable from the initialization expression
        var stringFormat = "{0} {1}";

        StringProperty = string.Format(stringFormat, " Hello", "World ");
        StringProperty = StringProperty.ToUpper(); //make it big
        StringProperty = StringProperty.ToLower(); //make it small
        StringProperty = StringProperty.Trim(); //remove spaces at start and end
        StringProperty = StringProperty.Replace("Hello", "Hi"); //replace Hello with Hi
        StringProperty = StringProperty.Substring(0, 2); //get first 2 characters
        var helloIndex = StringProperty.IndexOf("Hello"); //find index of Hello
        var containsWorld = StringProperty.Contains("World"); //check if contains World
        var length = StringProperty.Length; //get length of string
        var isNullOrEmpty = string.IsNullOrEmpty(StringProperty); //check if string is null or empty
    }

    private readonly BlueprintState _state;

    private const BlueprintDisplayMode Mode = BlueprintDisplayMode.Default;

    private const double Pi = 3.14159; //constant field

    private const string Message = "Hello World"; //constant field

    private readonly Guid _assignableOnlyInConstructorField;

    private int _field;

    public string StringProperty { get; set; }

    public DateTime DateTimeProperty { get; set; }

    public int PropertyWithBackingField
    {
        get => _field;
        set => _field = value;
    }

    public int PropertyWithBackingFieldLogic
    {
        get
        {
            //You can add logic to getter
            return _field;
        }
        set
        {
            if (value < 0)
            {
                _field = 0;
            }
            _field = value;
        }
    }

    public bool InitializedProperty { get; init; }

    public Guid ReadOnlyProperty => _assignableOnlyInConstructorField;
}

//Example of generic class
//Can be defined for both class and method
public class Blueprint<T> : Blueprint
{
    public T GenericProperty { get; set; }
}