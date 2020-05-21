// <copyright file="MainScreenViewModel.cs" company="AnchorFree Inc.">
// Copyright (c) AnchorFree Inc. All rights reserved.
// </copyright>
// <summary>Describes a MainScreenViewModel.</summary>

namespace CakeTubeSdk.Demo.ViewModel.Control
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using CakeTubeSdk.Demo.Countries;
    using CakeTubeSdk.Demo.Helper;
    using CakeTubeSdk.Demo.Logger;
    using CakeTubeSdk.Demo.Model;
    using CakeTubeSdk.Demo.Properties;
    using CakeTubeSdk.Windows;
    using CakeTubeSdk.Windows.Infrastructure;
    using CakeTubeSdk.Windows.Vpn;
    using PartnerApi;
    using PartnerApi.Misc;
    using PartnerApi.Model.Nodes;
    using PartnerApi.Parameters;
    using Prism.Commands;
    using Prism.Mvvm;

    /// <summary>
    /// Main screen view model.
    /// </summary>
    public class MainScreenViewModel : BindableBase
    {
        private static readonly string MachineId = RegistryHelper.GetMachineGuid();
        private ICommand connectCommand;
        private ICommand disconnectCommand;
        private ICommand clearLogCommand;
        private ICommand loginCommand;
        private ICommand logoutCommand;
        private IBackendService vpnServerService;
        private IReadOnlyCollection<VpnNodeModel> nodes;
        private VpnNodeModel selectedNodeModel;
        private VpnConnectionService vpnConnectionService;
        private VpnWindowsServiceHandler vpnWindowsServiceHandler;
        private DispatcherTimer dispatcherTimer;
        private string deviceId;
        private string carrierId;
        private string backendAddress;
        private string errorText;
        private string accessToken;
        private string password;
        private string vpnIpServerServer;
        private string vpnIp;
        private string bytesReceived;
        private string bytesSent;
        private string status;
        private string remainingTrafficResponse;
        private string gitHubLogin;
        private string gitHubPassword;
        private string serviceName = "CakeTube Demo Vpn Service";
        private string logContents;
        private bool isErrorVisible;
        private bool isConnectButtonVisible;
        private bool isDisconnectButtonVisible;
        private bool useGithubAuthorization;
        private bool isLoginButtonVisible;
        private bool isLogoutButtonVisible;
        private bool isLoggedIn;
        private bool useService = true;
        private bool reconnectOnWakeUp = true;
        private bool isCountryDropdownAvailable;

        /// <summary>
        /// Initializes static members of the <see cref="MainScreenViewModel"/> class.
        /// </summary>
        static MainScreenViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainScreenViewModel"/> class.
        /// </summary>
        public MainScreenViewModel()
        {
            // Init view model
            var dateTime = DateTime.Now;
            this.DeviceId = $"{MachineId}-{dateTime:dd-MM-yy}";
            this.CarrierId = "afdemo";
            this.BackendAddress = "https://backend.northghost.com";
            this.IsConnectButtonVisible = false;
            this.SetStatusDisconnected();
            this.SetStatusLoggedOut();

            // Init remaining traffic timer
            this.InitializeTimer();

            this.IsLoggingEnabled = CakeTubeLogger.IsEnabled;
        }

        /// <summary>
        /// Gets or sets the nodes collection.
        /// </summary>
        public IReadOnlyCollection<VpnNodeModel> Nodes
        {
            get => this.nodes;
            set => this.SetProperty(ref this.nodes, value);
        }

        /// <summary>
        /// Gets or sets the selected selectedNodeModel.
        /// </summary>
        public VpnNodeModel SelectedNodeModel
        {
            get => this.selectedNodeModel;
            set => this.SetProperty(ref this.selectedNodeModel, value);
        }

        /// <summary>
        /// Gets or sets device id for backend login method.
        /// </summary>
        public string DeviceId
        {
            get => this.deviceId;
            set => this.SetProperty(ref this.deviceId, value);
        }

        /// <summary>
        /// Gets or sets carrier id for backend service.
        /// </summary>
        public string CarrierId
        {
            get => this.carrierId;
            set => this.SetProperty(ref this.carrierId, value);
        }

        /// <summary>
        /// Gets or sets backend url address for backend service.
        /// </summary>
        public string BackendAddress
        {
            get => this.backendAddress;
            set => this.SetProperty(ref this.backendAddress, value);
        }

        /// <summary>
        /// Gets or sets message which is displayed in case of errors.
        /// </summary>
        public string ErrorText
        {
            get => this.errorText;
            set => this.SetProperty(ref this.errorText, value);
        }

        /// <summary>
        /// Gets or sets access token for backend methods.
        /// </summary>
        public string AccessToken
        {
            get => this.accessToken;
            set => this.SetProperty(ref this.accessToken, value);
        }

        /// <summary>
        /// Gets or sets user password for VPN.
        /// </summary>
        public string Password
        {
            get => this.password;
            set => this.SetProperty(ref this.password, value);
        }

        /// <summary>
        /// Gets or sets vPN service IP address.
        /// </summary>
        public string VpnIpServer
        {
            get => this.vpnIpServerServer;
            set => this.SetProperty(ref this.vpnIpServerServer, value);
        }

        /// <summary>
        /// Gets or sets vPN service IP address.
        /// </summary>
        public string VpnIp
        {
            get => this.vpnIp;
            set => this.SetProperty(ref this.vpnIp, value);
        }

        /// <summary>
        /// Gets or sets remaining traffic response.
        /// </summary>
        public string RemainingTrafficResponse
        {
            get => this.remainingTrafficResponse;
            set => this.SetProperty(ref this.remainingTrafficResponse, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether error visibility flag.
        /// </summary>
        public bool IsErrorVisible
        {
            get => this.isErrorVisible;
            set => this.SetProperty(ref this.isErrorVisible, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether connect button visibility flag.
        /// </summary>
        public bool IsConnectButtonVisible
        {
            get => this.isConnectButtonVisible;
            set => this.SetProperty(ref this.isConnectButtonVisible, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether disconnect button visibility flag.
        /// </summary>
        public bool IsDisconnectButtonVisible
        {
            get => this.isDisconnectButtonVisible;
            set => this.SetProperty(ref this.isDisconnectButtonVisible, value);
        }

        /// <summary>
        /// Gets or sets received bytes count.
        /// </summary>
        public string BytesReceived
        {
            get => this.bytesReceived;
            set => this.SetProperty(ref this.bytesReceived, value);
        }

        /// <summary>
        /// Gets or sets sent bytes count.
        /// </summary>
        public string BytesSent
        {
            get => this.bytesSent;
            set => this.SetProperty(ref this.bytesSent, value);
        }

        /// <summary>
        /// Gets or sets vPN connection status.
        /// </summary>
        public string Status
        {
            get => this.status;
            set => this.SetProperty(ref this.status, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether use service flag.
        /// </summary>
        public bool UseService
        {
            get => this.useService;
            set => this.SetProperty(ref this.useService, value);
        }

        /// <summary>
        /// Gets or sets name of windows service to use to establish VPN connection.
        /// </summary>
        public string ServiceName
        {
            get => this.serviceName;
            set => this.SetProperty(ref this.serviceName, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether logging enabled flag.
        /// </summary>
        public bool IsLoggingEnabled
        {
            get => CakeTubeLogger.IsEnabled;
            set
            {
                if (value)
                {
                    var eventLoggerListener = new EventLoggerListener();
                    eventLoggerListener.LogEntryArrived += (sender, args) => this.AddLogEntry(args.Entry);
                    CakeTubeLogger.AddHandler(eventLoggerListener);
                }
                else
                {
                    CakeTubeLogger.RemoveAllHandlers<EventLoggerListener>();
                }
            }
        }

        /// <summary>
        /// Gets or sets log contents.
        /// </summary>
        public string LogContents
        {
            get => this.logContents;
            set => this.SetProperty(ref this.logContents, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether login button visibility flag.
        /// </summary>
        public bool IsLoginButtonVisible
        {
            get => this.isLoginButtonVisible;
            set => this.SetProperty(ref this.isLoginButtonVisible, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether logout button visibility flag.
        /// </summary>
        public bool IsLogoutButtonVisible
        {
            get => this.isLogoutButtonVisible;
            set => this.SetProperty(ref this.isLogoutButtonVisible, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether use GitHyb authorization flag.
        /// </summary>
        public bool UseGithubAuthorization
        {
            get => this.useGithubAuthorization;
            set => this.SetProperty(ref this.useGithubAuthorization, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether logged in flag.
        /// </summary>
        public bool IsLoggedIn
        {
            get => this.isLoggedIn;
            set
            {
                this.SetProperty(ref this.isLoggedIn, value);
                this.RaisePropertyChanged(nameof(this.IsLoggedOut));
            }
        }

        /// <summary>
        /// Gets a value indicating whether logged out flag.
        /// </summary>
        public bool IsLoggedOut => !this.isLoggedIn;

        /// <summary>
        /// Gets or sets gitHub login.
        /// </summary>
        public string GitHubLogin
        {
            get => this.gitHubLogin;
            set => this.SetProperty(ref this.gitHubLogin, value);
        }

        /// <summary>
        /// Gets or sets gitHub password.
        /// </summary>
        public string GitHubPassword
        {
            get => this.gitHubPassword;
            set => this.SetProperty(ref this.gitHubPassword, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether reconnect on wake up event.
        /// </summary>
        public bool ReconnectOnWakeUp
        {
            get => this.reconnectOnWakeUp;
            set => this.SetProperty(ref this.reconnectOnWakeUp, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether is CountryDropdown available.
        /// </summary>
        public bool IsCountryDropdownAvailable
        {
            get => this.isCountryDropdownAvailable;
            set => this.SetProperty(ref this.isCountryDropdownAvailable, value);
        }

        /// <summary>
        /// Gets connect command.
        /// </summary>
        public ICommand ConnectCommand => this.connectCommand ?? (this.connectCommand = new DelegateCommand(this.Connect));

        /// <summary>
        /// Gets disconnect command.
        /// </summary>
        public ICommand DisconnectCommand => this.disconnectCommand ?? (this.disconnectCommand = new DelegateCommand(this.Disconnect));

        /// <summary>
        /// Gets clear log command.
        /// </summary>
        public ICommand ClearLogCommand => this.clearLogCommand ??
                                           (this.clearLogCommand = new DelegateCommand(this.ClearLog));

        /// <summary>
        /// Gets login command.
        /// </summary>
        public ICommand LoginCommand => this.loginCommand ?? (this.loginCommand = new DelegateCommand<object>(this.Login));

        /// <summary>
        /// Gets logout command.
        /// </summary>
        public ICommand LogoutCommand => this.logoutCommand ?? (this.logoutCommand = new DelegateCommand(this.Logout));

        /// <summary>
        /// Performs login to the backend server.
        /// </summary>
        private async void Login(object parameter)
        {
            try
            {
                // Work with UI
                this.IsErrorVisible = false;
                this.IsLoginButtonVisible = false;

                // Perform logout
                await LogoutHelper.Logout().ConfigureAwait(false);

                // Bootstrap VPN
                this.BootstrapVpn();

                var passwordBox = (PasswordBox)parameter;
                var passwordBoxValue = passwordBox.Password;

                var isGithub = this.UseGithubAuthorization;

                var vpnAuthenticationMethod = isGithub
                            ? AuthenticationMethod.GitHub
                            : AuthenticationMethod.Anonymous;

                var authAccessToken = isGithub ? await GitHubHelper.GetGithubOAuthToken(this.GitHubLogin, passwordBoxValue).ConfigureAwait(false) : string.Empty;

                if (isGithub && string.IsNullOrEmpty(authAccessToken))
                {
                    MessageBox.Show("Could not perform GitHub authorization!", Resources_Logs.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    this.IsLoginButtonVisible = true;
                    return;
                }

                // Perform login
                var loginParams = new LoginParams(
                    this.CarrierId,
                    this.DeviceId,
                    Environment.MachineName,
                    vpnAuthenticationMethod,
                    authAccessToken);
                var loginResponse = await this.vpnServerService.LoginAsync(loginParams).ConfigureAwait(false);

                // Check whether login was successful
                if (!loginResponse.IsSuccess)
                {
                    this.IsLoginButtonVisible = true;
                    this.ErrorText = loginResponse.Error ?? loginResponse.Result.ToString();
                    this.IsErrorVisible = true;
                    return;
                }

                // Remember access token for later usages
                LogoutHelper.AccessToken = loginResponse.AccessToken;
                LogoutHelper.BackendUrl = this.BackendAddress;
                this.AccessToken = loginResponse.AccessToken;

                this.UpdateCountries();

                // Work with UI
                this.SetStatusLoggedIn();

                // Update remaining traffic
                await this.UpdateRemainingTraffic().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Show error when exception occurred
                this.IsErrorVisible = true;
                this.ErrorText = e.Message;
                this.IsLoginButtonVisible = true;
            }
        }

        /// <summary>
        /// Update available countries list.
        /// </summary>
        private async void UpdateCountries()
        {
            try
            {
                // Get available countries
                var carrier = new Carrier(this.CarrierId, string.Empty, this.AccessToken);
                var nodesParams = new NodesParams(carrier, ProtocolType.OpenVpnUdp);
                var countriesResponse = await this.vpnServerService.NodesAsync(nodesParams).ConfigureAwait(false);

                // Check whether request was successful
                if (!countriesResponse.IsSuccess)
                {
                    this.IsLoginButtonVisible = true;
                    this.ErrorText = countriesResponse.Error ?? countriesResponse.Result.ToString();
                    this.IsErrorVisible = true;
                    return;
                }

                // Get countries from response
                var countries = countriesResponse.VpnNodes.Select(VpnCountriesParser.ToVpnNodeModel).ToList();

                // Remember countries
                this.Nodes = countries;
            }
            catch (Exception e)
            {
                // Show error when exception occurred
                this.IsLoginButtonVisible = true;
                this.IsErrorVisible = true;
                this.ErrorText = e.Message;
            }
        }

        private async void Logout()
        {
            try
            {
                // Work with UI
                this.IsErrorVisible = false;
                this.IsLogoutButtonVisible = false;
                this.IsLoggedIn = false;

                // Perform logout
                var logoutParams = new LogoutParams(this.AccessToken);
                var logoutResponse =
                    await this.vpnServerService.LogoutAsync(logoutParams).ConfigureAwait(false);

                // Check whether logout was successful
                if (!logoutResponse.IsSuccess)
                {
                    this.IsLogoutButtonVisible = true;
                    this.ErrorText = logoutResponse.Error ?? logoutResponse.Result.ToString();
                    this.IsErrorVisible = true;
                    return;
                }

                // Erase access token and other related properties
                LogoutHelper.AccessToken = string.Empty;
                this.AccessToken = string.Empty;
                this.VpnIp = string.Empty;
                this.Password = string.Empty;
                this.RemainingTrafficResponse = string.Empty;

                // Work with UI
                this.SetStatusLoggedOut();
            }
            catch (Exception e)
            {
                // Show error when exception occurred
                this.IsErrorVisible = true;
                this.ErrorText = e.Message;
                this.IsLoggedIn = true;
                this.IsCountryDropdownAvailable = true;
                this.IsLogoutButtonVisible = true;
            }
        }

        /// <summary>
        /// Clears log contents.
        /// </summary>
        private void ClearLog()
        {
            this.LogContents = string.Empty;
        }

        /// <summary>
        /// Performs remaining traffic timer initialization.
        /// </summary>
        private void InitializeTimer()
        {
            this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            this.dispatcherTimer.Tick += this.DispatcherTimerOnTick;
            this.dispatcherTimer.Start();
        }

        /// <summary>
        /// Subscribes to VPN client events.
        /// </summary>
        private void InitializeEvents()
        {
            this.vpnConnectionService.VpnStateChanged += this.VpnConnectionServiceOnVpnStateChanged;
            this.vpnConnectionService.VpnTrafficChanged += this.VpnConnectionServiceOnVpnTrafficChanged;
        }

        private void VpnConnectionServiceOnVpnTrafficChanged(VpnTraffic vpnTraffic)
        {
            this.BytesReceived = vpnTraffic.InBytes.ToString(CultureInfo.InvariantCulture);
            this.BytesSent = vpnTraffic.OutBytes.ToString(CultureInfo.InvariantCulture);
        }

        private void VpnConnectionServiceOnVpnStateChanged(VpnConnectionState vpnConnectionState)
        {
            this.Status = vpnConnectionState.ToString();
            switch (vpnConnectionState)
            {
                case VpnConnectionState.Disconnected:
                    this.SetStatusDisconnected();
                    break;
                case VpnConnectionState.Disconnecting:
                    this.SetStatusInProgress();
                    break;
                case VpnConnectionState.Connected:
                    this.VpnClientOnConnected();
                    break;
                case VpnConnectionState.Connecting:
                    this.SetStatusInProgress();
                    break;
            }
        }

        private void SetStatusInProgress()
        {
            this.IsDisconnectButtonVisible = false;
            this.IsConnectButtonVisible = false;
            this.IsLoginButtonVisible = false;
            this.IsLogoutButtonVisible = false;
        }

        /// <summary>
        /// VPN client connected event handler.
        /// </summary>
        private void VpnClientOnConnected()
        {
            this.IsConnectButtonVisible = false;
            this.IsDisconnectButtonVisible = true;
            this.IsLogoutButtonVisible = false;
        }

        /// <summary>
        /// Performs actions related to setting backend status to "Logged out".
        /// </summary>
        private void SetStatusLoggedOut()
        {
            this.IsLoginButtonVisible = true;
            this.IsLogoutButtonVisible = false;
            this.IsLoggedIn = false;
            this.IsCountryDropdownAvailable = false;
        }

        /// <summary>
        /// Performs actions related to setting backend status to "Logged in".
        /// </summary>
        private void SetStatusLoggedIn()
        {
            this.IsLoginButtonVisible = false;
            this.IsLogoutButtonVisible = true;
            this.IsLoggedIn = true;
            this.IsCountryDropdownAvailable = true;
        }

        /// <summary>
        /// Performs actions related to setting VPN status to "Disconnected".
        /// </summary>
        private void SetStatusDisconnected()
        {
            this.Status = "Disconnected";
            this.BytesReceived = "0";
            this.BytesSent = "0";
            this.IsDisconnectButtonVisible = false;
            this.IsConnectButtonVisible = true;
            this.IsLogoutButtonVisible = true;
            this.IsCountryDropdownAvailable = true;
        }

        /// <summary>
        /// Bootstraps VPN according to the selected parameters and initializes VPN events.
        /// </summary>
        private void BootstrapVpn()
        {
            CakeTube.Initialize(this.ServiceName, this.CarrierId, this.BackendAddress);

            this.vpnServerService = new BackendService(new Uri(this.BackendAddress));
            this.vpnConnectionService = CakeTube.VpnConnectionService;
            this.vpnWindowsServiceHandler = CakeTube.VpnWindowsServiceHandler;

            var isRunning = this.vpnWindowsServiceHandler.IsRunning();

            if (isRunning)
            {
                this.vpnWindowsServiceHandler.Stop();
            }

            this.InitializeEvents();
        }

        /// <summary>
        /// Performs VPN connection.
        /// </summary>
        private async void Connect()
        {
            try
            {
                this.IsConnectButtonVisible = false;
                this.IsDisconnectButtonVisible = false;
                this.IsLoginButtonVisible = false;
                this.IsCountryDropdownAvailable = false;
                var credentialsParams = new CredentialsParams(this.AccessToken, ProtocolType.OpenVpnUdp)
                {
                    WithCertificate = false,
                    Country = this.SelectedNodeModel.ServerModel.ServerRepresentation,
                };

                var connectionResult = await this.Connect(credentialsParams).ConfigureAwait(false);

                if (connectionResult)
                {
                    return;
                }

                credentialsParams = new CredentialsParams(this.AccessToken, ProtocolType.OpenVpnTcp)
                {
                    WithCertificate = false,
                    Country = this.SelectedNodeModel.ServerModel.ServerRepresentation,
                };

                await this.Connect(credentialsParams).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Show error when exception occurred
                this.IsLogoutButtonVisible = true;
                this.IsConnectButtonVisible = true;
                this.IsDisconnectButtonVisible = false;
                this.IsErrorVisible = true;
                this.ErrorText = e.Message;
                this.IsCountryDropdownAvailable = true;
            }
        }

        private async Task<bool> Connect(CredentialsParams credentialsParams)
        {
            var vpnCredentialsResponse = await this.vpnServerService.CredentialsAsync(credentialsParams).ConfigureAwait(false);

            if (!vpnCredentialsResponse.IsSuccess)
            {
                throw new InvalidOperationException(vpnCredentialsResponse.Error);
            }

            var connectResponse = await this.vpnConnectionService.ConnectAsync(vpnCredentialsResponse).ConfigureAwait(false);
            return connectResponse;
        }

        /// <summary>
        /// Disconnects from VPN server.
        /// </summary>
        private async void Disconnect()
        {
            try
            {
                // Disconnect VPN
                await this.vpnConnectionService.Disconnect().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Show error when exception occurred
                this.IsErrorVisible = true;
                this.ErrorText = e.Message;
            }

            // Update UI
            this.SetStatusDisconnected();
        }

        /// <summary>
        /// Remaining traffic timer tick event handler.
        /// </summary>
        private async void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            // Exit if AccessToken is empty
            if (string.IsNullOrEmpty(this.AccessToken))
            {
                return;
            }

            // Update remaining traffic
            await this.UpdateRemainingTraffic().ConfigureAwait(false);
        }

        /// <summary>
        /// Performs update of remaining traffic.
        /// </summary>
        private async Task UpdateRemainingTraffic()
        {
            try
            {
                // Check if access token is not empty
                if (string.IsNullOrEmpty(this.AccessToken))
                {
                    return;
                }

                // Get remaining traffic
                var remainingTrafficParams = new RemainingTrafficParams(this.AccessToken);
                var remainingTrafficResponseResult =
                    await this.vpnServerService.RemainingTrafficAsync(remainingTrafficParams).ConfigureAwait(false);

                // Check whether request was successful
                if (!remainingTrafficResponseResult.IsSuccess)
                {
                    return;
                }

                // Update UI with response data
                this.RemainingTrafficResponse
                    = remainingTrafficResponseResult.IsUnlimited
                        ? "Unlimited"
                        : $"Bytes remaining: {remainingTrafficResponseResult.TrafficRemaining}\nBytes used: {remainingTrafficResponseResult.TrafficUsed}";
            }
            catch (Exception e)
            {
                CakeTubeLogger.Trace(e.Message);
            }
        }

        /// <summary>
        /// Adds new log entry to the log contents.
        /// </summary>
        /// <param name="logEntry">Log entry to add.</param>
        private void AddLogEntry(string logEntry)
        {
            if (string.IsNullOrWhiteSpace(this.LogContents))
            {
                this.LogContents = string.Empty;
            }

            this.LogContents += logEntry + Environment.NewLine;
        }
    }
}