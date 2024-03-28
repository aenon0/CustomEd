--THIS IS WHAT THE CLIENT SIDE CODE LOOKS LIKE: 

using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5041/getAnnouncement", options =>
    {
        options.HttpMessageHandlerFactory = _ => new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseDefaultCredentials = true
        };
        options.Headers.Add("Authorization", $"Bearer tokenytoken");
    })
    .Build();




await connection.StartAsync();
Console.WriteLine("Connection started.");
connection.On<string>("Connected", (message) => {
    Console.WriteLine(message); 
});
await Task.Delay(-1);