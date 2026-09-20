using Apps.Kit.DataSourceHandlers.EnumDataHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.Models.Requests;

public class SearchSubscribersRequest
{
    [Display("Status"), DataSource(typeof(SubscriberStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Email Address")]
    public string? EmailAddress { get; set; }

    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }
}