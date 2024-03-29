﻿using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PressenceHub : Hub
{
    private readonly PressenceTracker _tracker;
    public PressenceHub(PressenceTracker tracker)
    {
        _tracker = tracker;
    }
    public override async Task OnConnectedAsync()
    {
        await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

        var currentUsers = await _tracker.GetOnlineUsers();

        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }


    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }

}
