using FluentValidation;

namespace RetailerDisplay.Application.Playlists;

public class CreatePlaylistRequestValidator : AbstractValidator<CreatePlaylistRequest>
{
    public CreatePlaylistRequestValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
}

public class UpdatePlaylistRequestValidator : AbstractValidator<UpdatePlaylistRequest>
{
    public UpdatePlaylistRequestValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
}

public class SetPlaylistItemsRequestValidator : AbstractValidator<SetPlaylistItemsRequest>
{
    public SetPlaylistItemsRequestValidator()
    {
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ContentId).GreaterThan(0);
            item.RuleFor(i => i.DurationSeconds).GreaterThan(0).When(i => i.DurationSeconds.HasValue);
            item.RuleFor(i => i.FitMode).IsInEnum();
        });
    }
}
