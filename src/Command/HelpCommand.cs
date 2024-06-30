namespace PlayerCommands.Command;

public static class HelpCommand
{
    public static void Help(CommandSender sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("help"))
        {
            return;
        }
        
        foreach (var s in Message.GetArray("Help"))
        {
            sender.SendMessage(s);
        }
    }
}