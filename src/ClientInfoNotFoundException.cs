using System;

namespace PlayerCommands;

public class ClientInfoNotFoundException : Exception
{
    public ClientInfoNotFoundException(string message) : base(message)
    {
    }
}