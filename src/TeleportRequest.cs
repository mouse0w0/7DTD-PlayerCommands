using System;

namespace PlayerCommands;

public class TeleportRequest
{
    public TeleportRequest(User requester, bool here = false)
    {
        Requester = requester;
        Time = DateTime.Now;
        Here = here;
    }

    public User Requester { get; }
    public DateTime Time { get; }
    public bool Here { get; }
}