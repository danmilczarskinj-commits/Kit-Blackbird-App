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

    [Action("Delete a broadcast",
            Description = "Delete a specified Kit broadcast")]
    public async Task DeleteBroadcast([ActionParameter] BroadcastRequest input)
    {
        var endpoint = $"broadcasts/{input.BroadcastId}";
        var request = new KitRequest(endpoint, Method.Delete, Creds);

        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Search broadcasts",
        Description = "Search available Kit broadcasts")]
    public async Task<List<BroadcastDto>> SearchBroadcasts(
        [ActionParameter] SearchBroadcastsRequest input)
    {
        var endpoint = "broadcasts";
        var request = new KitRequest(endpoint, Method.Get, Creds);

        AddSearchParameters(request, input);

        var response =
            await Client.ExecuteWithErrorHandling<SearchBroadcastsResponse>(request);

        var broadcasts = response.Broadcasts.ToList();

        while (response.Pagination?.HasNextPage == true &&
               response.Pagination.EndCursor is not null)
        {
            var nextRequest = new KitRequest(endpoint, Method.Get, Creds);

            AddSearchParameters(nextRequest, input);

            nextRequest.AddQueryParameter(
                "after",
                response.Pagination.EndCursor);

            response =
                await Client.ExecuteWithErrorHandling<SearchBroadcastsResponse>(nextRequest);

            broadcasts.AddRange(response.Broadcasts);
        }

        return broadcasts;
    }

    private void AddSearchParameters(
        KitRequest request,
        SearchBroadcastsRequest input)
    {
        request.AddQueryParameter("per_page", 1000);
        request.AddQueryParameter("slim", true);

        if (!string.IsNullOrEmpty(input.Status))
        {
            request.AddQueryParameter("status", input.Status);
        }

        if (input.SentAfter.HasValue)
        {
            request.AddQueryParameter(
                "sent_after",
                input.SentAfter.Value.ToString("yyyy-MM-dd"));
        }

        if (input.SentBefore.HasValue)
        {
            request.AddQueryParameter(
                "sent_before",
                input.SentBefore.Value.ToString("yyyy-MM-dd"));
        }
    }
}
