namespace Bps.uCloud.Gateway
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.WindowsServices;

    public class GatewayService : WebHostService
    {
        #region Constructor

        public GatewayService(IWebHost host) : base(host)
        {
        }

        #endregion

        #region Methods

        protected override void OnStarted()
        {
            Startup.Instance.OnStarted();
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            Startup.Instance.OnStopped();
            base.OnStopped();
        }

        #endregion
    }
}