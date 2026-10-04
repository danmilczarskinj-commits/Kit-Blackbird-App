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

public class BroadcastActions(InvocationContext invocationContext)
    : KitInvocable(invocationContext)
{
    [Action("Get a broadcast",
           Description = "Get details about a broadcast")]
    public async Task<BroadcastDto> GetBroadcast([ActionParameter] BroadcastRequest input)
    {
        var endpoint = $"broadcasts/{input.BroadcastId}";
        var request = new KitRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(request);

        if (response.Broadcast is null)
        {
            throw new PluginApplicationException("Kit returned an empty broadcast response.");
        }

        return response.Broadcast;
    }
}
