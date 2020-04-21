using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CliSample
{
    enum ExitCode : int
    {
        Success = 0,
        Fail = 1
    }
    class Program
    {
        public static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
         .AddEnvironmentVariables()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("local.settings.json");

            var configuration = builder.Build();

            var app = new CommandLineApplication
            {
                Name = "apper",
                Description = "This is a sample Cli that showcases the possibilities of a Cli tool and various ways to interact with it.",
            };

            app.HelpOption();

            app.Command("do", doCommand =>
            {
                doCommand.OnExecute(() =>
                {
                    doCommand.ShowHelp();
                    return 1;
                });

                doCommand.Command("this", thisCmd =>
                {
                    thisCmd.Description = "Do this.";
                    var arg1 = thisCmd.Argument("arg1", "First argument").IsRequired();
                    var arg2 = thisCmd.Argument("arg2", "Second argument").IsRequired();

                    thisCmd.OnExecuteAsync(async x =>
                    {
                        Console.WriteLine($"Doing this with arguments: {arg1.Value} & {arg2.Value}");

                        if (Prompt.GetYesNo("Do you want to proceed?", false))
                        {
                            await Task.Run(() => { Console.WriteLine("Doing this..."); });
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine("Command cancelled.");
                            return 1;
                        }
                    });
                });

                doCommand.Command("that", thatCmd =>
                {
                    var json = thatCmd.Option("--json", "Json output format", CommandOptionType.NoValue);

                    thatCmd.OnExecuteAsync(async x =>
                    {
                        Console.WriteLine("Doing that...");
                        if (json.HasValue())
                        {
                            Console.WriteLine("{'output':'Output in JSON'}");
                        }
                        else
                        {
                            Console.WriteLine("Output in text");
                        }
                        return 0;
                    });
                });
            });

            app.Command("list", testCmd =>
            {
                testCmd.OnExecute(() =>
                {
                    Console.WriteLine();
                    Console.WriteLine("These are the available commands:");
                    Console.WriteLine("this");
                    Console.WriteLine("that");
                    Console.WriteLine();

                    return 0;
                });
            });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            return app.Execute(args);
        }
    }
}
