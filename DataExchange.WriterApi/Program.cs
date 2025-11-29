using DataExchange.WriterApi.Services;

var builder = WebApplication.CreateBuilder(args);

// UKLONI Storage referencu - više ne koristimo direktno!
// builder.Services.AddSingleton<IRandomNumberStorage, InMemoryRandomNumberStorage>();

// External API service
builder.Services.AddHttpClient<ICsrngApiService, CsrngApiService>();

// Storage API client
builder.Services.AddHttpClient<IStorageApiClient, StorageApiClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();