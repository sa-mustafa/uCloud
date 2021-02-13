namespace Bps.uCloud.Contracts.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class Image
    {
        [DataMember(Name="id")]
        [IgnoreDataMember] 
        public string Id { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Name length can't be more than 128.")]
        [DataMember(Name="name")]
        public string Name { get; set; }

        [Required]
        [StringLength(1048576, ErrorMessage = "Source length can't be more than 1MB.")]
        [DataMember(Name="source")]
        public string Source { get; set; }

        [DataMember(Name="tag")]
        [StringLength(512, ErrorMessage = "Tag length can't be more than 512.")]
        public string Tag { get; set; }

        [DataMember(Name="type")]
        public ImageTypes Type { get; set; }

        public Image() { }

        public Image(Image image)
        {
            Id = image.Id;
            Name = image.Name;
            Source = image.Source;
            Tag = image.Tag;
            Type = image.Type;
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Image {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Source: ").Append(Source).Append("\n");
            sb.Append("  Tag: ").Append(Tag).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}