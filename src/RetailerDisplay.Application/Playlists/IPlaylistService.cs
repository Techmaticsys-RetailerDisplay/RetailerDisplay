namespace RetailerDisplay.Application.Playlists;

public interface IPlaylistService
{
    Task<IReadOnlyList<PlaylistDto>> ListAsync(long retailerId, CancellationToken ct = default);
    Task<PlaylistDetailDto> GetAsync(long retailerId, long playlistId, CancellationToken ct = default);
    Task<PlaylistDto> CreateAsync(long retailerId, CreatePlaylistRequest request, CancellationToken ct = default);
    Task<PlaylistDto> UpdateAsync(long retailerId, long playlistId, UpdatePlaylistRequest request, CancellationToken ct = default);
    Task<PlaylistDetailDto> SetItemsAsync(long retailerId, long playlistId, SetPlaylistItemsRequest request, CancellationToken ct = default);
    Task DeleteAsync(long retailerId, long playlistId, CancellationToken ct = default);
}
