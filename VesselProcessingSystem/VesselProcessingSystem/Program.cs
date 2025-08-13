using VesselProcessingSystem.Importers;
using VesselProcessingSystem.Models;
using VesselProcessingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICruseGateHHImporter, CruseGateHHImporter>();
builder.Services.AddScoped<ICosmosDbService, CosmosDbService>();

builder.Services.Configure<Configurations>(
    builder.Configuration.GetSection("VesselConfig"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
