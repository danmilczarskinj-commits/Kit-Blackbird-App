using Apps.Kit.Actions;
using Apps.Kit.Models.Requests;
using Newtonsoft.Json;
using Tests.Kit.Base;
using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Tests.Kit;

[TestClass]
public class SubscriberActionsTests : TestBase
{
    [TestMethod]
    public async Task GetSubscriber_ValidId_ShouldReturnSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);
        var request = new SubscriberRequest
        {
            SubscriberId = "4297863889"
        };

        var result = await subscriberActions.GetSubscriber(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.IsFalse(string.IsNullOrEmpty(result.EmailAddress));
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task SearchSubscribers_ShouldReturnSubscribers()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new SearchSubscribersRequest
        {
            Status = "active",
            EmailAddress = "tobetranslated01@gmail.com",
            CreatedAfter = new DateTime(2026, 9, 15)
        };

        var result = await subscriberActions.SearchSubscribers(request);

        Assert.IsNotNull(result.Subscribers);
        Assert.IsTrue(result.Subscribers.Count > 0);
        Assert.AreEqual(
            request.EmailAddress,
            result.Subscribers.First().EmailAddress);
        Assert.IsNotNull(result.Pagination);
        Console.WriteLine($"Has next page: {result.Pagination.HasNextPage}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Console.WriteLine($"Total subscribers returned: {result.Subscribers.Count}");
    }

    [TestMethod]
    public async Task CreateSubscriber_ValidInput_ShouldCreateSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new CreateSubscriberRequest
        {
            EmailAddress = "danmilczarskinj@gmail.com",
            FirstName = "Blackbird Test Updated",
            State = "inactive"
        };

        var result = await subscriberActions.CreateSubscriber(request);
        Assert.IsNotNull(result);
        Assert.AreEqual(request.EmailAddress, result.EmailAddress);
        Assert.AreEqual(request.State, result.State);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task UpdateSubscriber_ValidInput_ShouldUpdateSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new UpdateSubscriberRequest
        {
            SubscriberId = "4303366221",
            EmailAddress = "danmilczarskinj@gmail.com",
            FirstName = "Blackbird Test Updated"
        };

        var result = await subscriberActions.UpdateSubscriber(request);
        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.AreEqual(request.EmailAddress, result.EmailAddress);
        Assert.AreEqual(request.FirstName, result.FirstName);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task UpdateSubscriber_WithoutEmail_ShouldUpdateSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new UpdateSubscriberRequest
        {
            SubscriberId = "4303366221",
            FirstName = "Blackbird No Email Test"
        };

        var result = await subscriberActions.UpdateSubscriber(request);
        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.AreEqual(request.FirstName, result.FirstName);
        Assert.IsFalse(string.IsNullOrEmpty(result.EmailAddress));

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task CustomFieldDataHandler_ShouldReturnCustomFields()
    {
        var handler = new CustomFieldDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(
            new DataSourceContext(),
            CancellationToken.None);

        var fields = result.ToList();

        Assert.IsTrue(fields.Count > 0);

        foreach (var field in fields)
        {
            Console.WriteLine($"{field.DisplayName} -> {field.Value}");
        }
    }

    [TestMethod]
    public async Task UpdateSubscriberCustomField_ValidInput_ShouldUpdateCustomField()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new UpdateSubscriberCustomFieldRequest
        {
            SubscriberId = "4303366221",
            CustomField = "role",
            Value = "Solutions",
        };

        var result = await subscriberActions.UpdateSubscriberCustomField(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.IsNotNull(result.Fields);
        Assert.AreEqual(
            request.Value,
            result.Fields[request.CustomField]);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}
