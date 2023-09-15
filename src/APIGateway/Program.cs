using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var roadrunnerClientOrigins = "_roadrunnerClientOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    builder.Environment.IsDevelopment()
    ? $"ocelot.{builder.Environment.EnvironmentName}.json"
    : "ocelot.json");

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: roadrunnerClientOrigins,
                      policyBuilder =>
                      {
                          policyBuilder.WithOrigins(builder.Configuration.GetSection("CorsOptions:Origins").Value.Split(";"))
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                      });
});
builder.Services.AddOcelot();

var app = builder.Build();

//app.UseHttpsRedirection(); // TODO: https redirection not working together with ocelot

// Enable Cross-Origin Requests (CORS)
app.UseCors(roadrunnerClientOrigins);

app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();

app.Run();