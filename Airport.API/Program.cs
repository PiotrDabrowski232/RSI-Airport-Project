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

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var request = context.Request;
    var response = context.Response;

    var ipAddress = context.Connection.RemoteIpAddress?.ToString();
    var method = request.Method;
    var path = request.Path;
    var userAgent = request.Headers["User-Agent"].FirstOrDefault();
    var authHeader = request.Headers["Authorization"].FirstOrDefault();

    var stopwatch = new System.Diagnostics.Stopwatch();
    stopwatch.Start();

    var originalBodyStream = response.Body;
    await using var responseBody = new MemoryStream();
    response.Body = responseBody;

    try
    {
        await next.Invoke();

        stopwatch.Stop();

        responseBody.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        await responseBody.CopyToAsync(originalBodyStream);

        logger.LogInformation(
            "HTTP {Method} {Path} from {IP} | UA: {UA} | Auth: {Auth} | Status: {StatusCode} | Time: {Elapsed} ms | ResponseBody: {ResponseBody}",
            method, path, ipAddress, userAgent, authHeader, response.StatusCode,
            stopwatch.ElapsedMilliseconds, text);
    }
    finally
    {
        response.Body = originalBodyStream;
    }
});




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
