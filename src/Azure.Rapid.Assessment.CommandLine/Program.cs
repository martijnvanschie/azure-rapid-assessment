using Azure.Rapid.Assessment.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

// Setup the host builder
var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddSerilog((context, conf) => { conf.ReadFrom.Configuration(builder.Configuration); });
builder.Services.AddSingleton<Main>();
var host = builder.Build();

// Run the main process
var serviceProvider = builder.Services.BuildServiceProvider();
var service = serviceProvider.GetRequiredService<Main>();
var exitCode = await service.StartAsync(args);

Environment.Exit(exitCode);