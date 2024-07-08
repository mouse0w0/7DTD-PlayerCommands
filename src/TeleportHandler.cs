using System;
using UnityEngine;

namespace PlayerCommands;

public static class TeleportHandler
{
    public static bool Teleport(User user, Vector3 position, Vector3? rotation = null)
    {
        var now = DateTime.Now;
        var passed = now - user.LastTeleportTime;
        var remaining = Config.TeleportCooldown - passed;
        if (remaining > TimeSpan.Zero)
        {
            user.SendMessage(Message.Get("Tp.Cooldown").Format((int)remaining.TotalSeconds));
            return false;
        }

        user.Teleport(position, rotation);
        user.LastTeleportTime = now;
        return true;
    }

    public static bool Teleport(User user, Location location) =>
        Teleport(user, location.Position, location.Rotation);

    public static bool Teleport(User user, Entity entity) =>
        Teleport(user, entity.position, entity.rotation);

    public static bool Teleport(User user, User target) =>
        Teleport(user, target.Position, target.Rotation);
}