namespace Bps.uCloud.Gateway.Requests
{
    using Bps.uCloud.Contracts;
    using Nancy;
    using Nancy.ModelBinding;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class DataBinder : IModelBinder
    {
        object IBinder.Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var request = (instance as Data) ?? new Data();
            var form = context.Request.Form;
            if (form["data"].HasValue)
            {
                request = JsonConvert.DeserializeObject<Data>(form["data"].Value);
            }
            else
            {
                request.Format = form["Format"].Value;
                request.Size = int.Parse(form["Size"].Value);
                request.Type = Enum.Parse<DataTypes>(form["Type"].Value);
            }

            var file = context.Request.Files.FirstOrDefault();
            request.Size = (int)file.Value.Length;
            request.Bytes = new byte[request.Size];
            file.Value.Read(request.Bytes, 0, request.Size);
            return request;
        }

        bool IModelBinder.CanBind(Type modelType)
        {
            return modelType == typeof(Data);
        }
    }
}