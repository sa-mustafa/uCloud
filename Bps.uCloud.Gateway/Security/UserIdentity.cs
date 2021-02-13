namespace Bps.uCloud.Gateway.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    class UserIdentity : /*IUserIdentity,*/ IIdentity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AuthenticationType => "Basic";

        public bool IsAuthenticated => true;

        public string UserName => Name;

        public List<string> Claims { get;  set; }

        //IEnumerable<string> IUserIdentity.Claims => Claims;

        public UserIdentity(Token Token)
        {
            Id = Token.Id;
            Name = Token.Name;
        }

        public UserIdentity(Guid Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

    }
}
