using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Warren.Consle;
using Warren.Domain;

//LOAD CONFIGURATION
var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration configuration = builder.Build();

//SETUP HOST
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton(configuration);
        services.Configure<Settings>(options => configuration.GetSection("settings").Bind(options));
        services.Configure<SlotSettings>(options => configuration.GetSection("settings:slotSettings").Bind(options));

        services.AddSingleton<SlotMachineGame>();
    })
    .Build();

//RUN
var startup = host.Services.GetRequiredService<SlotMachineGame>();
startup.Execute();