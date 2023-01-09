using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(FunctionApp.Startup))]
namespace FunctionApp
{
    class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string cs = Environment.GetEnvironmentVariable("ConnectionString");
            string currentEnvironment = Environment.GetEnvironmentVariable("CurrentEnvironment");
            builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(cs)
                        // Load all keys thart have Prod label.
                        .Select("*", currentEnvironment)
                        // Configure to reload configuration if the registered key ' is modified.
                        .ConfigureRefresh(refresh =>
                        {
                            refresh.Register("*", refreshAll: true)
                                   .SetCacheExpiration(new TimeSpan(0, 5, 0));
                        });

            });
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
        }
    }
}
