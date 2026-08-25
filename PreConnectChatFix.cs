using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;

namespace PreConnectChatFix;

public sealed class PreConnectChatFix : IModSharpModule, IClientListener
{
    private readonly IClientManager _clients;
    private readonly ILogger<PreConnectChatFix> _logger;

    public PreConnectChatFix(
        ISharedSystem sharedSystem,
        string dllPath,
        string sharpPath,
        Version version,
        IConfiguration coreConfiguration,
        bool hotReload)
    {
        _clients = sharedSystem.GetClientManager();
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<PreConnectChatFix>();
    }

    public bool Init()
    {
        _clients.InstallClientListener(this);
        _logger.LogInformation("PreConnectChatFix loaded");
        return true;
    }

    public void Shutdown()
    {
        _clients.RemoveClientListener(this);
        _logger.LogInformation("PreConnectChatFix unloaded");
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

        if (!client.IsValid)
        {
            _logger.LogWarning("Blocked chat from an invalid or not-yet-connected client");
            return ECommandAction.Stopped;
        }

        if (!client.IsAuthenticated)
        {
            _logger.LogWarning(
                "Blocked pre-connect chat from {Name} in slot {Slot}; SteamID is not authenticated",
                client.Name,
                client.Slot);
            return ECommandAction.Stopped;
        }

        _logger.LogWarning(
            "Blocked pre-connect chat from {Name} ({SteamId}) in slot {Slot}",
            client.Name,
            client.SteamId,
            client.Slot);

        return ECommandAction.Stopped;
    }

    int IClientListener.ListenerVersion => IClientListener.ApiVersion;
    int IClientListener.ListenerPriority => 100;

    public string DisplayName => "Pre-Connect Chat Fix";
    public string DisplayAuthor => "Insanity Gaming";
}
