using System;
using System.Collections.Generic;
using HarmonyLib;
using PlayerCommands.Command;

namespace PlayerCommands;

public delegate void CommandHandler(CommandSender sender, string[] args);

[HarmonyPatch]
public static class CommandManager
{
    public static readonly Dictionary<string, CommandHandler> commands = new();

    static CommandManager()
    {
        commands["?"] = HelpCommand.Help;
        commands["help"] = HelpCommand.Help;
        commands["tp"] = TpCommand.Tp;
        commands["tph"] = TpCommand.TpHere;
        commands["ok"] = TpCommand.Ok;
        commands["spawn"] = SpawnCommand.Spawn;
        commands["sp"] = SpawnCommand.Spawn;
        commands["setspawn"] = SpawnCommand.SetSpawn;
        commands["ssp"] = SpawnCommand.SetSpawn;
        commands["delspawn"] = SpawnCommand.DelSpawn;
        commands["dsp"] = SpawnCommand.DelSpawn;
        commands["warp"] = WarpCommand.Warp;
        commands["wp"] = WarpCommand.Warp;
        commands["setwarp"] = WarpCommand.SetWarp;
        commands["swp"] = WarpCommand.SetWarp;
        commands["delwarp"] = WarpCommand.DelWarp;
        commands["dwp"] = WarpCommand.DelWarp;
        commands["listwarp"] = WarpCommand.ListWarp;
        commands["lwp"] = WarpCommand.ListWarp;
        commands["home"] = HomeCommand.Home;
        commands["hm"] = HomeCommand.Home;
        commands["sethome"] = HomeCommand.SetHome;
        commands["shm"] = HomeCommand.SetHome;
        commands["delhome"] = HomeCommand.DelHome;
        commands["dhm"] = HomeCommand.DelHome;
        commands["listhome"] = HomeCommand.ListHome;
        commands["lhm"] = HomeCommand.ListHome;
        commands["back"] = BackCommand.Back;
        commands["bk"] = BackCommand.Back;
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.ChatMessageServer))]
    [HarmonyPrefix]
    public static bool GameManager_ChatMessageServer_Prefix(ClientInfo _cInfo, int _senderEntityId, string _msg, string _mainName)
    {
        if (Utility.IsClient()) return true;

        if (_senderEntityId == -1) return true;

        _msg = _msg.Trim();
        if (!_msg.StartsWith("/")) return true;

        Log.Out(
            $"{_mainName} (from {_cInfo.GetPlayerId()}, entity id {_senderEntityId}) issued command: {_msg}");

        var idx = _msg.IndexOf(' ');
        var name = (idx == -1 ? _msg.Substring(1) : _msg.Substring(1, idx - 1)).ToLower();
        var args = idx == -1 ? Array.Empty<string>() : _msg.Substring(idx + 1).Split(' ');
        if (!commands.TryGetValue(name, out var handler)) return true;
        handler(new CommandSender(_senderEntityId, _mainName, _cInfo), args);
        return false;
    }
}