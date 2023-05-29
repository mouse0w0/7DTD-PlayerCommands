namespace PlayerCommands.Command;

public static class HelpCommand
{
    public static void Help(CommandSender sender, string[] args)
    {
        if (!sender.HasPermission("help"))
        {
            sender.SendMessage("[FF5555]没有足够的权限");
            return;
        }
        
        sender.SendMessage("[FFAA00]/home - 回家");
        sender.SendMessage("[FFAA00]/sethome - 设置家");
        sender.SendMessage("[FFAA00]/back - 返回上一地点");
        sender.SendMessage("[FFAA00]/spawn - 传送到出生点");
        sender.SendMessage("[FFAA00]/warp <地标> - 传送到地标");
        sender.SendMessage("[FFAA00]/tp <玩家> - 请求传送到玩家");
        sender.SendMessage("[FFAA00]/tph <玩家> - 请求玩家传送到你");
    }
}