using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Requests;
using Apps.Kit.Models.Responses;
using Apps.Kit.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Kit.Actions;

[ActionList("Subscribers")]
public class SubscriberActions(InvocationContext invocationContext) : KitInvocable(invocationContext)
{
    [Action("Get subscriber",
            Description = "Get a subscriber by ID")]
    public async Task<SubscriberDto> GetSubscriber([ActionParameter] SubscriberRequest input)
    {
        var endpoint = $"subscribers/{input.SubscriberId}";
        var request = new KitRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<GetSubscriberResponse>(request);

        if (response.Subscriber is null)
        {
            throw new PluginApplicationException("Kit returned an empty subscriber response.");
        }

        return response.Subscriber;
    }
}