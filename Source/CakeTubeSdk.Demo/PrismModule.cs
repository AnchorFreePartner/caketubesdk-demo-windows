// <copyright file="PrismModule.cs" company="AnchorFree Inc.">
// Copyright (c) AnchorFree Inc. All rights reserved.
// </copyright>
// <summary>Describes a Prism Module.</summary>

namespace CakeTubeSdk.Demo
{
    using CakeTubeSdk.Demo.Control;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;
    using Prism.Regions;

    /// <summary>
    /// Prism module for the sample application.
    /// </summary>
    public class PrismModule : IModule
    {
        /// <summary>
        /// PRISM region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Unity DI container.
        /// </summary>
        private readonly IUnityContainer unityContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrismModule"/> class.
        /// </summary>
        /// <param name="regionManager">PRISM region manager.</param>
        /// <param name="unityContainer">Unity DI container.</param>
        public PrismModule(
            IRegionManager regionManager,
            IUnityContainer unityContainer)
        {
            this.regionManager = regionManager;
            this.unityContainer = unityContainer;
        }

        /// <summary>
        /// Contains module initialization logic - types and view registration.
        /// </summary>
        public void Initialize()
        {
            this.unityContainer.RegisterType<object, MainScreen>(typeof(MainScreen).FullName);

            this.regionManager.RegisterViewWithRegion("MainRegion", () => this.unityContainer.Resolve<MainScreen>());
        }
    }
}