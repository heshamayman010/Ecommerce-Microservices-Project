
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services
  .AddOcelot()
  .AddPolly();

var app = builder.Build();
await app.UseOcelot();

app.Run();
