namespace Bps.uCloud.Contracts.API
{
    using Refit;
    using System.Threading.Tasks;

    public interface ICommonApi
    {
        [Get("/hello")]
        Task<string> HelloAsync();

        [Get("/gallery/{gallery}/items/{item}/detect")]
        Task<string> DetectAsync(string gallery, string item);

        [Post("/enroll")]
        Task<string> EnrollAsync();

        [Post("/identify")]
        Task<string> IdentifyAsync();
    }
}