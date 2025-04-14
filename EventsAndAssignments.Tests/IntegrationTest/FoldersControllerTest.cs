using NUnit.Framework;

namespace EventsAndAssignments.Tests.IntegrationTest
{
    public class FoldersControllerTest
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:7293/Folders/");
        }

        [Test]
        public async Task GetFolderProtocolsReturnOk()
        {
            HttpResponseMessage response = await _client.GetAsync("GetFolder?id=1");
            Assert.That(response.IsSuccessStatusCode, Is.True);
        }
    }
}