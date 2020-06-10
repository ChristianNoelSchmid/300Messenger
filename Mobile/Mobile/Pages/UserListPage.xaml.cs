using Mobile.Resources;
using Mobile.WebApi;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserListPage : ContentPage
    {
        private UserListContext _context;
        private SessionSettings _settings;
        private bool _initialized = false;
        public bool Canceled { get; private set; } = false;
        public List<User> SelectedFriends = new List<User>();

        public UserListPage(SessionSettings settings, bool multiSelection = false, List<string> friendEmails = null)
        {
            InitializeComponent();

            BindingContext = new UserListContext(multiSelection);
            _context = BindingContext as UserListContext;

            if (friendEmails != null)
                _context.SetFriendEmails(friendEmails);

            _settings = settings;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (!_initialized)
            {
                _initialized = true;
                LayoutMultiselect.IsVisible = _context.MultiSelection;
                await LoadFriends();
            }
            else
            {
                if (EntrySearch.Text == null || EntrySearch.Text.Trim() == "")
                    await LoadFriends();
                else await LoadUsers();
            }
        }

        async void OnSearchCompletion(object sender, EventArgs args)
        {
            if (EntrySearch.Text.Trim() == "")
                await LoadFriends();
            else
                await LoadUsers();
        }

        /// <summary>
        /// Loads the User's friends into the list.
        /// The default functionality of the page - other Users are
        /// only displayed when the search bar is used
        /// </summary>
        private async Task LoadFriends()
        {
            LayoutLoading.IsVisible = true;
            LayoutSearch.IsEnabled = false;
            ViewUsers.IsEnabled = false;
            // Get the User currently running the app
            var appUser = (await AccountsApi.GetUserByJwt(_settings.Jwt)).Content;
            // Retrieve the current User's friendships from the Friendships microservice
            var friendshipsResult = await FriendshipsApi.GetUserFriends(_settings.Jwt);

            if (friendshipsResult.IsSuccessful)
            {
                var friends = new List<UserInfo>();
                // Cycle through each friend
                foreach (var friendship in friendshipsResult.Content)
                {
                    // Retrieve the friend's Email. It is the email associated with the friendship that isn't the User's
                    var friendEmail = friendship.ConfirmerEmail == appUser.Email ? friendship.RequesterEmail : friendship.ConfirmerEmail;

                    // Retrieve the friends User data from the Accounts microservice
                    var friendUser = (await AccountsApi.GetUserByEmail(_settings.Jwt, friendEmail)).Content;

                    var userInfo = await UserToUserInfo(friendUser);
                    if (_context.SelectedFriendEmails.Contains(userInfo.Email))
                        userInfo.Selected = true;

                    // Finally, convert the friendUser to a UserInfo, and add to the context list
                    friends.Add(userInfo);
                }
                _context.ClearAndSetUsersInfo(friends);
            }

            LayoutLoading.IsVisible = false;
            LayoutSearch.IsEnabled = true;
            ViewUsers.IsEnabled = true;
        }

        /// <summary>
        /// Loads the Users into the list,
        /// based on supplied text from the search bar
        /// </summary>
        private async Task LoadUsers()
        {
            LayoutLoading.IsVisible = true;
            LayoutSearch.IsEnabled = false;
            ViewUsers.IsEnabled = false;

            // Get the User currently running the app
            var appUser = (await AccountsApi.GetUserByJwt(_settings.Jwt)).Content;
            // Retrieve the current User's friendships from the Friendships microservice
            var usersResult = await AccountsApi.GetUsersByQuery(_settings.Jwt, EntrySearch.Text);

            if (usersResult.IsSuccessful)
            {
                var users = new List<UserInfo>(usersResult.Content.Length);
                // Cycle through each friend
                foreach (var user in usersResult.Content)
                {
                    // Finally, convert the friendUser to a UserInfo, and add to the context list
                    users.Add(await UserToUserInfo(user));
                }
                users.OrderBy(s => s.FriendStatus);

                _context.ClearAndSetUsersInfo(users);
            }

            LayoutLoading.IsVisible = false;
            LayoutSearch.IsEnabled = true;
            ViewUsers.IsEnabled = true;
        }

        async void OnUserTapped(object sender, ItemTappedEventArgs args)
        {
            var userInfo = _context.UsersInfo[args.ItemIndex];
            if (_context.MultiSelection)
            {
                if (userInfo.FriendStatus == FriendStatus.IsConfirmed)
                {
                    userInfo.Selected = !userInfo.Selected;
                    if (userInfo.Selected)
                        _context.AddSelectedFriendEmail(userInfo.Email);
                    else
                        _context.RemoveSelectedFriendEmail(userInfo.Email);
                }
            }
            else
            {
                await Navigation.PushAsync(new UserPage(_settings, userInfo.Email));
            }
        }

        async void OnCancelPressed(object sender, EventArgs args)
        {
            Canceled = true;
            await Navigation.PopAsync();
        }

        async void OnConfirmPressed(object sender, EventArgs args)
        {
            if(_context.MultiSelection)
            {
                foreach(var friendEmail in _context.SelectedFriendEmails)
                {
                    SelectedFriends.Add(
                        (await AccountsApi.GetUserByEmail(_settings.Jwt, friendEmail)).Content
                    );
                }
            }

            await Navigation.PopAsync();
        }

        private async Task<UserInfo> UserToUserInfo(User user)
        {
            var userInfo = new UserInfo
            {
                Email = user.Email,
                Name = user.FirstName + " " + user.LastName,
                Selected = false
            };

            var friendResult = await FriendshipsApi.GetFriendship(_settings.Jwt, user.Email);
            if (friendResult.IsSuccessful && friendResult.Content != null)
                userInfo.FriendStatus = friendResult.Content.IsConfirmed ? FriendStatus.IsConfirmed : FriendStatus.IsNotConfirmed;
            else
                userInfo.FriendStatus = FriendStatus.NotFriend;

            userInfo.ProfilePhoto = (await ImagesApi.GetProfileImage(userInfo.Email, true)).Content;
            userInfo.IsSelectable = _context.MultiSelection && userInfo.FriendStatus == FriendStatus.IsConfirmed;

            return userInfo;
        }
    }

    public class UserListContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        /* ----------------- */

        public UserListContext(bool multiSelection)
        {
            MultiSelection = multiSelection;
            _usersInfo = new List<UserInfo>();
            _selectedFriends = new HashSet<string>();
        }

        /* Static Properties */
        public bool MultiSelection { get; private set; }

        /* Dynamic Properties */
        private List<UserInfo> _usersInfo;
        public ReadOnlyCollection<UserInfo> UsersInfo => _usersInfo.AsReadOnly();
        public void AddUserInfo(UserInfo info)
        {
            _usersInfo.Add(info);
            if (SelectedFriendEmails.Contains(info.Email))
                info.Selected = true;
            OnPropertyChanged(nameof(UsersInfo));
        }
        public void RemoveUserInfo(UserInfo info)
        {
            if (_usersInfo.Remove(info))
                OnPropertyChanged(nameof(UsersInfo));
        }
        public void ClearAndSetUsersInfo(IEnumerable<UserInfo> usersInfo)
        {
            _usersInfo = new List<UserInfo>(usersInfo);
            OnPropertyChanged(nameof(UsersInfo));
        }

        private HashSet<string> _selectedFriends;
        public ReadOnlyCollection<string> SelectedFriendEmails => _selectedFriends.ToList().AsReadOnly();
        public void AddSelectedFriendEmail(string friend)
        {
            _selectedFriends.Add(friend);
            OnPropertyChanged(nameof(SelectedFriendEmails));
            OnPropertyChanged(nameof(SelectedFriendsNames));
        }
        public void RemoveSelectedFriendEmail(string friend)
        {
            _selectedFriends.Remove(friend);
            OnPropertyChanged(nameof(SelectedFriendEmails));
            OnPropertyChanged(nameof(SelectedFriendsNames));
        }
        public void SetFriendEmails(IEnumerable<string> friends)
        {
            _selectedFriends = new HashSet<string>(friends);
            OnPropertyChanged(nameof(SelectedFriendEmails));
            OnPropertyChanged(nameof(SelectedFriendsNames));
        }
        public string SelectedFriendsNames => string.Join("; ", _selectedFriends);
    }

    public class UserInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /* ------------ */

        /* Static Properties */
        public bool IsSelectable { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public ImageSource ProfilePhoto { get; set; }
        public FriendStatus FriendStatus { get; set; }

        /* Dynamic Properties */
        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BackgroundColor));
                OnPropertyChanged(nameof(FriendStatusLabel));
            }
        }
        public Color BackgroundColor
        {
            get
            {
                if (FriendStatus == FriendStatus.IsConfirmed)
                {
                    if (Selected) return Color.Orange;
                    return Color.FromRgb(245, 245, 255);
                }
                else if (FriendStatus == FriendStatus.IsNotConfirmed)
                    return Color.FromRgb(255, 245, 245);
                return Color.Transparent;
            }
        }

        public string FriendStatusLabel
        {
            get
            {
                switch(FriendStatus)
                {
                    case FriendStatus.IsConfirmed: return "Friend";
                    case FriendStatus.IsNotConfirmed: return "Awaiting Confirmation";
                    default: return "";
                };
            }
        }
    }
}