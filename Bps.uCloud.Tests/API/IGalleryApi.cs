namespace Bps.uCloud.Contracts.API
{
    using Bps.uCloud.Contracts.Entities;
    using Refit;
    using System.Threading.Tasks;

    public interface IGalleryApi
    {
        [Get("/gallery")]
        Task<Gallery[]> ListAsync();

        [Post("/gallery")]
        Task<string> NewAsync([Body] Gallery item);

        [Get("/gallery/{id}")]
        Task<Gallery> ViewAsync(string id);

        [Delete("/gallery/{id}")]
        Task<string> RemoveAsync(string id);

        //[Put("/gallery/{id}")]
        //TODO Task UpdateAsync(string id, [Body] Gallery item);

    }
}