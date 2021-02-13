namespace Bps.uCloud.Contracts.API
{
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAuthApi
    {
        [Get("/login")]
        Task LoginAsync();

        [Post("/login")]
        Task LoginAsync([Body(BodySerializationMethod.Json)] Dictionary<string, object> data);
    }
}
