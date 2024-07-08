namespace PlayerCommands.Command;

public static class SuicideCommand
{
    public static readonly DamageSource suicide = new(EnumDamageSource.Internal, EnumDamageTypes.Suicide);

    public static void Suicide(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("suicide"))
        {
            return;
        }
        
        sender.EntityPlayer.DamageEntity(suicide, int.MaxValue, false);
    }
}