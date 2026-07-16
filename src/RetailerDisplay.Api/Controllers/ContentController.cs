using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Content;
using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/content")]
public class ContentController : ControllerBase
{
    private readonly IContentService _content;
    private readonly ICurrentUser _currentUser;

    public ContentController(IContentService content, ICurrentUser currentUser)
    {
        _content = content;
        _currentUser = currentUser;
    }

    private long RetailerId => _currentUser.RequireRetailerId();

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContentDto>>> List([FromQuery] ContentType? type, CancellationToken ct)
        => Ok(await _content.ListAsync(RetailerId, type, ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ContentDto>> Get(long id, CancellationToken ct)
        => Ok(await _content.GetAsync(RetailerId, id, ct));

    [HttpPost]
    public async Task<ActionResult<ContentDto>> Create(CreateContentRequest request, CancellationToken ct)
        => Ok(await _content.CreateAsync(RetailerId, request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ContentDto>> Update(long id, UpdateContentRequest request, CancellationToken ct)
        => Ok(await _content.UpdateAsync(RetailerId, id, request, ct));

    [HttpPut("{id:long}/products")]
    public async Task<IActionResult> SetProducts(long id, SetContentProductsRequest request, CancellationToken ct)
    {
        await _content.SetProductsAsync(RetailerId, id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _content.DeleteAsync(RetailerId, id, ct);
        return NoContent();
    }
}
