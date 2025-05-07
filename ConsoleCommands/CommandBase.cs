namespace ConsoleCommands;

public abstract class CommandBase
{
    public virtual string Syntax => GetType().Name.ToLower();
    public abstract string Description { get; }
    public string Usage => $"usage: {Syntax}";

    /// <returns>output message</returns>
    public abstract string Execute(IServiceProvider provider, string[] args);
}