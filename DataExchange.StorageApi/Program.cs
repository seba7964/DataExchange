using DataExchange.Storage.Interfaces;
using DataExchange.Storage.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRandomNumberStorage, InMemoryRandomNumberStorage>();

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