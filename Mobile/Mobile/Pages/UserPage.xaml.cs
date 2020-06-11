using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Mobile.Resources;
using Mobile.WebApi;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Shared.Models;
using Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Mobile.Pages
{
    internal enum UserPageFriendButtonStatus { AddAsFriend, RemoveFriend, RespondToFriendshipRequest, CancelFriendshipRequest };

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        private readonly SessionSettings _settings;
        private readonly string _email;
        private readonly UserContext _userContext;

        /// <summary>
        /// Constructor used when the User being displayed is 
        /// the User running the app.
        /// </summary>
        /// <param name="settings">The session settings for the app</param>
        public UserPage(SessionSettings settings)
        {
            InitializeComponent();
            BindingContext = new UserContext();

            _userContext = BindingContext as UserContext;
            _settings = settings;
            _email = null;
        }

        /// <summary>
        /// Constructor used when the User being displayed is not the
        /// User running the app. 
        /// </summary>
        /// <param name="email">The email of the User being displayed</param>
        /// <param name="settings">The session settings, which include the JWT of the User running the app</param>
        public UserPage(SessionSettings settings, string email)
        {
            InitializeComponent();
            BindingContext = new UserContext();

            _userContext = BindingContext as UserContext;
            _settings = settings;
            _email = email;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_email == null)
                _userContext.User = (await AccountsApi.GetUserByJwt(_settings.Jwt)).Content;
            else
            {
                _userContext.User = (await AccountsApi.GetUserByEmail(_settings.Jwt, _email)).Content;
                LayoutAddAsFriend.IsVisible = true;
                await SetFriendshipStatus();
            }

            var photoResult = await ImagesApi.GetProfileImage(_userContext.User.Email, false);

            if (photoResult.IsSuccessful)
                _userContext.Photo = photoResult.Content;

            LayoutLoading.IsVisible = false;
            LayoutContent.IsVisible = true;
        }

        private async Task SetFriendshipStatus()
        {
            var friendResult = await FriendshipsApi.GetFriendship(_settings.Jwt, _email);

            if (friendResult.IsSuccessful) // Server OK
            {
                if (friendResult.Content == null) // Friendship DNE
                {
                    _userContext.FriendLabel = "";
                    _userContext.ButtonStatus = UserPageFriendButtonStatus.AddAsFriend;
                    ButtonAddAsFriend.Text = "Add as Friend";
                }
                else // Friendship Exists
                {
                    _userContext.FriendshipId = friendResult.Content.Id;
                    if (!friendResult.Content.IsConfirmed)
                    {
                        if (_email == friendResult.Content.RequesterEmail) // If User is looking at requester's page
                        {
                            _userContext.FriendLabel = $"{_userContext.User.FirstName} wants to be Friends";
                            _userContext.ButtonStatus = UserPageFriendButtonStatus.RespondToFriendshipRequest;
                            ButtonAddAsFriend.Text = "Respond";
                        }
                        else // If User is looking at page AS requester
                        {
                            _userContext.FriendLabel = "Awaiting Friend Request";
                            _userContext.ButtonStatus = UserPageFriendButtonStatus.CancelFriendshipRequest;
                            ButtonAddAsFriend.Text = "Cancel Friend Request";
                        }
                    }
                    else
                    {
                        _userContext.FriendLabel = $"{_userContext.User.FirstName} is your friend";
                        _userContext.ButtonStatus = UserPageFriendButtonStatus.RemoveFriend;
                        ButtonAddAsFriend.Text = "Remove Friend"; ButtonAddAsFriend.TextColor = Color.DarkRed;
                    }
                }
            }
            else // Server ERROR
            {
                LayoutAddAsFriend.IsVisible = false;
            }
        }

        async void OnPhotoPressed(object sender, EventArgs args)
        {
            if (_email == null)
            {
                var choice = await DisplayActionSheet("Update Photo", "Cancel", null, "Upload a Photo", "Take a Picture");
                if (choice == "Upload a Photo")
                {
                    await CrossMedia.Current.Initialize();
                    var mediaOptions = new PickMediaOptions()
                    {
                        PhotoSize = PhotoSize.Medium
                    };
                    var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

                    if (selectedImageFile != null)
                    {
                        if ((await ImagesApi.PostProfileImage(_settings.Jwt, selectedImageFile)).IsSuccessful)
                        {
                            _userContext.Photo = ImageSource.FromStream(() => selectedImageFile.GetStream());
                        }
                    }
                }
                else if (choice == "Take a Picture")
                {
                    var selectedImageFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { });

                    if (selectedImageFile != null)
                    {
                        if ((await ImagesApi.PostProfileImage(_settings.Jwt, selectedImageFile)).IsSuccessful)
                        {
                            _userContext.Photo = ImageSource.FromStream(() => { return selectedImageFile.GetStream(); });
                        }
                    }
                }
            }
        }

        async void OnAddAsFriendPressed(object sender, EventArgs args)
        {
            if(_userContext.ButtonStatus == UserPageFriendButtonStatus.AddAsFriend)
                await FriendshipsApi.CreateFriendship(_settings.Jwt, _email);
            else if(_userContext.ButtonStatus == UserPageFriendButtonStatus.CancelFriendshipRequest)
                await FriendshipsApi.RemoveFriendship(_settings.Jwt, _userContext.FriendshipId.Value);
            else if(_userContext.ButtonStatus == UserPageFriendButtonStatus.RemoveFriend)
            {
                var choice = await DisplayActionSheet("Remove Friend?", "Cancel", "Confirm");
                if (choice == "Confirm")
                    await FriendshipsApi.RemoveFriendship(_settings.Jwt, _userContext.FriendshipId.Value);
            }
            else if(_userContext.ButtonStatus == UserPageFriendButtonStatus.RespondToFriendshipRequest)
            {
                var choice = await DisplayActionSheet("Respond to Friend Request", null, null, "Confirm", "Ignore");
                if (choice == "Confirm")
                    await FriendshipsApi.ConfirmFriendship(_settings.Jwt, _userContext.FriendshipId.Value);
                else
                    await FriendshipsApi.RemoveFriendship(_settings.Jwt, _userContext.FriendshipId.Value);
            }

            Navigation.InsertPageBefore(new UserPage(_settings, _email), this);
            await Navigation.PopAsync();
        }
 
        [ContentProperty(nameof(Source))]
        public class ImageResourceExtension : IMarkupExtension
        {
            public string Source { get; set; }

            public object ProvideValue(IServiceProvider serviceProvider)
            {
                if (Source == null)
                {
                    return null;
                }

                // Do your translation lookup here, using whatever method you require
                var imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).GetTypeInfo().Assembly);

                return imageSource;
            }
        }
    }

    public class UserContext : INotifyPropertyChanged
    {
        public double ScreenWidth => Application.Current.MainPage.Width;

        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        public int? FriendshipId { get; set; } = null;

        private ImageSource _photo;
        public ImageSource Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal UserPageFriendButtonStatus ButtonStatus { get; set; }

        private string _friendLabel;
        public string FriendLabel
        {
            get => _friendLabel;
            set
            {
                _friendLabel = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}