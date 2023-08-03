using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using Warren.Consle;
using Warren.Domain;
using Warren.SlotMachine.Account;
using Warren.SlotMachine.SlotMachine;
using Warren.SlotMachine.Validation;

//LOAD CONFIGURATION
var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration configuration = builder.Build();

//SETUP HOST
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton(configuration);
        
        services.AddOptions<Settings>()
        .BindConfiguration("settings")
        .Validate(SlotMachineSettingsValidator.ValidateSettings)
        .ValidateOnStart();

        services.AddOptions<SlotSettings>()
        .BindConfiguration("settings:slotsettings")
        .Validate(SlotMachineSettingsValidator.ValidateSlotSettings)
        .ValidateOnStart();

        services.AddSingleton<SlotMachineGame>();

        services.AddSingleton<IAccountService, AccountService>();
        services.AddTransient<ISlotMachineEngine, SlotMachineEngine>();
        services.AddTransient<ISlotMachine, SlotMachine>();
    })
    .Build();

//RUN
var startup = host.Services.GetRequiredService<SlotMachineGame>();
startup.Execute();
