// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Twitch;
using DiabloSpeech.Properties;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DiabloSpeech
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public ConnectWindow()
        {
            InitializeComponent();
            LoadSettings(Settings.Default);
        }

        void LoadSettings(Settings settings)
        {
            if (settings.RememberSettings)
            {
                rememberCheckBox.IsChecked = true;
                usernameTextBox.Text = settings.Username;
                channelTextBox.Text = settings.Channel;
                autenticationBox.Password = DecryptString(settings.AuthToken);
            }
        }

        void SaveSettings(Settings settings)
        {
            bool remember = rememberCheckBox.IsChecked ?? false;

            if (remember)
            {
                settings.Username = usernameTextBox.Text;
                settings.Channel = channelTextBox.Text;
                settings.AuthToken = EncryptString(autenticationBox.Password);
            }
            else
            {
                settings.Username = string.Empty;
                settings.Channel = string.Empty;
                settings.AuthToken = string.Empty;
            }

            settings.RememberSettings = remember;
            settings.Save();
        }

        string DecryptString(string encoded64)
        {
            if (string.IsNullOrEmpty(encoded64))
                return string.Empty;

            // Decode base64 string.
            byte[] buffer = Convert.FromBase64String(encoded64);

            // Decrypt data back to string.
            ProtectedMemory.Unprotect(buffer, MemoryProtectionScope.SameLogon);
            return Encoding.Unicode.GetString(buffer).TrimEnd();
        }

        string EncryptString(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("data");
            data = data + new string(' ', data.Length % 8);

            // Encypt bytes for current user.
            byte[] buffer = Encoding.Unicode.GetBytes(data);
            ProtectedMemory.Protect(buffer, MemoryProtectionScope.SameLogon);

            // Return safe value as base64.
            return Convert.ToBase64String(buffer);
        }

        void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        bool ValidateText(string text)
        {
            if (text == null) return false;
            string value = text.Trim();
            return !string.IsNullOrEmpty(value);
        }

        bool ValidateTextBox(TextBox textBox)
        {
            if (!ValidateText(textBox.Text))
            {
                textBox.Focus();
                return false;
            }
            else return true;
        }

        bool ValidatePasswordBox(PasswordBox passwordBox)
        {
            if (!ValidateText(passwordBox.Password))
            {
                passwordBox.Focus();
                return false;
            }
            else return true;
        }

        void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateTextBox(usernameTextBox))
                return;
            if (!ValidateTextBox(channelTextBox))
                return;
            if (!ValidatePasswordBox(autenticationBox))
                return;

            try
            {
                var connection = new TwitchChannelConnection(
                    usernameTextBox.Text,
                    channelTextBox.Text,
                    autenticationBox.Password);
                SaveSettings(Settings.Default);

                (new ChatBotWindow(connection)).Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void rememberCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Immidiately discard saved auhentication details if "remember me" was unchecked.
            var remember = rememberCheckBox.IsChecked ?? false;
            if (!remember) SaveSettings(Settings.Default);
        }
    }
}
