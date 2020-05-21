// <copyright file="Shell.xaml.cs" company="AnchorFree Inc.">
// Copyright (c) AnchorFree Inc. All rights reserved.
// </copyright>
// <summary>Describes a Main Window behavior.</summary>

namespace CakeTubeSdk.Demo.View
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;

    using CakeTubeSdk.Demo.Helper;
    using CakeTubeSdk.Demo.ViewModel;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// Main window.
    /// </summary>
    public partial class Shell : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            this.InitializeComponent();
            var assembly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Title = $"CakeTube Demo {version.FileVersion}";
        }

        /// <summary>
        /// Gets or sets shell view model (injected).
        /// </summary>
        [Dependency]
        public ShellViewModel ShellViewModel
        {
            get => this.DataContext as ShellViewModel;
            set => this.DataContext = value;
        }

        /// <summary>
        /// Actions to perform on main window closing.
        /// </summary>
        protected override async void OnClosing(CancelEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            // Do not close window now
            e.Cancel = true;

            // Logout from backend
            await LogoutHelper.Logout().ConfigureAwait(false);

            // Shutdown application
            Application.Current.Shutdown();
        }
    }
}