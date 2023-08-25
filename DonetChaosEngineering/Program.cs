using Polly.Extensions.Http;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;
using System.Net;
using System.Net.Sockets;
using Polly.Fallback;
using Polly.Timeout;
using Microsoft.Extensions.DependencyInjection;
using DonetChaosEngineering.Service;

var builder = WebApplication.CreateBuilder(args);
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));
}

// Chaos policy
static IAsyncPolicy<HttpResponseMessage> GetChaosPolicy()
{
    // Inject a fault with a 50% probability.
    var fault = new SocketException((int)SocketError.HostNotFound);
    return MonkeyPolicy.InjectFaultAsync<HttpResponseMessage>(
        fault,
        injectionRate: 0.5, // 50% of the calls
        enabled: () => true // you can control if you want to enable or not, for example, based on a configuration setting.
    );
}


// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IChaosService, ChaosService>(client =>
{
    client.BaseAddress = new Uri("https://api.externalshakyapi.com/");
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy())
.AddPolicyHandler(GetChaosPolicy());

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
