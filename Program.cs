using Microsoft.AspNetCore.SignalR;
using UpdateHub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// CORS
app.UseCors("AllowAll");

app.MapHub<ClientHub>("/Hub");
app.MapPost("/", async (GlobalUpdate globalUpdate, IHubContext<ClientHub> hub) => 
    await hub.Clients.All
        .SendAsync(globalUpdate.MethodName, globalUpdate.Message, globalUpdate.Data));
app.MapPost("/Group", async (GroupUpdate groupUpdate, IHubContext<ClientHub> hub) => 
    await hub.Clients.Groups(groupUpdate.GroupName)
        .SendAsync(groupUpdate.MethodName, groupUpdate.Message, groupUpdate.Data));
app.MapPost("/User", async (UserUpdate userUpdate, IHubContext<ClientHub> hub) => 
    await hub.Clients.User(userUpdate.UserId)
        .SendAsync(userUpdate.MethodName, userUpdate.Message, userUpdate.Data));

app.Run();