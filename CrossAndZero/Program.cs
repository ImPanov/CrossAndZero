using BasicAuthorization.Middleware;
using BasicAuthorization.Services.Implementations;
using BasicAuthorization.Services.Interfaces;
using Packet.Shared;
using Orleans.Runtime;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    // In order to support multiple hosts forming a cluster, they must listen on different ports.
    // Use the --InstanceId X option to launch subsequent hosts.
    var instanceId = ctx.Configuration.GetValue<int>("InstanceId");
    var port = 11_111;
    siloBuilder.UseLocalhostClustering(
        siloPort: port + instanceId,
        gatewayPort: 30000 + instanceId,
        primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, port));
});
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseMiddleware<BasicAuthMiddleware>();

app.MapControllers();

app.Run();
