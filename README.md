# ConsoleCommands
An easy to use .NET library for reading and executing console commands in dependency injection projects

## Usage
1. Inherit CommandBase class and implement the **Execute** method with desired command's behavior. Command name to be used in the console is the same as your inherited class name, but in all lower letters.
2. Before startup, call the **CC.ReadCommands** method, providing dependency injection service provider. Now the library will intercept the console input and respond if a command (class) is found in the assembly. If not, it will show all available commands and their usage. It will read forever until the thread exits or the console is closed.
	- You can also put a **CancellationTokenSource** in the DI container. If you do so, library will detect when a cancellation is requested and then it will stop intercepting commands.
   
    **Important:** CommandBase inherited commands **have to be** in the same assembly as the **CC.ReadCommands** method is called.

# Example

  Example command **AddUser**:

  >     internal class AddUser : CommandBase
  >     {
  >         public override string Syntax => base.Syntax + " <username>";
  >         public override string Description => "add a new user";
  >     
  >         public override string Execute(IServiceProvider provider, string[] args)
  >         {
  >             if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
  >             {
  >                 return Usage;
  >             }
  >     
  >             Console.WriteLine("password: ");
  >             string? password = Console.ReadLine();
  >     
  >             using var scope = provider.CreateScope();
  >             scope.ServiceProvider.GetRequiredService<AuthService>().Register(new RegisterRequest(args[0], password ?? ""));
  >     
  >             return "success";
  >         }
  >     }