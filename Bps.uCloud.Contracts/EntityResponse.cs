namespace Bps.uCloud.Contracts
{
    public class EntityResponse<Entity> where Entity : class
    {
        public Entity[] List { get; set; }
    }
}
