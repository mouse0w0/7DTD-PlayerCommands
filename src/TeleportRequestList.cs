using System;
using System.Collections.Generic;

namespace PlayerCommands;

public class TeleportRequestList
{
    private readonly List<TeleportRequest> _list = new();
    private readonly Dictionary<string, TeleportRequest> _playerNameDict = new();

    public int Count => _list.Count;

    public void Push(TeleportRequest request)
    {
        if (_playerNameDict.Remove(request.Requester.PlayerName, out var oldRequest))
        {
            _list.Remove(oldRequest);
        }

        _list.Add(request);
        _playerNameDict.Add(request.Requester.PlayerName, request);

        if (Config.TeleportMaxRequests > 0)
        {
            var needRemoved = _list.Count - Config.TeleportMaxRequests;
            if (needRemoved > 0)
            {
                for (var i = 0; i < needRemoved; i++)
                {
                    _playerNameDict.Remove(_list[i].Requester.PlayerName);
                }

                _list.RemoveRange(0, needRemoved);
            }
        }
    }

    public bool Pop(out TeleportRequest request)
    {
        var lastIndex = _list.Count - 1;
        if (lastIndex < 0)
        {
            request = null;
            return false;
        }

        var tempRequest = _list[lastIndex];
        if (Config.TeleportRequestTimeout > TimeSpan.Zero &&
            DateTime.Now - tempRequest.Time >= Config.TeleportRequestTimeout)
        {
            Clear();
            request = null;
            return false;
        }

        request = tempRequest;
        _list.RemoveAt(lastIndex);
        _playerNameDict.Remove(tempRequest.Requester.PlayerName);
        return true;
    }

    public bool Pop(string playerName, out TeleportRequest request)
    {
        foreach (var playerNameKey in _playerNameDict.Keys)
        {
            if (playerNameKey.EqualsCaseInsensitive(playerName))
            {
                _playerNameDict.Remove(playerNameKey, out var tempRequest);
                _list.Remove(tempRequest);

                if (Config.TeleportRequestTimeout > TimeSpan.Zero &&
                    DateTime.Now - tempRequest.Time >= Config.TeleportRequestTimeout)
                {
                    request = null;
                    return false;
                }

                request = tempRequest;
                return true;
            }
        }

        request = null;
        return false;
    }

    public bool PopFuzzily(string playerName, out TeleportRequest request)
    {
        int foundPlayerCount = 0;
        string foundPlayerName = null;
        foreach (var playerNameKey in _playerNameDict.Keys)
        {
            if (playerNameKey.EqualsCaseInsensitive(playerName))
            {
                _playerNameDict.Remove(playerNameKey, out var tempRequest);
                _list.Remove(tempRequest);

                if (Config.TeleportRequestTimeout > TimeSpan.Zero &&
                    DateTime.Now - tempRequest.Time >= Config.TeleportRequestTimeout)
                {
                    request = null;
                    return false;
                }

                request = tempRequest;
                return true;
            }

            if (playerNameKey.ContainsCaseInsensitive(playerName))
            {
                foundPlayerCount++;
                foundPlayerName = playerNameKey;
            }
        }

        if (foundPlayerCount == 1)
        {
            _playerNameDict.Remove(foundPlayerName, out var tempRequest);
            _list.Remove(tempRequest);

            if (Config.TeleportRequestTimeout > TimeSpan.Zero &&
                DateTime.Now - tempRequest.Time >= Config.TeleportRequestTimeout)
            {
                request = null;
                return false;
            }

            request = tempRequest;
            return true;
        }

        request = null;
        return false;
    }

    public void Clear()
    {
        _list.Clear();
        _playerNameDict.Clear();
    }
}