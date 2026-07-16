using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Application.Playlists;

public record PlaylistDto(long PlaylistId, string Name, int Version, bool IsActive, int ItemCount);

public record PlaylistDetailDto(long PlaylistId, string Name, int Version, bool IsActive, IReadOnlyList<PlaylistItemDto> Items);

public record PlaylistItemDto(
    long PlaylistItemId,
    long ContentId,
    string ContentName,
    string ContentType,
    int SortOrder,
    int? DurationSeconds,
    string FitMode);

public record CreatePlaylistRequest(string Name);

public record UpdatePlaylistRequest(string Name);

public record PlaylistItemInput(long ContentId, int? DurationSeconds, FitMode FitMode);

public record SetPlaylistItemsRequest(IReadOnlyList<PlaylistItemInput> Items);
