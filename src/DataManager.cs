using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;

namespace PlayerCommands;

[HarmonyPatch]
public static class DataManager
{
    public static Location? spawn;
    public static Dictionary<string, Location> warps;

    public static readonly Dictionary<string, PlayerData> PlayerDataDict = new();

    [HarmonyPatch(typeof(GameManager), "createWorld")]
    [HarmonyPostfix]
    public static void GameManager_CreateWorld_Postfix()
    {
        if (Utility.IsClient()) return;
        Load();
    }

    [HarmonyPatch(typeof(World), nameof(World.UnloadWorld))]
    [HarmonyPrefix]
    public static void World_UnloadWorld_Prefix()
    {
        if (Utility.IsClient()) return;
        Save();
        Cleanup();
    }

    internal static void OnPlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType type, Vector3i position)
    {
        if (Utility.IsClient()) return;
        if (type is RespawnType.Died or RespawnType.Teleport or RespawnType.Unknown) return;

        var playerId = clientInfo.GetPlayerId();
        var playerName = clientInfo.GetPlayerName();
        Log.Out($"[PlayerCommands] Loading player data {playerId}/{playerName}");

        var playerDataPath = GetPlayerDataPath($"{playerId}.json");
        PlayerData playerData = null;
        if (File.Exists(playerDataPath))
        {
            try
            {
                playerData = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(playerDataPath));
            }
            catch (Exception e)
            {
                Log.Error($"[PlayerCommands] Error while loading player data {playerId}/{playerName}");
                Log.Exception(e);
            }
        }

        playerData ??= new PlayerData();
        playerData.lastPlayerName = playerName;
        PlayerDataDict.Add(playerId, playerData);

        Log.Out($"[PlayerCommands] Loaded player data {playerId}/{playerName}");
    }

    internal static void OnPlayerDisconnected(ClientInfo clientInfo, bool shutdown)
    {
        if (Utility.IsClient()) return;
        SavePlayerData(clientInfo.GetPlayerId());
    }

    private static void SavePlayerData(string playerId)
    {
        if (PlayerDataDict.Remove(playerId, out var playerData))
        {
            SavePlayerData(playerId, playerData);
        }
    }

    private static void SavePlayerData(string playerId, PlayerData playerData)
    {
        Log.Out($"[PlayerCommands] Saving player data {playerId}/{playerData.lastPlayerName}");
        var playerDataPath = GetPlayerDataPath($"{playerId}.json");
        try
        {
            Utility.WriteAllText(playerDataPath, JsonConvert.SerializeObject(playerData));
        }
        catch (Exception e)
        {
            Log.Error($"[PlayerCommands] Error while saving player data {playerId}/{playerData.lastPlayerName}");
            Log.Exception(e);
        }

        Log.Out($"[PlayerCommands] Saved player data {playerId}/{playerData.lastPlayerName}");
    }

    private static void Load()
    {
        Log.Out("[PlayerCommands] Loading data.");
        spawn = JsonConvert.DeserializeObject<Location?>(
            Utility.ReadAllText(GetGlobalDataPath("Spawn.json")) ?? "null");
        warps = JsonConvert.DeserializeObject<Dictionary<string, Location>>(
            Utility.ReadAllText(GetGlobalDataPath("Warps.json")) ?? "{}");
        Log.Out("[PlayerCommands] Loaded data.");
    }

    private static void Save()
    {
        Log.Out("[PlayerCommands] Saving data.");
        Utility.WriteAllText(GetGlobalDataPath("Spawn.json"), JsonConvert.SerializeObject(spawn));
        Utility.WriteAllText(GetGlobalDataPath("Warps.json"), JsonConvert.SerializeObject(warps));
        foreach (var keyValuePair in PlayerDataDict)
        {
            SavePlayerData(keyValuePair.Key, keyValuePair.Value);
        }

        Log.Out("[PlayerCommands] Saved data.");
    }

    private static void Cleanup()
    {
        spawn = null;
        warps = null;
        PlayerDataDict.Clear();
    }

    private static string GetGlobalDataPath(string fileName)
    {
        return GameIO.GetSaveGameDir() + "/PlayerCommands/" + fileName;
    }

    private static string GetPlayerDataPath(string fileName)
    {
        return GameIO.GetSaveGameDir() + "/PlayerCommands/PlayerData/" + fileName;
    }
}