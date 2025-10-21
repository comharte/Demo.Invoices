using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;


namespace Demo.Invoices.API.SampleCode;

//https://learn.microsoft.com/en-us/dotnet/standard/threading/overview-of-synchronization-primitives
public class SynchronizationPrimitives
{
    public class RaceStarter
    {
        public bool Started { get; set; }
    }

    public class Invoice;

    public class WarehouseDocument;

    public static void RunCode()
    {
        var uniqueIdentifiers = new ConcurrentBag<UniqueIdentifier>();

        var raceTasks = Enumerable.Range(0, 10).Select(_ => CreateUniqueIdentifierTask(uniqueIdentifiers)).ToImmutableArray();

        Parallel.ForEach(raceTasks, task => task.Start());

        Task.WaitAll(raceTasks);

        foreach (var uniqueIdentifier in uniqueIdentifiers.OrderBy(ui => ui.UniqueCode))
        {
            Console.WriteLine($"UniqueIdentifier| Id: {uniqueIdentifier.Id}, UniqueCode: {uniqueIdentifier.UniqueCode}, GlobalId: {uniqueIdentifier.GlobalId}");
        }
    }

    private static Task CreateUniqueIdentifierTask(ConcurrentBag<UniqueIdentifier> ids)
        => new(() =>
            {
                ids.Add(UniqueIdentifierService.Instance.CreateNext<Invoice>());
                ids.Add(UniqueIdentifierService.Instance.CreateNext<WarehouseDocument>());
            });
}

public record UniqueIdentifier(string Id, string UniqueCode, Guid GlobalId);


public class UniqueIdGenerator
{
    private int currentId = 0;

    public int GetNextId()
        => Interlocked.Increment(ref currentId); // example of thread-safe increment
}

public class UniqueIdentifierGenerator
{
    public static implicit operator UniqueIdentifierGenerator(string typeName)
        => new(typeName);

    private readonly UniqueIdGenerator _idGenerator = new();

    private readonly string _typeName;

    public UniqueIdentifierGenerator(string typeName)
    {
        _typeName = typeName;
    }

    public UniqueIdentifier CreateNext()
    {
        var id = _idGenerator.GetNextId();
        return new UniqueIdentifier($"{id:00}", $"{_typeName}-{id:00000}", Guid.NewGuid());
    }
}

//This is example of singleton implementation
public sealed class UniqueIdentifierService
{
    private static readonly UniqueIdentifierService instance = new UniqueIdentifierService();
    static UniqueIdentifierService() { } // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
    private UniqueIdentifierService() { }
    public static UniqueIdentifierService Instance => instance;

    private readonly object _generatorsLock = new();

    private readonly Dictionary<string, UniqueIdentifierGenerator> _generators = [];

    public UniqueIdentifier CreateNext<TType>()
        => CreateNext(typeof(TType).Name);

    public UniqueIdentifier CreateNext(string typeName)
    {
        // Locking operation costs.
        // If we create 10000 documents of the same type we don't want to lock every time.
        // We need to lock only for the first time
        // This is also the reason why we don't use ConcurrentDictionary here
        if (!_generators.TryGetValue(typeName, out var generator))
        {
            //grants mutually exclusive access to a shared resource by acquiring or releasing a lock on the object that identifies the resource.
            lock (_generatorsLock)
            {
                //only one thread can enter this section at a time
                //lock is synchronous. You cannot await async tasks inside the lock

                //let's make sure while we were waiting for the lock another thread didn't create the generator
                if (!_generators.TryGetValue(typeName, out generator))
                {
                    generator = new UniqueIdentifierGenerator(typeName);
                    _generators.Add(typeName, generator);
                }
            }
        }

        return generator.CreateNext();
    }
}