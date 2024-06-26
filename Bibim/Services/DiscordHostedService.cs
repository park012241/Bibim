using Bibim.Groups;
using Bibim.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Bibim.Services;

public class DiscordHostedService(
    ILogger<DiscordHostedService> logger,
    IServiceProvider serviceProvider,
    IOptions<DiscordOptions> options,
    InteractionService interactionService,
    DiscordSocketClient client)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var token = options.Value.Token;

        client.Ready += OnClientReady;
        client.Log += OnClientLog;
        interactionService.Log += OnClientLog;
        client.InteractionCreated += OnClientOnInteractionCreated;

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(-1, stoppingToken);
    }

    private async Task OnClientOnInteractionCreated(SocketInteraction x)
    {
        var ctx = new SocketInteractionContext(client, x);
        await interactionService.ExecuteCommandAsync(ctx, serviceProvider);
    }

    private Task OnClientLog(LogMessage arg)
    {
        logger.Log(arg.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException()
        }, "{message}", arg.Message);
        return Task.CompletedTask;
    }

    private async Task OnClientReady()
    {
        try
        {
            await interactionService.AddModuleAsync<AudioModule>(serviceProvider);
#if DEBUG
            foreach (var guild in client.Guilds) await interactionService.RegisterCommandsToGuildAsync(guild.Id);
#else
            await interactionService.RegisterCommandsGloballyAsync();
#endif
        }
        catch (Exception ex)
        {
            logger.LogError(ex, null);
        }
    }
}