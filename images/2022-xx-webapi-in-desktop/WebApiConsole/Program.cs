var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapGet("/sliders", () =>
{
    // HACK FOR NOW
    lock (slidersStore)
    {
        return slidersStore.Sliders;
    }
})
.WithName("GetSliders");

app.MapPut("/sliders", (Dictionary<string, int> newSliders) =>
{
    // HACK FOR NOW
    lock (slidersStore)
    {
        slidersStore.Sliders = newSliders;
    }
})
.WithName("PutSliders");


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
