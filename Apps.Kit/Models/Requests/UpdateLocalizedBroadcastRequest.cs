using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Kit.Models.Requests;

public class UpdateLocalizedBroadcastRequest
{
    [Display("Localized broadcast ID",
        Description = "ID of the existing localized Kit broadcast to update.")]
    public string BroadcastId { get; set; } = string.Empty;

    [Display("Translated content",
        Description = "Translated HTML or XLIFF file containing the updated broadcast.")]
    public FileReference Content { get; set; } = new();

    [Display("Target language",
        Description = "Language code of the localized broadcast, such as de or de-DE.")]
    public string TargetLanguage { get; set; } = string.Empty;
}
