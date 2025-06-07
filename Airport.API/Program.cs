using Airport.Server.Context;
using Airport.Server.Repositories.Interfaces;
using Airport.Server.Repositories;
using Ariport.Server.Repositories.Interfaces;
using Ariport.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Ariport.Server.Services.Interfaces;
using Ariport.Server.Services;

var builder = WebApplication.CreateBuilder(args);
// Pobierz connection string z appsettings.json
var connectionString = builder.Configuration.GetConnectionString("AirportServiceDbContext");

// Rejestracja kontekstu z przekazaniem connection stringa do konstruktora
builder.Services.AddScoped<AirportDbContext>(_ => new AirportDbContext(connectionString));

builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<IAirplaneTicketRepository, AirplaneTicketRepository>();
builder.Services.AddScoped<IFlightService, AirportService>();
builder.Services.AddScoped<IPassengerService, AirportService>();
builder.Services.AddScoped<IAirplaneTicketService, AirportService>();


builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
