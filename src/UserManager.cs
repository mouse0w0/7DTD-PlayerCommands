using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace PlayerCommands;

public static class UserManager
{
    private static readonly string UserDataDirectory = GameIO.GetSaveGameDir() + "/PlayerCommands/UserData";
    private static readonly string LegacyUserDataDirectory = GameIO.GetSaveGameDir() + "/PlayerCommands/PlayerData";

    private static readonly Dictionary<int, User> EntityIdToUser = new();
    private static readonly Dictionary<string, User> PlayerIdToUser = new();
    private static readonly Dictionary<string, User> PlayerNameToUser = new();

    static UserManager()
    {
        if (Directory.Exists(LegacyUserDataDirectory))
        {
            foreach (var legacyFile in Directory.GetFiles(LegacyUserDataDirectory))
            {
                var jObject = JObject.Parse(File.ReadAllText(legacyFile));
                if (jObject.Remove("lastPlayerName", out var jLastPlayerName))
                {
                    jObject.Add("LastPlayerName", jLastPlayerName);
                }

                if (jObject.Remove("homes", out var jHomes))
                {
                    jObject.Add("Homes", jHomes);
                }

                Utility.WriteAllText(UserDataDirectory + "/" + Path.GetFileName(legacyFile), jObject.ToString());
            }

            Directory.Move(LegacyUserDataDirectory, LegacyUserDataDirectory + "_deletable");
        }
    }

    public static IEnumerable<User> GetUsers()
    {
        return EntityIdToUser.Values;
    }

    public static User GetPrimaryUser()
    {
        return EntityIdToUser.GetValueOrDefault(GameManager.Instance.World.GetPrimaryPlayerId());
    }

    public static User GetUserByEntityId(int entityId)
    {
        return EntityIdToUser.GetValueOrDefault(entityId);
    }

    public static User GetUserByPlayerId(string playerId)
    {
        return PlayerIdToUser.GetValueOrDefault(playerId);
    }

    public static User GetUserByPlayerName(string playerName)
    {
        return PlayerNameToUser.GetValueOrDefault(playerName);
    }

    [CanBeNull]
    public static User FindUserByPlayerName(string playerName)
    {
        int foundUserCount = 0;
        User foundUser = null;
        foreach (var user in EntityIdToUser.Values)
        {
            if (user.PlayerName.EqualsCaseInsensitive(playerName))
            {
                return user;
            }

            if (user.PlayerName.ContainsCaseInsensitive(playerName))
            {
                foundUserCount++;
                foundUser = user;
            }
        }

        return foundUserCount == 1 ? foundUser : null;
    }

    public static User ToUser(this ClientInfo clientInfo)
    {
        return GetUserByEntityId(clientInfo.GetEntityId());
    }

    public static User ToUser(this EntityPlayer entityPlayer)
    {
        return GetUserByEntityId(entityPlayer.entityId);
    }

    public static void OnPlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType type, Vector3i position)
    {
        if (Utility.IsClient()) return;
        if (type is RespawnType.Died or RespawnType.Teleport or RespawnType.Unknown) return;

        var playerId = clientInfo.GetPlayerId();
        var playerName = clientInfo.GetPlayerName();
        Log.Out($"[PlayerCommands] Loading user data {playerId}/{playerName}");

        var userData = new UserData(UserDataDirectory + $"/{playerId}.json");
        try
        {
            userData.Load();
        }
        catch (Exception e)
        {
            Log.Error($"[PlayerCommands] Error while loading user data {playerId}/{playerName}");
            Log.Exception(e);
        }


        var user = new User(clientInfo.GetEntityPlayer(), clientInfo, userData);
        user.UserData.LastPlayerName = user.PlayerName;
        EntityIdToUser[user.EntityId] = user;
        PlayerIdToUser[user.PlayerId] = user;
        PlayerNameToUser[user.PlayerName] = user;

        Log.Out($"[PlayerCommands] Loaded user data {playerId}/{playerName}");
    }

    public static void OnPlayerDisconnected(ClientInfo clientInfo, bool shutdown)
    {
        if (Utility.IsClient()) return;

        var user = clientInfo.ToUser();
        if (user == null) return;

        user.Save();

        EntityIdToUser.Remove(user.EntityId);
        PlayerIdToUser.Remove(user.PlayerId);
        PlayerNameToUser.Remove(user.PlayerName);
    }

    public static void OnWorldUnloading()
    {
        if (Utility.IsClient()) return;
        foreach (var user in EntityIdToUser.Values)
        {
            user.Save();
        }

        EntityIdToUser.Clear();
        PlayerIdToUser.Clear();
        PlayerNameToUser.Clear();
    }
}