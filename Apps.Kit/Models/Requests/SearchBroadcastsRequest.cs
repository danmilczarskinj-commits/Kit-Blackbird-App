using Apps.Kit.DataSourceHandlers.EnumDataHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.Models.Requests;

public class SearchBroadcastsRequest
{
    [Display("Status"), DataSource(typeof(BroadcastStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Sent after")]
    public DateTime? SentAfter { get; set; }

    [Display("Sent before")]
    public DateTime? SentBefore { get; set; }
}
