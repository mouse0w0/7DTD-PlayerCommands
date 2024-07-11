namespace PlayerCommands.Commands;

public static class SuicideCommand
{
    public static readonly DamageSource suicide = new(EnumDamageSource.Internal, EnumDamageTypes.Suicide);

    public static void Suicide(User sender, Command command, string label, string[] args)
    {
        sender.EntityPlayer.DamageEntity(suicide, int.MaxValue, false);
    }
}