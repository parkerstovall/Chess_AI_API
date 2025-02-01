using api.models.client;
using api.repository;
using ChessApi.Models.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace api.BackgroundJobs;

public class DeactivateGame : IHostedService, IDisposable
{
    private readonly IMemoryCache _cache;
    private Timer? _timer = null;

    public DeactivateGame(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(RemoveOldGames, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        return Task.CompletedTask;
    }

    private void RemoveOldGames(object? state)
    {
        if (!_cache.TryGetValue("OpenGames", out List<OpenGame>? openGames) || openGames is null)
        {
            return;
        }

        List<OpenGame> newOpenGames = new();
        DateTime minTime = DateTime.Now.AddMinutes(-1);
        foreach (OpenGame game in openGames)
        {
            if (game.LastPing > minTime)
            {
                newOpenGames.Add(game);
            }
            else
            {
                _cache.Remove($"Board:{game.GameID}");
                _cache.Remove($"Turn:{game.GameID}");
                _cache.Remove($"Moves:{game.GameID}");
                _cache.Remove($"SelectedSquare:{game.GameID}");
                _cache.Remove($"Check:{game.GameID}");
            }
        }

        _cache.Set("OpenGames", newOpenGames);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer?.Dispose();
    }
}
