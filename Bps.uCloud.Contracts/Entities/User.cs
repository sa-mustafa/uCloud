namespace Bps.uCloud.Contracts.Entities
{
    using System;

    public class User : IUser
    {
        public string Id { get; set; }

        public string Password { get; set; }

        public string Tag { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime LastLogin { get; set; }

        public User()
        {
            Timestamp = DateTime.UtcNow;
        }

        public bool IsNull => Id?.Length == 0;

        public static User Null => new User { Id = string.Empty };

        public static EntityRequest<User> Request(Tasks type)
        {
            return new EntityRequest<User>(new User(), type);
        }

        public static EntityRequest<User> Request(Tasks type, string id)
        {
            return new EntityRequest<User>(new User() { Id = id }, type);
        }

        public static EntityResponse<User> Response()
        {
            return new EntityResponse<User>();
        }
    }
}