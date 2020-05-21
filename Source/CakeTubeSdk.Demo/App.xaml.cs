// <copyright file="App.xaml.cs" company="AnchorFree Inc.">
// Copyright (c) AnchorFree Inc. All rights reserved.
// </copyright>
// <summary>Encapsulates a Windows Presentation Foundation (WPF) application.</summary>

namespace CakeTubeSdk.Demo
{
    using System;
    using System.Windows;
    using CakeTubeSdk.Demo.Properties;

    /// <inheritdoc />
    public partial class App : Application
    {
        /// <summary>
        /// Application startup logic.
        /// </summary>
        /// <param name="e">Arguments of the <see cref="Application.Startup"/> event.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is Exception ex))
            {
                return;
            }

            MessageBox.Show(ex.Message, Resources_Logs.UnhandledExceptionError, MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
    }
}