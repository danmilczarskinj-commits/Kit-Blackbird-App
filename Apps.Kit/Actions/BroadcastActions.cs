using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Requests;
using Apps.Kit.Models.Responses;
using Apps.Kit.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using RestSharp;
using System.Text;

namespace Apps.Kit.Actions;

public class BroadcastActions(
    InvocationContext invocationContext,
    IFileManagementClient fileManagementClient)
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

    [Action("Create a broadcast",
    Description = "Create a new draft broadcast in Kit")]
    public async Task<BroadcastDto> CreateBroadcast(
    [ActionParameter] CreateBroadcastRequest input)
    {
        var html = await ReadHtmlFile(input.Content);

        var dto = new CreateBroadcastDto
        {
            Subject = input.Subject,
            Content = html,
            PreviewText = input.PreviewText,
            Description = input.Description
        };

        var request = new KitRequest("broadcasts", Method.Post, Creds);
        request.AddJsonBody(dto);

        var response =
            await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(request);

        if (response.Broadcast is null)
        {
            throw new PluginApplicationException(
                "Kit returned an empty broadcast response.");
        }

        return response.Broadcast;
    }

    [Action("Update a broadcast",
    Description = "Update an existing Kit broadcast as a draft")]
    public async Task<BroadcastDto> UpdateBroadcast(
    [ActionParameter] UpdateBroadcastRequest input)
    {
        var html = await ReadHtmlFile(input.Content);

        var dto = new UpdateBroadcastDto
        {
            Subject = input.Subject,
            Content = html,
            PreviewText = input.PreviewText,
            Description = input.Description
        };

        var endpoint = $"broadcasts/{input.BroadcastId}";
        var request = new KitRequest(endpoint, Method.Put, Creds);
        request.AddJsonBody(dto);

        var response =
            await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(request);

        if (response.Broadcast is null)
        {
            throw new PluginApplicationException(
                "Kit returned an empty broadcast response.");
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

    [Action("Download broadcast HTML",
    Description = "Download a Kit broadcast's content as an HTML file")]
    public async Task<DownloadBroadcastResponse> DownloadBroadcast(
    [ActionParameter] BroadcastRequest input)
    {
        var broadcast = await GetBroadcast(input);

        if (broadcast.Content is null)
        {
            throw new PluginApplicationException(
                "This broadcast has no HTML content.");
        }

        var bytes = Encoding.UTF8.GetBytes(broadcast.Content);
        using var stream = new MemoryStream(bytes);

        var file = await fileManagementClient.UploadAsync(
            stream,
            "text/html",
            $"broadcast-{broadcast.Id}.html");

        return new DownloadBroadcastResponse
        {
            Content = file
        };
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

    private async Task<string> ReadHtmlFile(FileReference content)
    {
        var file = await fileManagementClient.DownloadAsync(content);
        var bytes = await file.GetByteData();

        return Encoding.UTF8.GetString(bytes);
    }
}
