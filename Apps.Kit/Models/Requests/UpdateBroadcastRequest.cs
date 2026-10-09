using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Kit.Models.Requests;

public class UpdateBroadcastRequest
{
    [Display("Broadcast ID")]
    public string BroadcastId { get; set; } = string.Empty;

    [Display("Subject")]
    public string Subject { get; set; } = string.Empty;

    [Display("Content")]
    public FileReference Content { get; set; } = new();

    [Display("Preview text")]
    public string? PreviewText { get; set; }

    [Display("Description")]
    public string? Description { get; set; }
}
