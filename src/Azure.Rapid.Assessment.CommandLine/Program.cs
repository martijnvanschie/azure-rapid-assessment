using Azure.Rapid.Assessment.CommandLine;
using Azure.Rapid.Assessment.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

// Setup the host builder
var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddSerilog((context, conf) => { conf.ReadFrom.Configuration(builder.Configuration); });
builder.Services.AddSingleton<Main>();

builder.Services.AddTransient<QueryFileHandler>();

var host = builder.Build();

// Run the main process
var serviceProvider = builder.Services.BuildServiceProvider();
var service = serviceProvider.GetRequiredService<Main>();

// Register upstream dependencies
var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
HostManager.Initiate(loggerFactory);

var exitCode = await service.StartAsync(args);

Environment.Exit(exitCode);