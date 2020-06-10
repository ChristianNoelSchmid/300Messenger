using Mobile.Resources;
using Mobile.WebApi;
using Newtonsoft.Json;
using Shared.Models;
using Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        private DashboardContext _context;
        public SessionSettings Settings { get; private set; }

        public DashboardPage(SessionSettings settings)
        {
            InitializeComponent();
            Settings = settings;
            _context = (BindingContext = new DashboardContext()) as DashboardContext;
        }

        async void OnSessionPressed(object sender, ItemTappedEventArgs args)
        {
            var session = _context.Sessions[args.ItemIndex];
            await Navigation.PushAsync(new MessageSessionPage(Settings, session));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            ListViewSessions.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            await RetrieveMessageSessions();
            ListViewSessions.IsEnabled = true;
            LoadingIndicator.IsVisible = false;
        }

        private async Task RetrieveMessageSessions()
        {
            var messagesResult = await MessagesApi.GetMessageSessionsForUser(Settings.Jwt);

            if (messagesResult.IsSuccessful)
            {
                _context.Sessions = new List<MessageSession>(messagesResult.Content);
            }
        }
        async void OnUserAccountPressed(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new UserPage(Settings));
        }

        async void OnFriendsPressed(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new UserListPage(Settings));
        }

        async void OnNewSessionPressed(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new NewEditSessionPage(Settings));
        }

        async void OnLogoutPressed(object sender, EventArgs args)
        {
            var response = await DisplayActionSheet("Are you sure you want to logout?", "Cancel", null, "Logout");

            if (response == "Logout")
            {
                SecureStorage.Remove("300MEmail");
                SecureStorage.Remove("300MPassword");

                Navigation.InsertPageBefore(new LoginPage(), this);
                await Navigation.PopAsync();
            }
        }
    }

    /// <summary>
    /// Binding Context for DashboardPage
    /// Contains SessionSettings and the list of Message Sessions
    /// </summary>
    public class DashboardContext : INotifyPropertyChanged
    { 
        /// <summary>
        /// Notifier for binding cells in the View
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private List<MessageSession> _sessions = new List<MessageSession>();
        public List<MessageSession> Sessions
        {
            get => _sessions;
            set
            {
                _sessions = value;
                OnPropertyChanged();
            }
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}