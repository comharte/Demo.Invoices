using System.Collections.Immutable;

namespace Demo.Invoices.API.SampleCode;

public static class Collections
{
    public static void RunCode()
    {
        var example = new CollectionExamples();
    }
}


// Casting generic list 
public interface IBuilder;

public class Builders<T> 
    where T : IBuilder, new()
{
    public T CreateBuilder()
    {
        return new T();
    }
}

public class CollectionExamples
{
    private int[] _array; // Fixed size collection

    private ImmutableArray<int> _immutableArray; // Fixed size collection that cannot be changed after creation

    private List<int> _list; // Dynamic size collection

    private ImmutableList<int> _immutableList; // Fixed size collection that cannot be changed after creation

    private HashSet<int> _hashSet; // Indexed collection of unique items (faster reads, slower writes)

    private ImmutableHashSet<int> _immutablehashSet; // Fixed size collection of unique items that cannot be changed after creation

    private Dictionary<int, string> _dictionary; // Indexed collection of key-value pairs

    private ImmutableDictionary<int, string> _immutableDictionary; // Fixed size collection of key-value pairs that cannot be changed after creation

    private Queue<int> _queue;

    public CollectionExamples()
    {
        //Initialization examples
        //Diffrent syntaxt, same compiler output
        _array = new int[5];
        _array[0] = 1;
        _array = new int[] { 1, 2, 3, 4 };
        _array = new[] { 1, 2, 3, 4, 5 }; // type is inferred by compiler
        _array = [1, 2, 3, 4, 5,]; // promoted in C#
        Array.Resize(ref _array, 10); // resize array (creates new array and copies old values)
        _array[8] = 10; // set value at index 8
        _array = Enumerable.Range(1, 10).ToArray(); // create array with numbers from 1 to 10

        _immutableArray = ImmutableArray.Create<int>();
        _immutableArray = ImmutableArray.Create<int>(1, 2, 3);
        _immutableArray = ImmutableArray.CreateRange(Enumerable.Range(1, 10)); // create immutable array with numbers from 1 to 10
        _immutableArray = [1, 2, 3, 4, 5]; // promoted in C#
        _immutableArray = _array.ToImmutableArray(); // convert array to immutable array

        _list = new List<int>();
        _list = new();
        _list = []; // promoted in C#
        _list = new List<int> { 1, 2, 3 };
        _list = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]; // promoted in C#
        _list = _array.ToList(); // convert array to list
        _list.Add(11); // add value to list
        _list.Remove(5); // remove value from list
        _list.Insert(0, 0); // insert value at index 0
        _list.Clear(); // remove all values from list
        _list.Sort(); // sort list
        _list.Reverse(); // reverse list
        _list.Contains(3); // check if list contains value
        _list = Enumerable.Range(1, 10).ToList(); // create list with numbers from 1 to 10
        _list.Add(11);

        _immutableList = ImmutableList.Create<int>();
        _immutableList = ImmutableList.Create<int>(1, 2, 3);
        _immutableList = ImmutableList.CreateRange(Enumerable.Range(1, 10)); // create immutable list with numbers from 1 to 10
        _immutableList = [1, 2, 3, 4, 5]; // promoted in C#
        _immutableList = _array.ToImmutableList(); // convert array to immutable list


