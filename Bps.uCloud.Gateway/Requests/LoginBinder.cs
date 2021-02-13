namespace Bps.uCloud.Gateway.Requests
{
    using Nancy;
    using Nancy.ModelBinding;
    using System;

    public class LoginBinder : IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var request = (instance as Login) ?? new Login();
            var form = context.Request.Form;
            request.UserName = form["username"];
            request.Password = form["password"];
            request.RememberMe = form["rememberme"];
            return request;
        }

        public bool CanBind(Type modelType)
        {
            return (modelType == typeof(LoginBinder));
        }
    }
}
