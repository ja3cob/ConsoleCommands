using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleCommands
{
    public static class CC
    {
        private static readonly Dictionary<string, CommandBase> Commands = new Dictionary<string, CommandBase>();

        static CC()
        {
            var types = Assembly.GetEntryAssembly()
                ?.GetTypes()
                .Where(t => typeof(CommandBase).IsAssignableFrom(t) && t.IsAbstract == false);
            if (types == null)
            {
                return;
            }

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is CommandBase instance)
                {
                    Commands[type.Name.ToLower()] = instance;
                }
            }
        }

        public static async void ReadCommands(IServiceProvider provider)
        {
            await Task.Run(() =>
            {
                while ((provider.GetService<CancellationTokenSource>()?.IsCancellationRequested ?? false) == false)
                {
                    string[] input = Console.ReadLine()?.Split(' ');
                    if (input == null)
                    {
                        //Console.ReadLine() returns null when nothing more can be read from the terminal, so the service is terminated
                        return;
                    }

                    if (Commands.TryGetValue(input[0], out var command) == false)
                    {
                        Console.WriteLine("Commands:");
                        foreach (var dictCommand in Commands.Values)
                        {
                            Console.WriteLine($"{dictCommand.Syntax} - {dictCommand.Description}");
                        }

                        continue;
                    }

                    try
                    {
                        Console.WriteLine(command.Execute(provider, input.Skip(1).ToArray()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error executing command: " + ex.Message);
                    }
                }
            });
        }
    }
}