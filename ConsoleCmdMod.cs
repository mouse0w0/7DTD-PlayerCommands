using System.Collections.Generic;

namespace PlayerCommands;

public class ConsoleCmdMod : ConsoleCmdAbstract
{
    public override bool AllowedInMainMenu => true;
    
    public override string[] GetCommands() => new[]
    {
        "playercommands",
        "pc"
    };
    
    public override string GetHelp() => Localization.Get("PC_CommandCmdHelp");

    public override string GetDescription() => Localization.Get("PC_ConsoleCmdDesc");

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        if (IsNoEnoughParam(_params, 1)) return;

        switch (_params[0])
        {
            case "reload":
            {
                Config.Load();
                Log.Out(Localization.Get("PDB_ReloadConfig"));
                return;
            }
        }
    }
    
    private static bool IsNoEnoughParam(List<string> _params, int _expectedCount)
    {
        if (_params.Count >= _expectedCount) return false;
        Log.Out(Localization.Get("PDB_NoEnoughParam"));
        return true;
    }
}