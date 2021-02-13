namespace Bps.uCloud.Contracts.Entities
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class Gallery : IGallery
    {
        [BsonId]
        [IgnoreDataMember]
        public string dbId { get; set; }

        [DataMember(Name="id")]
        [IgnoreDataMember]
        public string Id
        {
            get => GenerateID(UserId, Name);
        }

        [DataMember(Name="userId")]
        public string UserId { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Name length can't be more than 128.")]
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="tag")]
        [StringLength(512, ErrorMessage = "Tag length can't be more than 512.")]
        public string Tag { get; set; }

        [DataMember(Name="timestamp")]
        public DateTime Timestamp { get; set; }

        [DataMember(Name = "watchlist")]
        public bool Watchlist { get; set; } 

        public Gallery()
        {
            Timestamp = DateTime.UtcNow;
        }

        public bool IsNull => Id == ":";

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Gallery {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  UserId: ").Append(UserId).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Tag: ").Append(Tag).Append("\n");
            sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
            sb.Append("  Watchlist: ").Append(Watchlist).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        public static string GenerateID(string UserId, string Name)
        {
            return UserId + ":" + Name;
        }

        public static Gallery Null => new Gallery();

        public static EntityRequest<Gallery> Request(Tasks type, Gallery request)
        {
            return new EntityRequest<Gallery>(request, type);
        }

        public static EntityRequest<Gallery> Request(Tasks type, string userId)
        {
            return new EntityRequest<Gallery>(new Gallery() { UserId = userId }, type);
        }

        public static EntityRequest<Gallery> Request(Tasks type, string userId, string name)
        {
            return new EntityRequest<Gallery>(new Gallery() { UserId = userId, Name = name }, type);
        }

        public static EntityResponse<Gallery> Response()
        {
            return new EntityResponse<Gallery>();
        }

    }
}