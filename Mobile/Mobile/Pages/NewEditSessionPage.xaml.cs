using Mobile.Resources;
using Mobile.WebApi;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewEditSessionPage : ContentPage
    {
        private readonly SessionSettings _settings;
        private readonly NewEditSessionPageContext _context;

        private readonly int _editingSessionId = -1;

        public NewEditSessionPage(SessionSettings settings)
        {
            InitializeComponent();

            _settings = settings;

            BindingContext = new NewEditSessionPageContext { Jwt = _settings.Jwt, FriendsList = new List<string>() };
            _context = BindingContext as NewEditSessionPageContext; 
        }

        public NewEditSessionPage(SessionSettings settings, MessageSession session)
        {
            InitializeComponent();
            _settings = settings;

            BindingContext = new NewEditSessionPageContext
            {
                Jwt = _settings.Jwt,
                Title = session.Title,
                Description = session.Description,
                FriendsList = new List<string>(session.Emails.Split(';'))
            };
            _context = BindingContext as NewEditSessionPageContext;

            _editingSessionId = session.Id;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(_editingSessionId != -1)
            {
                var builder = new StringBuilder(); bool first = true;
                foreach (var email in _context.FriendsList)
                {
                    if(!first) builder.Append(", ");
                    builder.Append((await AccountsApi.GetUserByEmail(_settings.Jwt, email)).Content.FirstName);
                    first = false;
                } 

                _context.FriendsLabel = builder.ToString();
            }
        }

        async void OnChooseFriendsPressed(object sender, EventArgs args)
        {
            var friendshipPage = new UserListPage(
                _settings,
                true,
                _context.FriendsList
            );

            friendshipPage.Disappearing += (__, _) =>
            {
                if(!friendshipPage.Canceled)
                {
                    _context.FriendsList = new List<string>(friendshipPage.SelectedFriends.Select(f => f.Email));
                }
            };

            await Navigation.PushAsync(friendshipPage);
        }

        async void OnDeleteSessionPressed(object sender, EventArgs args)
        {
            var response = await DisplayActionSheet(
                "Are you sure you want to delete this session? This cannot be undone.",
                "Cancel", null, "Yes"
            );

            if(response == "Yes")
            {
                var deleteResult = await MessagesApi.DeleteSession(_settings.Jwt, _editingSessionId);
                if(deleteResult.IsSuccessful)
                {
                    await Navigation.PopToRootAsync();
                }
            }
        }

        async void OnCancelPressed(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        async void OnSubmitPressed(object sender, EventArgs args)
        {
            var sessionResult = 
                await MessagesApi.CreateMessageSession(_settings.Jwt, _context.Title, _context.Description, _context.FriendsList);
            if(sessionResult.IsSuccessful)
            {
                await Navigation.PushAsync(new MessageSessionPage(_settings, sessionResult.Content));
            }
        }
    }

    public class NewEditSessionPageContext : INotifyPropertyChanged
    { 
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        /* ----------------------- */

        /* Static Properties */
        public string Jwt { get; set; }

        /* Dynamic Properties */
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private List<string> _friendsList;
        public List<string> FriendsList
        {
            get => _friendsList;
            set
            {
                _friendsList = value;
                OnPropertyChanged();
            }
        }

        private string _friendsLabel;
        public string FriendsLabel 
        {
            get => _friendsLabel;
            set 
            {
                _friendsLabel = value;
                OnPropertyChanged();
            }
        }
    }
}