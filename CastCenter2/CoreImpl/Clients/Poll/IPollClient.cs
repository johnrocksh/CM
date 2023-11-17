namespace CastManager.Client.Poll
{
    public interface IPollClient
    {
        Task<string> GetAsync(string endpointUrl);
    }
}
