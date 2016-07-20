// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using DiabloSpeech.ViewModel;
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
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ConnectViewModel;
            viewModel.CloseRequested += () => Close();
            viewModel.AuthTokenChanged += token =>
                authenticationBox.Password = token.ToUnsecureString();
            viewModel.Load();
        }

        void authenticationBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            PasswordBinder.SetEncryptedPassword(passwordBox, passwordBox.SecurePassword);
        }
    }
}
