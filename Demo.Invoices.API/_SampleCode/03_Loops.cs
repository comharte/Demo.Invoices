namespace Demo.Invoices.API.SampleCode;

public static class Loops
{
    private readonly static int[] _numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    public static void RunCode()
    {
        for (int i = 0; i < _numbers.Length; i++)
        {
            Console.WriteLine($"for loop: {_numbers[i]}");
        }

        foreach (var number in _numbers)
        {
            Console.WriteLine($"foreach loop: {number}");
        }

        int count = 0;
        while (count < _numbers.Length)
        {
            Console.WriteLine($"while loop: {_numbers[count]}");
            count++;
        }
    }
}
