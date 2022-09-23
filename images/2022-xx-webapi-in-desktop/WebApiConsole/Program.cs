using System.Diagnostics;
using Swashbuckle.AspNetCore.Annotations;

Action<string> log = t => { Trace.WriteLine($"T{Environment.CurrentManagedThreadId:D2} {t}"); };
var builder = WebApplication.CreateBuilder();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.EnableAnnotations());
builder.WebHost.UseUrls("http://localhost:8080");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var slidersStore = new SlidersStore(new Dictionary<string, int>()
    {
        { "S0", 000 },
        { "S1", 010 },
        { "S2", 020 },
    });

app.MapGet("/", () => "Hello World!");

app.MapGet("/sliders",
[SwaggerOperation(Summary = "Returns current sliders", Description = "GET DESCRIPTION")]
//[SwaggerResponse(200, "success")]
//[SwaggerResponse(500, "some failure")]
async () =>
{
    log("GET /sliders");
    var tcs = new TaskCompletionSource<IReadOnlyDictionary<string, int>>();

#pragma warning disable CS4014 // Ignore Task returned here since we just use Task.Run as hack of sending action to another thread
    Task.Run(() => GetSliders(tcs));
#pragma warning restore CS4014

    return await tcs.Task;
})
.WithName("GetSliders");

app.MapPut("/sliders",
[SwaggerOperation(Summary = "Set new sliders", Description = "PUT DESCRIPTION")]
//[SwaggerResponse(200, "success")]
//[SwaggerResponse(500, "some failure")]
async (Dictionary<string, int> newSliders) =>
{
    log("PUT /sliders");
    var tcs = new TaskCompletionSource();
    // Put can return state of sliders after if we want to be sure web client uses latest state after put, if that could be different
    //var tcs = new TaskCompletionSource<IReadOnlyDictionary<string, int>>();

#pragma warning disable CS4014 // Ignore Task returned here since we just use Task.Run as hack of sending action to another thread
    Task.Run(() => PutSliders(newSliders, tcs));
#pragma warning restore CS4014

    await tcs.Task;

    // if put returns latest state
    //return await tcs.Task;
})
.WithName("PutSliders");

void GetSliders(TaskCompletionSource<IReadOnlyDictionary<string, int>> tcs)
{
    log(nameof(GetSliders));
    lock (slidersStore)
    {
        tcs.SetResult(slidersStore.Sliders);
    }
}

void PutSliders(IReadOnlyDictionary<string, int> sliders, TaskCompletionSource tcs)
{
    log(nameof(PutSliders));
    lock (slidersStore)
    {
        slidersStore.Sliders = sliders;
        tcs.SetResult();
    }
}

var cts = new CancellationTokenSource();
await app.RunAsync(cts.Token);

public class SlidersStore
{
    public SlidersStore(IReadOnlyDictionary<string, int> sliders)
    {
        Sliders = sliders;
    }

    public IReadOnlyDictionary<string, int> Sliders { get; set; }
}
