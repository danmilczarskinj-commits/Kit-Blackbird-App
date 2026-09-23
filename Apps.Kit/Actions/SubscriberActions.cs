using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Requests;
using Apps.Kit.Models.Responses;
using Apps.Kit.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
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

    [Action("Create subscriber",
        Description = "Create a new subscriber or update an existing subscriber with the same email address")]
    public async Task<SubscriberDto> CreateSubscriber(
        [ActionParameter] CreateSubscriberRequest input)
    {
        var endpoint = "subscribers";
        var dto = new CreateSubscriberDto
        {
            EmailAddress = input.EmailAddress,
            FirstName = input.FirstName,
            State = input.State
        };
        var request = new KitRequest(endpoint, Method.Post, Creds).WithJsonBody(dto);

        var response = await Client.ExecuteWithErrorHandling<GetSubscriberResponse>(request);

        if (response.Subscriber is null)
        {
            throw new PluginApplicationException("Kit was unable to create or update the subscriber.");
        }

        return response.Subscriber;
    }

    [Action("Update subscriber",
        Description = "Update an existing subscriber")]
    public async Task<SubscriberDto> UpdateSubscriber(
        [ActionParameter] UpdateSubscriberRequest input)
    {
        var endpoint = $"subscribers/{input.SubscriberId}";
        var emailAddress = input.EmailAddress;

        if (string.IsNullOrEmpty(emailAddress))
        {
            var getRequest = new KitRequest(
                $"subscribers/{input.SubscriberId}",
                Method.Get,
                Creds);

            var getResponse =
                await Client.ExecuteWithErrorHandling<GetSubscriberResponse>(getRequest);

            if (getResponse.Subscriber is null)
            {
                throw new PluginApplicationException(
                    "Kit returned an empty subscriber response.");
            }

            emailAddress = getResponse.Subscriber.EmailAddress;
        }

        var dto = new UpdateSubscriberDto
        {
            EmailAddress = emailAddress,
            FirstName = input.FirstName
        };
        var request = new KitRequest(endpoint, Method.Put, Creds).WithJsonBody(dto);

        var response = await Client.ExecuteWithErrorHandling<GetSubscriberResponse>(request);

        if (response.Subscriber is null)
        {
            throw new PluginApplicationException("Kit was unable to update the subscriber.");
        }

        return response.Subscriber;
    }

    [Action("Update subscriber custom field",
        Description = "Update a custom field on an existing subscriber")]
    public async Task<SubscriberDto> UpdateSubscriberCustomField(
        [ActionParameter] UpdateSubscriberCustomFieldRequest input)
    {
        var endpoint = $"subscribers/{input.SubscriberId}";

        // Kit requires an email address when updating a subscriber.
        var getRequest = new KitRequest(endpoint, Method.Get, Creds);

        var getResponse =
            await Client.ExecuteWithErrorHandling<GetSubscriberResponse>(getRequest);

        if (getResponse.Subscriber is null)
        {
            throw new PluginApplicationException(
                "Kit returned an empty subscriber response.");
        }

        var dto = new UpdateSubscriberDto
        {
            EmailAddress = getResponse.Subscriber.EmailAddress,
            Fields = new Dictionary<string, string>
            {
                { input.CustomField, input.Value }
            }
        };

        var request = new KitRequest(endpoint, Method.Put, Creds)
            .WithJsonBody(dto);

        var response =
            await Client.ExecuteWithErrorHandling<GetSubscriberResponse>(request);

        if (response.Subscriber is null)
        {
            throw new PluginApplicationException(
                "Kit was unable to update the subscriber custom field.");
        }

        return response.Subscriber;
    }

    [Action("Search subscribers",
            Description = "Search for subscribers")]
    public async Task<SearchSubscribersResponse> SearchSubscribers([ActionParameter] SearchSubscribersRequest input)
    {
        var endpoint = "subscribers";
        var request = new KitRequest(endpoint, Method.Get, Creds);

        AddSearchParameters(request, input);

        var response = await Client.ExecuteWithErrorHandling<SearchSubscribersResponse>(request);
        var subscribers = response.Subscribers.ToList();

        while (response.Pagination?.HasNextPage == true &&
            response.Pagination?.EndCursor is not null)
        {
            var nextRequest = new KitRequest(endpoint, Method.Get, Creds);

            AddSearchParameters(nextRequest, input);

            nextRequest.AddQueryParameter(
                "after",
                response.Pagination.EndCursor);

            var nextResponse = await Client.ExecuteWithErrorHandling<SearchSubscribersResponse>(nextRequest);
            subscribers.AddRange(nextResponse.Subscribers);
            response = nextResponse;
        }

        response.Subscribers = subscribers;
        return response;
    }

    private void AddSearchParameters(KitRequest request, SearchSubscribersRequest input)
    {
        if (!string.IsNullOrEmpty(input.Status))
        {
            request.AddQueryParameter("status", input.Status);
        }

        if (!string.IsNullOrEmpty(input.EmailAddress))
        {
            request.AddQueryParameter("email_address", input.EmailAddress);
        }

        if (input.CreatedAfter.HasValue)
        {
            request.AddQueryParameter("created_after", input.CreatedAfter.Value.ToString("yyyy-MM-dd"));
        }

        if (input.CreatedBefore.HasValue)
        {
            request.AddQueryParameter("created_before", input.CreatedBefore.Value.ToString("yyyy-MM-dd"));
        }

        if (input.UpdatedAfter.HasValue)
        {
            request.AddQueryParameter("updated_after", input.UpdatedAfter.Value.ToString("yyyy-MM-dd"));
        }

        if (input.UpdatedBefore.HasValue)
        {
            request.AddQueryParameter("updated_before", input.UpdatedBefore.Value.ToString("yyyy-MM-dd"));
        }
    }
}
