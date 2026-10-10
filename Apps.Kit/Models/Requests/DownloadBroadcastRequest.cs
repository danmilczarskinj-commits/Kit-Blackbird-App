using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Requests;

public class DownloadBroadcastRequest
{
    [Display("Broadcast ID")]
    public string BroadcastId { get; set; } = string.Empty;

    [Display("Source language")]
    public string SourceLanguage { get; set; } = string.Empty;
}
