namespace PlayerCommands.Commands;

public static class HelpCommand
{
    public static void Help(User sender, Command command, string label, string[] args)
    {
        sender.SendMessage(Message.GetArray("Help"));
    }
}