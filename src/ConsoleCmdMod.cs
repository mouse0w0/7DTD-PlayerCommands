using System.Collections.Generic;

namespace PlayerCommands;

public class ConsoleCmdMod : ConsoleCmdAbstract
{
    public override bool AllowedInMainMenu => true;
    
    public override string[] getCommands() => new[]
    {
        "playercommands",
        "pc"
    };

    public override string getHelp() => Message.Get("Console.Help");

    public override string getDescription() => Message.Get("Console.Desc");

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        if (_params.Count == 0)
        {
            Log.Out(Message.Get("Console.Usage"));
            return;
        }

        switch (_params[0])
        {
            case "reload":
            {
                Config.Load();
                Message.Load();
                Log.Out(Message.Get("Console.Reloaded"));
                return;
            }
        }
    }
}