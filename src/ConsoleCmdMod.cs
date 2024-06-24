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

    public override string getHelp() => Localization.Get("PCCommandCmdHelp");

    public override string getDescription() => Localization.Get("PCConsoleCmdDesc");

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        if (_params.Count == 0)
        {
            Log.Out(Localization.Get("PCConsoleCmdUsage"));
            return;
        }

        switch (_params[0])
        {
            case "reload":
            {
                Config.Load();
                Log.Out(Localization.Get("PCConsoleReloadConfig"));
                return;
            }
        }
    }

    private static bool IsNoEnoughParam(List<string> _params, int _expectedCount)
    {
        if (_params.Count >= _expectedCount) return false;
        Log.Out(Localization.Get("PCConsoleNoEnoughParam"));
        return true;
    }
}