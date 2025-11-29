using DataExchange.ReaderApi.Services;

var builder = WebApplication.CreateBuilder(args);

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