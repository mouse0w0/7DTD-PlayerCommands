namespace PlayerCommands.Command;

public static class HelpCommand
{
    public static void Help(User sender, string[] args)
    {
        if (sender.IsNoPermissionAndSendMessage("help"))
        {
            return;
        }
        
        sender.SendMessage(Message.GetArray("Help"));
    }
}