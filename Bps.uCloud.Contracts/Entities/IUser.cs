namespace Bps.uCloud.Contracts.Entities
{
    using System;

    public interface IUser
    {
        string Id { get; }

        string Password { get; }

        string Tag { get; }

        DateTime Timestamp { get; }

        DateTime LastLogin { get; }
    }
}