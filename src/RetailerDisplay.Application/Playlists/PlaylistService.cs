using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Application.Playlists;

public class PlaylistService : IPlaylistService
{
    private readonly IApplicationDbContext _db;

    public PlaylistService(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<PlaylistDto>> ListAsync(long retailerId, CancellationToken ct = default)
    {
        return await _db.Playlists
            .Where(p => p.RetailerId == retailerId)
            .OrderBy(p => p.Name)
            .Select(p => new PlaylistDto(p.PlaylistId, p.Name, p.Version, p.IsActive, p.Items.Count))
            .ToListAsync(ct);
    }

    public async Task<PlaylistDetailDto> GetAsync(long retailerId, long playlistId, CancellationToken ct = default)
    {
        var playlist = await Find(retailerId, playlistId, ct);
        var items = await _db.PlaylistItems
            .Where(i => i.PlaylistId == playlistId)
            .OrderBy(i => i.SortOrder)
            .Select(i => new PlaylistItemDto(
                i.PlaylistItemId, i.ContentId, i.Content.Name, i.Content.ContentType.ToString(),
                i.SortOrder, i.DurationSeconds, i.FitMode.ToString()))
            .ToListAsync(ct);

        return new PlaylistDetailDto(playlist.PlaylistId, playlist.Name, playlist.Version, playlist.IsActive, items);
    }

    public async Task<PlaylistDto> CreateAsync(long retailerId, CreatePlaylistRequest r, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var playlist = new Playlist
        {
            RetailerId = retailerId,
            Name = r.Name.Trim(),
            Version = 1,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Playlists.Add(playlist);
        await _db.SaveChangesAsync(ct);
        return new PlaylistDto(playlist.PlaylistId, playlist.Name, playlist.Version, playlist.IsActive, 0);
    }

    public async Task<PlaylistDto> UpdateAsync(long retailerId, long playlistId, UpdatePlaylistRequest r, CancellationToken ct = default)
    {
        var playlist = await Find(retailerId, playlistId, ct);
        playlist.Name = r.Name.Trim();
        playlist.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var count = await _db.PlaylistItems.CountAsync(i => i.PlaylistId == playlistId, ct);
        return new PlaylistDto(playlist.PlaylistId, playlist.Name, playlist.Version, playlist.IsActive, count);
    }

    public async Task<PlaylistDetailDto> SetItemsAsync(long retailerId, long playlistId, SetPlaylistItemsRequest r, CancellationToken ct = default)
    {
        var playlist = await Find(retailerId, playlistId, ct);

        var contentIds = r.Items.Select(i => i.ContentId).Distinct().ToList();
        var validContent = await _db.Contents
            .Where(c => c.RetailerId == retailerId && contentIds.Contains(c.ContentId))
            .Select(c => c.ContentId)
            .ToListAsync(ct);
        var validSet = validContent.ToHashSet();

        var existing = await _db.PlaylistItems.Where(i => i.PlaylistId == playlistId).ToListAsync(ct);
        _db.PlaylistItems.RemoveRange(existing);

        var now = DateTime.UtcNow;
        var order = 0;
        foreach (var item in r.Items)
        {
            if (!validSet.Contains(item.ContentId))
                throw AppException.NotFound($"Content {item.ContentId}");

            _db.PlaylistItems.Add(new PlaylistItem
            {
                PlaylistId = playlistId,
                ContentId = item.ContentId,
                SortOrder = order++,
                DurationSeconds = item.DurationSeconds,
                FitMode = item.FitMode,
                CreatedAt = now
            });
        }

        playlist.Version += 1;   // sync signal for devices
        playlist.UpdatedAt = now;
        await _db.SaveChangesAsync(ct);

        return await GetAsync(retailerId, playlistId, ct);
    }

    public async Task DeleteAsync(long retailerId, long playlistId, CancellationToken ct = default)
    {
        var playlist = await Find(retailerId, playlistId, ct);
        _db.Playlists.Remove(playlist);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Playlist> Find(long retailerId, long playlistId, CancellationToken ct) =>
        await _db.Playlists.FirstOrDefaultAsync(p => p.RetailerId == retailerId && p.PlaylistId == playlistId, ct)
            ?? throw AppException.NotFound("Playlist");
}
