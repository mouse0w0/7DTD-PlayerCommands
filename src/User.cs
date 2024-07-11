using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCommands;

public class User
{
    public User(EntityPlayer entityPlayer, ClientInfo clientInfo, UserData userData)
    {
        EntityPlayer = entityPlayer;
        ClientInfo = clientInfo;
        UserData = userData;
    }

    // TODO Memory leak
    public EntityPlayer EntityPlayer { get; }
    public ClientInfo ClientInfo { get; }
    public UserData UserData { get; }

    public Location? PrevLocation { get; set; }
    public DateTime LastTeleportTime { get; set; }
    public TeleportRequestList TeleportRequestList { get; } = new();

    public int EntityId => EntityPlayer.entityId;
    public string EntityName => EntityPlayer.entityName;
    public string PlayerId => ClientInfo.GetPlayerId();
    public string PlayerName => EntityPlayer.entityName;
    public Vector3 Position => EntityPlayer.position;
    public Vector3 Rotation => EntityPlayer.rotation;
    public Location Location => EntityPlayer.GetLocation();

    public bool HasPermission(Command command) => ClientInfo.HasPermission(command.PermissionLevel);

    public bool HasPermission(int permissionLevel) => ClientInfo.HasPermission(permissionLevel);

    public void SendMessage(string message) => ClientInfo.SendMessage(message);

    public void SendMessage(IEnumerable<string> messages)
    {
        foreach (var message in messages)
        {
            SendMessage(message);
        }
    }

    public void Teleport(Location location) => ClientInfo.Teleport(location.Position, location.Rotation);

    public void Teleport(Vector3 position, Vector3? rotation = null) => ClientInfo.Teleport(position, rotation);

    public void Teleport(Entity target) => ClientInfo.Teleport(target.position, target.rotation);

    public void Save()
    {
        Log.Out($"[PlayerCommands] Saving user data {PlayerId}/{PlayerName}");
        UserData.LastPlayerName = PlayerName;
        try
        {
            UserData.Save();
        }
        catch (Exception e)
        {
            Log.Error($"[PlayerCommands] Error while saving user data {PlayerId}/{PlayerName}");
            Log.Exception(e);
        }

        Log.Out($"[PlayerCommands] Saved user data {PlayerId}/{PlayerName}");
    }
}