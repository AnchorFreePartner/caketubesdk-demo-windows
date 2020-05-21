// <copyright file="MainScreen.xaml.cs" company="AnchorFree Inc.">
// Copyright (c) AnchorFree Inc. All rights reserved.
// </copyright>
// <summary>Describes a  Main screen control.</summary>

namespace CakeTubeSdk.Demo.Control
{
    using System.Windows.Controls;

    using CakeTubeSdk.Demo.ViewModel.Control;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// Main screen control.
    /// </summary>
    public partial class MainScreen : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainScreen"/> class.
        /// </summary>
        public MainScreen()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets main screen view model (<see cref="MainScreenViewModel"/>, injected).
        /// </summary>
        [Dependency]
        public MainScreenViewModel MainScreenViewModel
        {
            get => this.DataContext as MainScreenViewModel;
            set => this.DataContext = value;
        }
    }
}