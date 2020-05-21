// <copyright file="LogoutHelper.cs" company="AnchorFree Inc.">
// Copyright (c) AnchorFree Inc. All rights reserved.
// </copyright>
// <summary>Describes a  Logout Helper.</summary>

namespace CakeTubeSdk.Demo.Helper
{
    using System;
    using System.Threading.Tasks;
    using PartnerApi;
    using PartnerApi.Parameters;

    /// <summary>
    /// Logout related properties and methods.
    /// </summary>
    internal static class LogoutHelper
    {
        /// <summary>
        /// Sets access token to perform logout with.
        /// </summary>
        public static string AccessToken { private get; set; }

        /// <summary>
        /// Sets access token to perform logout with.
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
                await partnerBackendService.LogoutAsync(logoutParams).ConfigureAwait(false);
            }
            catch
            {
                // Ignored
            }
        }
    }
}