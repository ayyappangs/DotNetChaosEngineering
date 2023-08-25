namespace DonetChaosEngineering.Service
{
    public class ChaosService : IChaosService
    {
        private readonly HttpClient _client;

        public ChaosService(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> MakeApiCall()
        {
            return await _client.GetAsync("/externalendpoint");
        }
    }

    public interface IChaosService
    {
        Task<HttpResponseMessage> MakeApiCall();
    }
}
