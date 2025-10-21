using System.Collections.Concurrent;
using System.Diagnostics;

namespace Demo.Invoices.API.SampleCode;

//https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
public static class Asynchronous
{
    //Running async method synchronously
    //This is just for Demo purposes. Usually running async code synchronously is bad idea and inidicates that our design is broken.
    //This is also extremely rare scenario as most of the modern applications are async all the way.
    public static void RunCode()
    {
        //CancellationTokenSource allows to create CancellationToken
        //Source can signal cancellation request
        using var applicationCancellationTokenSource = new CancellationTokenSource();
        //CancellationToken is used to propagate notification that operations should be canceled
        var cancellationToken = applicationCancellationTokenSource.Token;

        //This will create task running in background
        var asyncTask = RunCodeAsync();
        //Wait method blocks current thread until task is completed
        asyncTask.Wait();

        //This will create task with result running in background
        var asyncTaskWithResult1 = RunCodeAsyncWithResult();
        asyncTaskWithResult1.Wait();
        var result1 = asyncTaskWithResult1.Result;

        //This will create task with result running in background
        var asyncTaskWithResult2 = RunCodeAsyncWithResult();
        var result2 = asyncTaskWithResult2.Result; //Result property blocks current thread until task is completed

        //Creating task and not awaiting it is popular way to create quick fire-and-forget operations
        //However, this is not recommended as it can lead to unobserved exceptions and other issues
        //Better approach is create generic singleton service and pass created f-a-f task to it

        RunCodeAsyncForWebDocument(cancellationToken).Wait();

        //Signal cancellation to all operations using this token
        applicationCancellationTokenSource.Cancel();
    }

    //Task class in .NET is representation of an asynchronous operation
    public static Task RunCodeAsync()
    {
        //This method should create task and return it
        //Or it can return task created by underlying frameworks (e.g. repository while awaiting SQL server response, or httpClient)

        //Task.CompletedTask is a static property returning already completed task
        //This is very usefull when method is declared as async but it runs synchronously
        //Perfect example is if we implement interface expecting method to be async (IValidator.ValidateAsync()) but we run it synchronously - check if string is empty
        return Task.CompletedTask;
    }

    public static Task<int> RunCodeAsyncWithResult()
        => Task.FromResult(42); //This method creates already completed task with result


    //'async' keyword enables 'await' instruction to be use in method
    //in practice this means that our method can wait for another to complete without blocking current thread
    public static async Task RunCodeAsyncForWebDocument(CancellationToken hostCancellationToken)
    {
        //Allows to combine multiple cancellation tokens into one
        //Which ever token signals cancellation first - linked token will be signaled as well
        var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            hostCancellationToken,
            new CancellationTokenSource(TimeSpan.FromSeconds(55)).Token); // Timeout token

        //Concurrent collections are thread-safe collections
        var documentsToProcess = new ConcurrentQueue<WebDocument>(); //concurrent queue
        var processedDocuments = new ConcurrentBag<WebDocument>(); //concurrent list

        var urls = new[]
        {
            "https://learn.microsoft.com",
            "https://www.microsoft.com",
            "https://www.google.com",
            "https://www.github.com",
            "https://www.bing.com",
            "https://portal.azure.com",
            "https://dev.azure.com",
            "https://www.win-rar.com/start.html?&L=0"
        };

        //Parallel loop
        Parallel.ForEach(urls,
            new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, url =>
            {
                documentsToProcess.Enqueue(url);
            });

        var numberOfProcessors = Environment.ProcessorCount;
        var timer = Stopwatch.StartNew();
        var processorsTasks = Enumerable.Range(0, numberOfProcessors)
            .Select(_ => WebDocumentProcessor.CreateRunningProcessor(documentsToProcess, processedDocuments, linkedCancellationTokenSource.Token));

        await Task.WhenAll(processorsTasks); // Await all processors to complete
        timer.Stop();

        var total = processorsTasks.Select(s => s.Result).Sum();
        Console.WriteLine($"Processed {total} documents in {timer.ElapsedMilliseconds / 1000} s");

        foreach (var doc in processedDocuments)
        {
            Console.WriteLine($"{doc.Url} contains Microsoft fingerprints: {doc.MicrosoftFingerprint}");
        }
    }

    public class WebDocument
    {
        public static implicit operator WebDocument(string url) => new(url);
        public WebDocument(string url)
        {
            Url = url;
        }

        public string Url { get; set; } = null!;

        public string Page { get; set; } = string.Empty;

        public bool MicrosoftFingerprint { get; set; }
    }

    public class WebDocumentProcessor
    {
        public static Task<int> CreateRunningProcessor(ConcurrentQueue<WebDocument> documentsToProcess, ConcurrentBag<WebDocument> processedDocuments, CancellationToken cancellationToken)
        {
            var processor = new WebDocumentProcessor(documentsToProcess, processedDocuments);
            return processor.RunProcessor(cancellationToken);
        }

        private readonly ConcurrentQueue<WebDocument> _documentsToProcess;

        private readonly ConcurrentBag<WebDocument> _processedDocuments;

        private WebDocumentProcessor(ConcurrentQueue<WebDocument> documentsToProcess, ConcurrentBag<WebDocument> processedDocuments)
        {
            _documentsToProcess = documentsToProcess;
            _processedDocuments = processedDocuments;
        }

        private async Task<int> RunProcessor(CancellationToken cancellationToken)
        {
            var documentsProcessed = 0;

            while (!cancellationToken.IsCancellationRequested
                && !_documentsToProcess.IsEmpty
                && _documentsToProcess.TryDequeue(out var document))
            {
                _processedDocuments.Add(document);

                await LoadPageAsync(document, cancellationToken);
                await Analyze(document, cancellationToken);
            }

            return documentsProcessed;
        }

        private Task Analyze(WebDocument document, CancellationToken cancellationToken)
        {
            var analyzeTask = new Task(() =>
            {
                LongAnalysis();

                document.MicrosoftFingerprint = document.Page.Contains("microsoft", StringComparison.OrdinalIgnoreCase);

            }, cancellationToken);

            analyzeTask.Start();

            return analyzeTask;
        }

        // Simulate long analysis
        private void LongAnalysis()
            => Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        private async Task LoadPageAsync(WebDocument document, CancellationToken cancellationToken)
        {
            //This is demo code.
            //Creating HttpClient is very bad practice.
            //Use DI Container and IHttpClientFactory to create HttpClient instances.
            using var httpClient = new HttpClient();
            document.Page = await httpClient.GetStringAsync(document.Url, cancellationToken);

            //This is also good example why droping 'await' to implement fire-and-forget is bad idea.
            //In this case it would result in httpClient being disposed before request is completed.
            //Document page would not be loaded and unobserved exception would be thrown.
        }
    }
}