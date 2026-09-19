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
}