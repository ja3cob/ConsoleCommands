using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleCommands;

public static class CC
{
    private static readonly Dictionary<string, CommandBase> Commands = [];

    static CC()
    {
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(CommandBase).IsAssignableFrom(t) && t.IsAbstract == false);

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
            while (provider.GetRequiredService<CancellationTokenSource>().IsCancellationRequested == false)
            {
                string[]? input = Console.ReadLine()?.Split(' ');
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
                    Console.WriteLine(command.Execute(provider, input[1..]));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing command: " + ex.Message);
                }
            }
        });
    }
}