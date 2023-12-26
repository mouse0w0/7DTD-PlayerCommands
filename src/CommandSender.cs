namespace PlayerCommands;

public struct CommandSender
{
    public readonly int entityId;
    public readonly string entityName;
    public readonly EntityPlayer entityPlayer;
    public readonly ClientInfo clientInfo;

    public CommandSender(int entityId, string entityName, ClientInfo clientInfo)
    {
        this.entityId = entityId;
        this.entityName = entityName;
        this.entityPlayer = Utility.GetEntityPlayer(entityId);
        this.clientInfo = clientInfo;
    }

    public string PlayerId => clientInfo.GetPlayerId();
    public Location Location => entityPlayer.GetLocation();

    public bool HasPermission(string permission) => clientInfo.HasPermission(permission);

    public void SendMessage(string message) => clientInfo.SendMessage(message);

    public void Teleport(Location location) => clientInfo.Teleport(location);
}