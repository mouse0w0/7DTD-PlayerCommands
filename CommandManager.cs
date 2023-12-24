using System;
using System.Collections.Generic;
using HarmonyLib;
using PlayerCommands.Command;

namespace PlayerCommands;

public delegate void CommandHandler(CommandSender sender, string[] args);

[HarmonyPatch]
public static class CommandManager
{
    public static readonly Dictionary<string, CommandHandler> Commands = new();

    static CommandManager()
    {
        Commands["help"] = HelpCommand.Help;
        Commands["tp"] = TpCommand.Tp;
        Commands["tph"] = TpCommand.TpHere;
        Commands["tpaccept"] = TpCommand.TpAccept;
        Commands["spawn"] = SpawnCommand.Spawn;
        Commands["setspawn"] = SpawnCommand.SetSpawn;
        Commands["delspawn"] = SpawnCommand.DelSpawn;
        Commands["warp"] = WarpCommand.Warp;
        Commands["setwarp"] = WarpCommand.SetWarp;
        Commands["delwarp"] = WarpCommand.DelWarp;
        Commands["listwarp"] = WarpCommand.ListWarp;
        Commands["home"] = HomeCommand.Home;
        Commands["sethome"] = HomeCommand.SetHome;
        Commands["delhome"] = HomeCommand.DelHome;
        Commands["listhome"] = HomeCommand.ListHome;
        Commands["back"] = BackCommand.Back;
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.ChatMessageServer))]
    [HarmonyPrefix]
    public static bool GameManager_ChatMessageServer_Prefix(ClientInfo _cInfo, int _senderEntityId, string _msg,
        string _mainName)
    {
        if (Utility.IsClient()) return true;
        if (_senderEntityId == -1) return true;
        
        _msg = _msg.Trim();
        
        var idx = _msg.IndexOf(' ');
        var name = (idx == -1 ? _msg : _msg.Substring(0, idx)).ToLower();
        if (!Config.Commands.TryGetValue(name, out var command)) return true;
        Log.Out($"{_mainName} (from {_cInfo.GetPlayerId()}, entity id {_senderEntityId}) issued command: {_msg}");
        if (!Commands.TryGetValue(command, out var handler)) return true;
        var args = idx == -1 ? Array.Empty<string>() : _msg.Substring(idx + 1).Split(' ');
        handler(new CommandSender(_senderEntityId, _mainName, _cInfo), args);
        return false;
    }
}