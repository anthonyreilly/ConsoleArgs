using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;

namespace ConsoleArgs
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate the command line app
            var app = new CommandLineApplication();

            // This should be the name of the executable itself.
            // the help text line "Usage: ConsoleArgs" uses this
            app.Name = "ConsoleArgs";
            app.Description = ".NET Core console app with argument parsing.";
            app.ExtendedHelpText = "This is a sample console app to demostrate the usage of Microsoft.Extensions.CommandLineUtils."
                + Environment.NewLine + "Depending on your OS, you may need to execute the application as ConsoleArgs.exe or 'dotnet ConsoleArgs.dll'";

            // Set the arguments to display the description and help text
            app.HelpOption("-?|-h|--help");

            var basicOption = app.Option("-o|--option <optionvalue>",
                    "Some option value",
                    CommandOptionType.SingleValue);

            // Arguments are basic arguments, that are parsed in the order they are given
            // e.g ConsoleArgs "first value" "second value"
            // This is OK for really simple tasks, but generally you're better off using Options
            // since they avoid confusion
            var argOne = app.Argument("argOne", "App argument one");
            var argTwo = app.Argument("argTwo", "App argument two");

            // When no commands are specified, this block will execute.
            // This is the main "command"
            app.OnExecute(() =>
            {
                Console.WriteLine("Argument value one: {0}", argOne.Value ?? "null");
                Console.WriteLine("Argument value two: {0}", argTwo.Value ?? "null");

                //You can also use the Arguments collection to iterate through the supplied arguments
                foreach (CommandArgument arg in app.Arguments)
                {
                    Console.WriteLine("Arguments collection value: {0}", arg.Value ?? "null");
                }

                // Use the HasValue() method to check if the option was specified
                if (basicOption.HasValue())
                {
                    Console.WriteLine("Option was selected, value: {0}", basicOption.Value());
                }
                else
                {
                    Console.WriteLine("No options specified.");
                    // ShowHint() will display: "Specify --help for a list of available options and commands."
                    app.ShowHint();
                }

                return 0;
            });

            // This is a command with no arguments - it just does default action.
            app.Command("simple-command", (command) =>
                {
                    //description and help text of the command.
                    command.Description = "This is the description for simple-command.";
                    command.ExtendedHelpText = "This is the extended help text for simple-command.";
                    command.HelpOption("-?|-h|--help");

                    command.OnExecute(() =>
                    {
                        Console.WriteLine("simple-command is executing");

                        //Do the command's work here, or via another object/method

                        Console.WriteLine("simple-command has finished.");
                        return 0; //return 0 on a successful execution
                    });

                }
            );

            app.Command("complex-command", (command) =>
            {
                // This is a command that has it's own options.
                command.ExtendedHelpText = "This is the extended help text for complex-command.";
                command.Description = "This is the description for complex-command.";
                command.HelpOption("-?|-h|--help");

                // There are 3 possible option types:
                // NoValue
                // SingleValue
                // MultipleValue

                // MultipleValue options can be supplied as one or multiple arguments
                // e.g. -m valueOne -m valueTwo -m valueThree
                var multipleValueOption = command.Option("-m|--multiple-option <value>",
                    "A multiple-value option that can be specified multiple times",
                    CommandOptionType.MultipleValue);

                // SingleValue: A basic Option with a single value
                // e.g. -s sampleValue
                var singleValueOption = command.Option("-s|--single-option <value>",
                    "A basic single-value option",
                    CommandOptionType.SingleValue);

                // NoValue are basically booleans: true if supplied, false otherwise
                var booleanOption = command.Option("-b|--boolean-option",
                    "A true-false, no value option",
                    CommandOptionType.NoValue);

                command.OnExecute(() =>
                {
                    Console.WriteLine("complex-command is executing");

                    // Do the command's work here, or via another object/method                    

                    // Grab the values of the various options. when not specified, they will be null.

                    // The NoValue type has no Value property, just the HasValue() method.
                    bool booleanOptionValue = booleanOption.HasValue();
                    
                    // MultipleValue returns a List<string>
                    List<string> multipleOptionValues = multipleValueOption.Values;

                    // SingleValue returns a single string
                    string singleOptionValue = singleValueOption.Value();

                    // Check if the various options have values and display them.
                    // Here we're checking HasValue() to see if there is a value before displaying the output.
                    // Alternatively, you could just handle nulls from the Value properties
                    if(booleanOption.HasValue())
                    {
                        
                        Console.WriteLine("booleanOption option: {0}", booleanOptionValue.ToString());
                    }

                    if(multipleValueOption.HasValue())
                    {                        
                        Console.WriteLine("multipleValueOption option(s): {0}", string.Join(",", multipleOptionValues));
                    }

                    if(singleValueOption.HasValue())
                    {
                        
                        Console.WriteLine("singleValueOption option: {0}", singleOptionValue ?? "null");
                    }

                    Console.WriteLine("complex-command has finished.");
                    return 0; // return 0 on a successful execution
                });
            });

            try
            {
                // This begins the actual execution of the application
                Console.WriteLine("ConsoleArgs app executing...");
                app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                // You'll always want to catch this exception, otherwise it will generate a messy and confusing error for the end user.
                // the message will usually be something like:
                // "Unrecognized command or argument '<invalid-command>'"
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to execute application: {0}", ex.Message);
            }
        }
    }
}
