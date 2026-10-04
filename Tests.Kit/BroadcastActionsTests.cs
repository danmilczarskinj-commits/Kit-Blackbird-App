using Apps.Kit.Actions;
using Apps.Kit.Models.Requests;
using Newtonsoft.Json;
using Tests.Kit.Base;

namespace Tests.Kit;

[TestClass]
public class BroadcastActionsTests : TestBase
{
    [TestMethod]
    public async Task GetBroadcast_ValidId_ShouldReturnBroadcast()
    {
        var broadcastActions = new BroadcastActions(InvocationContext);
        var request = new BroadcastRequest
        {
            BroadcastId = "26025023"
        };

        var result = await broadcastActions.GetBroadcast(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.BroadcastId, result.Id);
        Console.WriteLine($"EmailTemplate: {result.EmailTemplate?.Id}");
        Console.WriteLine($"EmailTemplate: {result.EmailTemplate?.Name}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}
