// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Core;
using DiabloSpeech.Core.Twitch;
using DiabloSpeech.Extensions;
using DiabloSpeech.Properties;
using System;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace DiabloSpeech.ViewModel
{
    public class ConnectViewModel : ViewModelBase
    {
        #region Events

        public event Action CloseRequested;
        public event Action<SecureString> AuthTokenChanged;

        #endregion

        #region Properties

        bool rememberSettings;
        public bool RememberSettings
        {
            get { return rememberSettings; }
            set
            {
                if (SetField(ref rememberSettings, value) && value == false)
                {
                    // Forget settings when unchecking unchecking.
                    Save();
                }
            }
        }

        string username;
        public string Username
        {
            get { return username; }
            set { SetField(ref username, value); }
        }

        string channel;
        public string Channel
        {
            get { return channel; }
            set { SetField(ref channel, value); }
        }

        SecureString authToken;
        public SecureString AuthToken
        {
            get { return authToken; }
            set { SetField(ref authToken, value); }
        }

        #endregion

        #region Constructor

        public ConnectViewModel()
        {
            // Only be valid after all these properties have changes.
            RegisterChangedPropertyValid(nameof(Username));
            RegisterChangedPropertyValid(nameof(Channel));
            RegisterChangedPropertyValid(nameof(AuthToken));

            // Simple string validation for input fields.
            RegisterValidator(nameof(Username), StringValidation(minLength: 3));
            RegisterValidator(nameof(Channel), StringValidation(minLength: 3));
            RegisterValidator(nameof(AuthToken), SecureStringValidation(minLength: 3));

            CloseCommand = new RelayCommand(param => Close());
            LoginCommand = new RelayCommand(param => Login());
        }

        #endregion

        #region Persistance

        public void Load()
        {
            var settings = Settings.Default;
            if (settings.RememberSettings)
            {
                RememberSettings = settings.RememberSettings;
                Username = settings.Username;
                Channel = settings.Channel;
                if (!string.IsNullOrEmpty(settings.AuthToken))
                {
                    var sstring = new SecureString();
                    string data = ProtectedString.DecryptBase64(settings.AuthToken);
                    foreach (char c in data.ToCharArray())
                        sstring.AppendChar(c);
                    AuthToken = sstring;
                    AuthTokenChanged?.Invoke(sstring);
                }
            }
        }

        public void Save()
        {
            var settings = Settings.Default;

            if (RememberSettings)
            {
                settings.Username = Username;
                settings.Channel = Channel;
                settings.AuthToken = ProtectedString.EncryptBase64(AuthToken.ToUnsecureString());
            }
            else
            {
                settings.Username = string.Empty;
                settings.Channel = string.Empty;
                settings.AuthToken = string.Empty;
            }

            settings.RememberSettings = RememberSettings;
            settings.Save();
        }

        #endregion

        #region Validation

        Action<string, object> StringValidation(int minLength = 0)
        {
            return (prop, obj) => {
                string value = (string)obj;
                if (string.IsNullOrEmpty(value) || value.Length < minLength)
                    AddError(prop, $"{prop} needs to have at least {minLength} characters.");
            };
        }

        Action<string, object> SecureStringValidation(int minLength = 0)
        {
            return (prop, obj) => {
                SecureString value = (SecureString)obj;
                if (value == null || value.Length < minLength)
                    AddError(prop, $"{prop} needs to have at least {minLength} characters.");
            };
        }

        #endregion

        #region Actions

        public ICommand CloseCommand { get; }
        public ICommand LoginCommand { get; }

        public void Close()
        {
            CloseRequested?.Invoke();
        }

        public void Login()
        {
            var auth = new TwitchAuthenticationDetails()
            {
                Username = Username,
                Channel = Channel,
                Password = AuthToken.ToUnsecureString(),
            };

            try
            {
                var stream = new NetworkStreamTcpAdapter("irc.chat.twitch.tv", 6667);
                var connection = new TwitchChannelConnection(stream, auth);
                Save();

                (new ChatBotWindow(connection)).Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
