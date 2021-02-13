namespace Bps.uCloud.Contracts
{
    public class EntityRequest<Entity> where Entity : class
    {
        public EntityRequest(Entity command, Tasks task)
        {
            Command = command;
            Task = task;
        }

        public Entity Command { get; set; }

        public Tasks Task { get; set; }

    }
}
