using Apps.Kit.Actions;
using Apps.Kit.Models.Requests;
using Newtonsoft.Json;
using Tests.Kit.Base;

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
}