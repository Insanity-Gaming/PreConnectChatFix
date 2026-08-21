using Microsoft.Extensions.Configuration;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;

namespace PreConnectChatFix;

public sealed class PreConnectChatFix : IModSharpModule, IClientListener
{
    private readonly IClientManager _clients;

    public PreConnectChatFix(
        ISharedSystem sharedSystem,
        string dllPath,
        string sharpPath,
        Version version,
        IConfiguration coreConfiguration,
        bool hotReload)
    {
        _clients = sharedSystem.GetClientManager();
    }

    public bool Init()
    {
        _clients.InstallClientListener(this);
        Console.WriteLine("[PreConnectChatFix] Loaded.");
        return true;
    }

    public void Shutdown()
    {
        _clients.RemoveClientListener(this);
        Console.WriteLine("[PreConnectChatFix] Unloaded.");
    }

    public ECommandAction OnClientSayCommand(
        IGameClient client,
        bool teamOnly,
        bool isCommand,
        string commandName,
        string message)
    {
        if (client.IsInGame)
            return ECommandAction.Skipped;

        Console.WriteLine($"[PreConnectChatFix] Blocked pre-connect chat from slot {client.Slot}.");
        return ECommandAction.Stopped;
    }

    int IClientListener.ListenerVersion => IClientListener.ApiVersion;
    int IClientListener.ListenerPriority => 100;

    public string DisplayName => "Pre-Connect Chat Fix";
    public string DisplayAuthor => "Insanity Gaming";
}
