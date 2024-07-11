using System;
using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using PlayerCommands.Commands;

namespace PlayerCommands;

public delegate void CommandHandler(User sender, string[] args);

[HarmonyPatch]
public static class CommandManager
{
    private static readonly Dictionary<string, Command> NameToCommands = new();
    private static readonly Dictionary<string, Command> LabelToCommands = new();

    static CommandManager()
    {
        Register("help", HelpCommand.Help);
        Register("tp", TpCommand.Tp);
        Register("tphere", TpCommand.TpHere);
        Register("tpall", TpCommand.TpAll);
        Register("tpcancel", TpCommand.TpCancel);
        Register("tpaccept", TpCommand.TpAccept);
        Register("tpdeny", TpCommand.TpDeny);
        Register("tptoggle", TpCommand.TpToggle);
        Register("tpauto", TpCommand.TpAuto);
        Register("suicide", SuicideCommand.Suicide);
        Register("back", BackCommand.Back);
        Register("home", HomeCommand.Home);
        Register("sethome", HomeCommand.SetHome);
        Register("delhome", HomeCommand.DelHome);
        Register("listhome", HomeCommand.ListHome);
        Register("spawn", SpawnCommand.Spawn);
        Register("setspawn", SpawnCommand.SetSpawn);
        Register("delspawn", SpawnCommand.DelSpawn);
        Register("warp", WarpCommand.Warp);
        Register("setwarp", WarpCommand.SetWarp);
        Register("delwarp", WarpCommand.DelWarp);
        Register("listwarp", WarpCommand.ListWarp);
        // TODO tprandom - 随机传送到某个地方
        // TODO ignore - 忽略某个玩家
    }

    private static void Register(string name, CommandExecutor executor)
    {
        NameToCommands.Add(name, new Command(name, executor));
    }

    internal static void LoadConfig(JToken jToken)
    {
        LabelToCommands.Clear();
        foreach (var (name, command) in NameToCommands)
        {
            var commandToken = jToken[name];
            if (commandToken == null) continue;

            commandToken.PopulateObject(command);
            foreach (var label in command.Labels)
            {
                if (!LabelToCommands.TryAdd(label, command))
                {
                    Log.Warning($"[PlayerCommands] Duplicate label `{label}` in command `{name}`");
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.ChatMessageServer))]
    [HarmonyPrefix]
    public static bool GameManager_ChatMessageServer_Prefix(ClientInfo _cInfo, int _senderEntityId, string _msg,
        string _mainName)
    {
        if (Utility.IsClient()) return true;
        if (_senderEntityId == -1) return true;

        _msg = _msg.Trim();

        var args = _msg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0) return false;

        var label = args[0].ToLower();
        if (!LabelToCommands.TryGetValue(label, out var command)) return true;

        var sender = _cInfo.ToUser();
        Log.Out($"{sender.PlayerName} (from {sender.PlayerId}, entity id {sender.EntityId}) issued command: {_msg}");

        if (!sender.HasPermission(command))
        {
            sender.SendMessage(Message.Get("NoEnoughPerm"));
            return false;
        }

        command.Executor(_cInfo.ToUser(), command, label, args[1..]);
        return false;
    }
}