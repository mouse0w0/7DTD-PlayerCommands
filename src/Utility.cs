using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform;
using UnityEngine;

namespace PlayerCommands;

public static class Utility
{
    public static string Format(this string format, object arg0)
    {
        return string.Format(format, arg0);
    }

    public static string Format(this string format, object arg0, object arg1)
    {
        return string.Format(format, arg0, arg1);
    }

    public static string Format(this string format, object arg0, object arg1, object arg2)
    {
        return string.Format(format, arg0, arg1, arg2);
    }

    public static string Format(this string format, params object[] args)
    {
        return string.Format(format, args);
    }

    [CanBeNull]
    public static EntityPlayer GetEntityPlayer(int entityId)
    {
        return GameManager.Instance.World.Players.dict.GetValueOrDefault(entityId, null);
    }

    [CanBeNull]
    public static EntityPlayer GetEntityPlayer(string playerName)
    {
        return GameManager.Instance.World.Players.list.Find(p => p.EntityName.EqualsCaseInsensitive(playerName));
    }

    [CanBeNull]
    public static EntityPlayer GetEntityPlayer(this ClientInfo clientInfo)
    {
        return clientInfo != null
            ? GetEntityPlayer(clientInfo.entityId)
            : GameManager.Instance.World.GetPrimaryPlayer();
    }

    [CanBeNull]
    public static ClientInfo GetClientInfo(int entityId)
    {
        var clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
        if (clientInfo != null) return clientInfo;

        var primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
        if (primaryPlayer != null && primaryPlayer.entityId == entityId) return null;

        throw new ClientInfoNotFoundException("Not found ClientInfo by entity id: " + entityId);
    }

    [CanBeNull]
    public static ClientInfo GetClientInfo(string playerName)
    {
        var clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForPlayerName(playerName);
        if (clientInfo != null) return clientInfo;

        var primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
        if (primaryPlayer != null && primaryPlayer.EntityName.EqualsCaseInsensitive(playerName)) return null;

        throw new ClientInfoNotFoundException("Not found ClientInfo by player name: " + playerName);
    }

    [CanBeNull]
    public static ClientInfo GetClientInfo(this EntityPlayer entityPlayer)
    {
        return GetClientInfo(entityPlayer.entityId);
    }

    public static void SendMessage(this ClientInfo receiver, string message, EChatType type = EChatType.Global)
    {
        if (receiver != null)
            GameManager.Instance.ChatMessageServer(receiver, type, -1, message, [receiver.entityId],
                EMessageSender.None);
        else
            GameManager.Instance.ChatMessageClient(type, -1, message, null, EMessageSender.None);
    }

    public static void Teleport(this ClientInfo clientInfo, Location location)
    {
        clientInfo.Teleport(location.Position, location.Rotation);
    }

    public static void Teleport(this ClientInfo clientInfo, Vector3 position, Vector3? rotation = null)
    {
        var netPackageTeleportPlayer =
            NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(position, rotation);
        if (clientInfo != null)
            clientInfo.SendPackage(netPackageTeleportPlayer);
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

    public static bool HasPermission(this ClientInfo clientInfo, int permissionLevel)
    {
        return clientInfo.GetUserPermissionLevel() <= permissionLevel;
    }

    public static int GetUserPermissionLevel(this ClientInfo clientInfo)
    {
        return clientInfo != null ? GameManager.Instance.adminTools.Users.GetUserPermissionLevel(clientInfo) : 0;
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

    public static void PopulateObject(this JToken token, object target,
        [CanBeNull] JsonSerializerSettings settings = null)
    {
        using (var reader = token.CreateReader())
        {
            JsonSerializer.CreateDefault(settings).Populate(reader, target);
        }
    }

    public static string _ToString([CanBeNull] this object value)
    {
        return value?.GetType().FullName + "\n" +
               JsonConvert.SerializeObject(value, new JsonSerializerSettings
               {
                   MaxDepth = 3,
                   Formatting = Formatting.Indented,
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                   ContractResolver = new MyContractResolver(),
                   Error = (sender, args) =>
                   {
                       Log.Out($"Serialization error: {args.ErrorContext.Error.Message}");
                       args.ErrorContext.Handled = true;
                   },
               });
    }
}