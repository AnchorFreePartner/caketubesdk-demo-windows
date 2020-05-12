namespace CakeTubeSdk.Demo.Helper
{
    using System;
    using System.Threading.Tasks;
    using PartnerApi;
    using PartnerApi.Parameters;

    /// <summary>
    /// Logout related properties and methods.
    /// </summary>
    public static class LogoutHelper
    {
        /// <summary>
        /// Access token to perform logout with.
        /// </summary>
        public static string AccessToken { private get; set; }

        /// <summary>
        /// Access token to perform logout with.
        /// </summary>
        public static string BackendUrl { private get; set; }

        /// <summary>
        /// Performs logout from backend.
        /// </summary>
        public static async Task Logout()
        {
            try
            {
                // Resolve backend service
                var partnerBackendService = new BackendService(new Uri(BackendUrl));
                var logoutParams = new LogoutParams(AccessToken);
                // Logout from backend
                await partnerBackendService.LogoutAsync(logoutParams);
            }
            catch
            {
                // Ignored
            }
        }
    }
}