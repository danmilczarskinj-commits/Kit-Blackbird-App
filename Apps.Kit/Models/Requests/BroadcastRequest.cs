using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Requests;

public class BroadcastRequest
{
    [Display("Broadcast ID", Description = "ID of the Kit broadcast.")]
    public string BroadcastId { get; set; } = string.Empty;
}
