namespace Bps.uCloud.Contracts.API
{
    using Bps.uCloud.Contracts.Entities;
    using Refit;
    using System.IO;
    using System.Threading.Tasks;

    public interface IItemsApi
    {
        [Get("/gallery/{gallery}/items")]
        Task<Item[]> ListAsync(string gallery);

        [Multipart]
        [Post("/gallery/{gallery}/items/{item}")]
        Task<string> NewAsync(string gallery, string item, FileInfoPart file, Data data);

        [Multipart]
        [Post("/gallery/{gallery}/items/{item}")]
        Task<string> NewAsync(string gallery, string item, FileInfo file, string Format, int Size, DataTypes Type);

        [Get("/gallery/{gallery}/items/{item}")]
        Task<Item> ViewAsync(string gallery, string item);

        [Delete("/gallery/{gallery}/items/{item}")]
        Task RemoveAsync(string gallery, string item);

        //[Put("/gallery/{gallery}/items/{id}")]
        //TODO Task UpdateAsync(string id, string gallery, [Body] Item item);

    }
}