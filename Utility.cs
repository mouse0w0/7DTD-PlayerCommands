using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Platform;
using UniLinq;
using UnityEngine;

namespace PlayerCommands;

public static class Utility
{
    public static EntityPlayer GetEntityPlayer(int entityId)
    {
        return GameManager.Instance.World.Players.dict.TryGetValue(entityId, out var entityPlayer)
            ? entityPlayer
            : null;
    }

    public static EntityPlayer GetEntityPlayer(string playerName)
    {
        var clientInfo = GetClientInfo(playerName);
        if (clientInfo != null) return GetEntityPlayer(clientInfo.entityId);

        var primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
        return primaryPlayer.EntityName == playerName ? primaryPlayer : null;
    }

    public static EntityPlayer GetEntityPlayer(this ClientInfo clientInfo)
    {
        return clientInfo != null
            ? GetEntityPlayer(clientInfo.entityId)
            : GameManager.Instance.World.GetPrimaryPlayer();
    }

    public static EntityPlayer FindEntityPlayer(string playerName)
    {
        var player = GetEntityPlayer(playerName);
        if (player != null) return player;
        var matches = GameManager.Instance.World.Players.list
            .FindAll(p => p.EntityName.ContainsCaseInsensitive(playerName))
            .ToArray();
        return matches.Length == 1 ? matches[0] : null;
    }

    public static ClientInfo GetClientInfo(int entityId)
    {
        return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
    }

    public static ClientInfo GetClientInfo(string playerName)
    {
        return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForPlayerName(playerName);
    }

    public static ClientInfo GetClientInfo(this EntityPlayer entityPlayer)
    {
        return GetClientInfo(entityPlayer.entityId);
    }

    public static void SendMessage(int receiverId, string message)
    {
        SendMessage(receiverId, null, message);
    }

    public static void SendMessage(this ClientInfo receiver, string message)
    {
        SendMessage(receiver, null, message);
    }

    public static void SendMessage(int receiverId, string sender, string message, EChatType type = EChatType.Global)
    {
        SendMessage(GetClientInfo(receiverId), sender, message, type);
    }

    public static void SendMessage(this ClientInfo receiver, string sender, string message,
        EChatType type = EChatType.Global)
    {
        if (receiver != null)
            GameManager.Instance.ChatMessageServer(receiver, type, -1, message, sender, false,
                new List<int> { receiver.entityId });
        else
            GameManager.Instance.ChatMessageClient(type, -1, message, sender, false, null);
    }

    public static void Teleport(this ClientInfo client, Location location)
    {
        Teleport(client, location.GetPosition(), location.GetRotation());
    }

    public static void Teleport(this ClientInfo client, Vector3 position, Vector3? rotation = null)
    {
        var netPackageTeleportPlayer =
            NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(position, rotation);
        if (client != null)
            client.SendPackage(netPackageTeleportPlayer);
        else
            netPackageTeleportPlayer.ProcessPackage();
    }

    public static bool IsServer()
    {
        return SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
    }

    public static bool IsClient()
    {
        return !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
    }

    public static bool IsAdmin(this ClientInfo clientInfo)
    {
        return clientInfo == null || GameManager.Instance.adminTools.GetUserPermissionLevel(clientInfo) == 0;
    }

    public static bool HasPermission(this ClientInfo clientInfo, string permission)
    {
        return Config.PlayerCommands.Contains(permission) || clientInfo.IsAdmin();
    }

    public static int GetEntityId(this ClientInfo clientInfo)
    {
        return clientInfo?.entityId ?? GameManager.Instance.World.GetPrimaryPlayerId();
    }

    public static string GetPlayerId(this ClientInfo clientInfo)
    {
        return clientInfo?.InternalId?.CombinedString ?? PlatformManager.InternalLocalUserIdentifier.CombinedString;
    }

    public static string GetPlayerName(this ClientInfo clientInfo)
    {
        return clientInfo?.playerName ?? GamePrefs.GetString(EnumGamePrefs.PlayerName);
    }

    public static void ProcessPackage(this NetPackage package)
    {
        package.ProcessPackage(GameManager.Instance.World, GameManager.Instance);
    }

    public static Location GetLocation(this Entity entity)
    {
        return new Location(entity);
    }

    public static string ToPositionString(float x, float z)
    {
        return (x, z) switch
        {
            (< 0, < 0) => $"{(int)-x} W {(int)-z} S",
            (< 0, _) => $"{(int)-x} W {(int)z} N",
            (_, < 0) => $"{(int)x} E {(int)-z} S",
            (_, _) => $"{(int)x} E {(int)z} N"
        };
    }

    [CanBeNull]
    public static string ReadAllText(string path)
    {
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    public static void WriteAllText(string path, string contents)
    {
        var parent = Path.GetDirectoryName(path);
        if (parent != null)
        {
            Directory.CreateDirectory(parent);
        }

        File.WriteAllText(path, contents);
    }
}