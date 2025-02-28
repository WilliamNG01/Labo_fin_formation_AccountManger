using JwtConfiguration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddJwtAutnetication(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

try
{
    await app.UseOcelot();
}
catch (Exception ex)
{
    // G�rer l'exception ici (par exemple, journalisation, message d'erreur)
    Console.WriteLine($"Erreur lors du d�marrage d'Ocelot : {ex.Message}");
}

app.Run();
