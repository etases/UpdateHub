using Microsoft.AspNetCore.SignalR;
using UpdateHub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Services.AddCors();

var app = builder.Build();

app.MapHub<ClientHub>("/Hub");
app.MapPost("/", async (GlobalUpdate globalUpdate, IHubContext<ClientHub> hub, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger<GlobalUpdate>();
    logger.LogInformation("GlobalUpdate: {GlobalUpdateMethodName} {GlobalUpdateMessage} {GlobalUpdateData}", globalUpdate.MethodName, globalUpdate.Message, globalUpdate.Data);
    await hub.Clients.All.SendAsync(globalUpdate.MethodName, globalUpdate.Message, globalUpdate.Data);
});
app.MapPost("/Group", async (GroupUpdate groupUpdate, IHubContext<ClientHub> hub) => 
    await hub.Clients.Groups(groupUpdate.GroupName)
        .SendAsync(groupUpdate.MethodName, groupUpdate.Message, groupUpdate.Data));
app.MapPost("/User", async (UserUpdate userUpdate, IHubContext<ClientHub> hub) => 
    await hub.Clients.User(userUpdate.UserId)
        .SendAsync(userUpdate.MethodName, userUpdate.Message, userUpdate.Data));

app.UseCors
(
    policyBuilder =>
        policyBuilder
            .WithOrigins("http://localhost:3000", "https://localhost:3000", "https://localhost:7226", "http://localhost:5131")
            .AllowCredentials()
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
);

app.Run();