# Building a custom CLI tool

In today's CI/CD pipelines there is a lot of stuff going on. We run commands, scripts and prebuild jobs that combined add to the complexity in understanding what is going on, especially in the case of debugging. An approach to simplify pipeline configuration can be to build a custom CLI tool that you build to run certain application specific tasks, like kicking of an integration test or bootstrapping certain components in our topology (API Call, DB Init). An additional benefit of having a CLI for your solution specifically is that it also can be used on the dev box and overtime you might find yourself updating the CLI to handle repetetive tasks (like kicking off a batch of API calls to test your updates). 

Having a custom CLI potentially can be a nice addition for your endusers to manage configuration and admin tasks very easily. It is very likely you have come to respect and appreciate a couple of these CLI tools already (like Azure's 'AZ' CLI), so why not build one yourself? 

## Getting Started

There have been a couple of attempts at simplifying building a CLI. The framework we are using in this article and the sample code is Nate McMaster's excellent and very lively [CommandLineUtils ultilities project](https://github.com/natemcmaster/CommandLineUtils). 

The basis of a CLI is a console application that handles multiple inputs parameters to indicate which task needs to be executed with what parameters. What the utilities add on top of that is the nice commandline UI we are accustomed to like seeing some help information and a listing of all the option a command supports. One convention we apply to the console app behaviour is the return value being 0 in case of successful execution and other values to indicate a different outcome, this is especially important if you are going to use your CLI in the context of a background process like a pipeline of some sorts. 

We will use this enum to apply this convention:
```
    enum ExitCode : int
    {
        Success = 0,
        Fail = 1
    }
 ```
We start of by defining the command line application with some metedata and the indication that we are going to inject the help mechanism.

```
    var app = new CommandLineApplication
    {
        Name = "apper",
        Description = "This is a sample Cli that showcases the possibilities of a Cli tool and various ways to interact with it.",
    };

    app.HelpOption();

    
```

Next we setup a command by giving it a name and defining a handler. The handler itself can get a command assinged to it and this gives us the typical CLI subcommand hierarchy approach.

```
    app.Command("do", doCommand =>
    {
        doCommand.OnExecute(() =>
        {
            doCommand.ShowHelp();
            return 1;
        });

        doCommand.Command("this", thisCmd =>
        {
            thisCmd.OnExecuteAsync(async x =>
            {
                //Your code executing the command
                
                return 0;

                ...
```

Handling and defining parameters is very easy. The meta data is used to construct the help output automatically. 

```
   doCommand.Command("this", thisCmd =>
                {
                    thisCmd.Description = "Do this.";
                    var arg1 = thisCmd.Argument("arg1", "First argument").IsRequired();
                    var arg2 = thisCmd.Argument("arg2", "Second argument").IsRequired();
```

It also possible to get user input but make sure to provide a default values to support non-interactive execution.

```
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

```
So this is how can define complex structures of commands and parameters. The last lines of our console app kick off the execution and arrange the help info to be displayed when subcommands are passed.

```
    app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

    return app.Execute(args);
```


### Considerations

It is also possible to use an attribute based approach where your decorate local variables (getters) to turn them into automatically mapped arguments. The route you take just depends on your personal preference.

### Running the sample

To run the sample you can just build and execute in a command window. To debug, the easiest way is to update the debug setting in 

![alt text](https://github.com/valeryjacobs/CliSample/blob/master/DebugSetting.PNG "Debug setting to run Cli")

## License

This sample is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## References

* [Nate McMaster's CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)