        _hashSet = new HashSet<int>();
        _hashSet = new();
        _hashSet = new HashSet<int> { 1, 2, 3 };
        _hashSet = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]; // promoted in C#
        _hashSet = _array.ToHashSet(); // convert array to hashset
        _hashSet = Enumerable.Range(1, 10).ToHashSet(); // create hashset with numbers from 1 to 10
        var added = _hashSet.Add(11); //true
        added = _hashSet.Add(11); //false

        _immutablehashSet = ImmutableHashSet.Create<int>();
        _immutablehashSet = ImmutableHashSet.Create<int>(1, 2, 3);
        _immutablehashSet = ImmutableHashSet.CreateRange(Enumerable.Range(1, 10)); // create immutable hashset with numbers from 1 to 10
        _immutablehashSet = [1, 2, 3, 4, 5]; // promoted in C#

        _dictionary = new Dictionary<int, string>();
        _dictionary = new();
        _dictionary = new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } };
        _dictionary = new() { { 1, "One" }, { 2, "Two" } };
        _dictionary = _list.ToDictionary(i => i, i => $"Number: {i}"); // convert list to dictionary with key and value the same

        //iteration examples
        foreach (var item in _dictionary)
        {
            int k = item.Key;
            string v = item.Value;
        }

        foreach (var key in _dictionary.Keys)
        {
            var value = _dictionary[key];
            int k = key;
            string v = value;
        }

        foreach ((var key, var value) in _dictionary)
        {
            int k = key;
            string v = value;
        }


        _immutableDictionary = ImmutableDictionary.Create<int, string>();
        _immutableDictionary = ImmutableDictionary.CreateRange(new Dictionary<int, string> { { 1, "One" }, { 2, "Two" } });
        _immutableDictionary = _dictionary.ToImmutableDictionary(); // convert dictionary to immutable dictionary

        _dictionary.Remove(11);
        _dictionary.Add(11, "ElevenXXX"); // add key-value pair, throws exception if key already exists
        _dictionary[11] = "Eleven"; // add key-value pair, overwrites value if key already exists

        _queue = new Queue<int>();
        _queue = new();
        _queue = new Queue<int>([1, 2, 3, 4, 5]);
        _queue = new([1, 2, 3, 4, 5]); // promoted in C#
        _queue.Enqueue(6); // add value to end of queue
        var dequeued = _queue.Dequeue(); // remove and return value from start of queue

        PlayWithQuerying(_array);
        PlayWithQuerying(_immutableArray);
        PlayWithQuerying(_list);
        PlayWithQuerying(_immutableList);
        PlayWithQuerying(_hashSet);
        PlayWithQuerying(_immutablehashSet);
        PlayWithQuerying(_dictionary.Keys);
        PlayWithQuerying(_immutableDictionary.Keys);
        PlayWithQuerying(_queue);
    }

    // LINQ - Language-Integrated Query
    // .NET framework for querying collections
    private void PlayWithQuerying(IEnumerable<int> numbers)
    {
        //First
        var first = numbers.First(); // throws exception if collection is empty
        var firstOrDefault = numbers.FirstOrDefault(); // returns default value if collection is empty (e.g., 0 for int, null for reference types)
        var firstWithMatchingLambdaCondition = numbers.First(n => n % 2 == 0); // throws exception if no element matches condition
        var firstWithMatchingMethodCondition = numbers.First(IsEven); // throws exception if no element matches condition

        //Single
        try
        {
            var single = numbers.Single(); // throws exception if collection does not contain exactly one element
        }
        catch { }

        try
        {
            var singleOrDefault = numbers.SingleOrDefault(); // returns default value if collection does not contain exactly one element
        }
        catch { }

        var singleWithMatchingLambdaCondition = numbers.Single(n => n == 5); // throws exception if no element matches condition or more than one element matches condition
        var singleWithMatchingMethodCondition = numbers.Single(IsFive); // throws exception if no element matches condition or more than one element matches condition

        //Where
        var whereWithMatchingLambdaCondition = numbers.Where(n => n % 2 == 0); // filter even numbers
        var whereWithMatchingMethodCondition = numbers.Where(IsEven); // filter even numbers

        var orderedAsc = numbers.OrderBy(n => n); // order ascending
        var orderedDesc = numbers.OrderByDescending(n => n); // order descending
        var orderByEven = numbers.OrderBy(n => n % 2); // 

        var max = numbers.Max(); // get maximum value
        var sum = numbers.Sum(); // get sum of values
        var average = numbers.Average(); // get average of values

        //Chaining
        var maxByOrder = numbers
            .Where(IsEven)
            .OrderBy(n => n)
            .First();

        var filtered = numbers.Where(IsEven);
        var ordered = filtered.OrderBy(n => n);
        var firstItem = ordered.First();

        var maxByMax = numbers
            .Where(IsEven)
            .Max();

        //Projections
        var users = numbers.Select(n => new User(n)); // project numbers to users

        var userNames = users.Where(u => IsEven(u.Id))
            .Select(u => u.Name); // project users with evenIds to names
    }

    public class User
    {
        public User(int id)
        {
            Id = id;
            Name = Guid.NewGuid().ToString();
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private bool IsEven(int number) => number % 2 == 0;

    private bool IsFive(int number) => number == 5;

}