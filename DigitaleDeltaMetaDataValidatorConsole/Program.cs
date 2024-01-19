using DigitaleDeltaMetaDataValidatorConsole.CommandLineHandlers;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
  config.AddCommand<ValidateCommand>("validate");
  config.AddCommand<ExtractCommand>("extract");
});

// args = [ "validate", "D:\\PropertyCheck2024.01.csv", "https://localhost:7071/v3/odata", "d:\\result.csv" ];
// args = [ "extract", "https://localhost:7071/v3/odata", "D:\\PropertyCheck2024.01.csv" ];

return await app.RunAsync(args);
