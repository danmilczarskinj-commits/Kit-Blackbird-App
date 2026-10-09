using Apps.Kit.Actions;
using Apps.Kit.Models.Requests;
using Newtonsoft.Json;
using Tests.Kit.Base;
using Blackbird.Applications.Sdk.Common.Files;

namespace Tests.Kit;

[TestClass]
public class BroadcastActionsTests : TestBase
{
    [TestMethod]
    public async Task GetBroadcast_ValidId_ShouldReturnBroadcast()
    {
        var broadcastActions = new BroadcastActions(InvocationContext, null!);
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

    [TestMethod]
    public async Task SearchBroadcasts_ShouldReturnBroadcasts()
    {
        var broadcastActions = new BroadcastActions(InvocationContext, null!);

        var request = new SearchBroadcastsRequest
        {
            Status = "completed"
        };

        var result = await broadcastActions.SearchBroadcasts(request);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);

        Console.WriteLine($"Broadcasts returned: {result.Count}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task CreateBroadcast_ShouldCreateDraft()
    {
        var broadcastActions = new BroadcastActions(InvocationContext, FileManager);

        var request = new CreateBroadcastRequest
        {
            Subject = "Blackbird Integration Test",
            PreviewText = "Testing the Kit API",
            Description = "Created during Blackbird Dev Lab",
            Content = new FileReference
            {
                Name = "test-broadcast.html",
                ContentType = "text/html"
            }
        };

        var result = await broadcastActions.CreateBroadcast(request);

        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result.Id));
        Assert.AreEqual("draft", result.Status);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task DownloadBroadcast_ShouldReturnHtmlFile()
    {
        var broadcastActions = new BroadcastActions(InvocationContext, FileManager);

        var request = new BroadcastRequest
        {
            BroadcastId = "26025023"
        };

        var result = await broadcastActions.DownloadBroadcast(request);

        Assert.IsNotNull(result.Content);
        Assert.AreEqual("text/html", result.Content.ContentType);

        var html = FileManager.ReadOutputAsString(result.Content);

        Assert.IsFalse(string.IsNullOrWhiteSpace(html));

        Console.WriteLine($"Downloaded file: {result.Content.Name}");
        Console.WriteLine(html);
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task UpdateBroadcast_ShouldUpdateDraft()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);

        var input = new UpdateBroadcastRequest
        {
            BroadcastId = "26301241",
            Subject = "Updated broadcast test",
            PreviewText = "Testing the Kit update action",
            Description = "Updated through Blackbird",
            Content = new FileReference
            {
                Name = "test-broadcast.html"
            }
        };

        var result = await actions.UpdateBroadcast(input);

        Assert.AreEqual(input.BroadcastId, result.Id);
        Assert.AreEqual(input.Subject, result.Subject);
        Assert.AreEqual(false, result.Public);

        Console.WriteLine($"Updated broadcast: {result.Id}");
        Console.WriteLine($"Status: {result.Status}");
    }
}
