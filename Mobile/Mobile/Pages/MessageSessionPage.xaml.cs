using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Resources;
using Mobile.WebApi;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageSessionPage : ContentPage
    {
        private SessionSettings _settings;
        private MessageSessionPageContext _context;

        private HubConnection _hubConnection;
        
        public MessageSessionPage(SessionSettings settings, MessageSession session)
        {
            _settings = settings;
            BindingContext = new MessageSessionPageContext(session);
            _context = BindingContext as MessageSessionPageContext;

            InitializeComponent();
        }   

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            _context.UserEmail = (await AccountsApi.GetUserByJwt(_settings.Jwt)).Content.Email;
            await _context.Initialize();
            await _context.RefreshMessages(_settings.Jwt);

            _hubConnection = await MessagesApi.GetSessionConnection(_context.Session);
            _hubConnection.On("UpdateSessions", async () =>
            {
                await _context.RefreshMessages(_settings.Jwt);
            });

            if (_context.UserIsOwner)
            {
                ButtonEditSession.IsVisible = true;
            }
        }

        protected async override void OnDisappearing()
        {
            await _hubConnection.StopAsync();
        }

        async void OnEditSessionPressed(object sender, EventArgs args)
        {
            await Navigation.PushAsync(
                new NewEditSessionPage(_settings, _context.Session)
            );
        }

        async void OnNewMessageEntered(object sender, EventArgs args)
        {
            if (EditorAddNewMessage.Text.Trim() != "")
            {
                var addMessageResult =
                    await MessagesApi.AddMessageToSession(_settings.Jwt, _context.Session.Id, EditorAddNewMessage.Text);

                if (addMessageResult.IsSuccessful)
                {
                    EditorAddNewMessage.Text = "";
                    await _hubConnection.InvokeAsync("UpdateMessage", _context.Session.Id);
                }
            }
        }

        async void OnUserMessagePressed(object sender, ItemTappedEventArgs args)
        {
            var messageInfo = _context.Messages[args.ItemIndex];

            if (_context.UserEmail == messageInfo.Email)
                await Navigation.PushAsync(new UserPage(_settings));
            else
                await Navigation.PushAsync(new UserPage(_settings, messageInfo.Email));
        }
    }

    public class MessageSessionPageContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        /* ------------------ */

        public MessageSessionPageContext(MessageSession session)
        {
            Session = session;
            Title = session.Title;
            Description = session.Description;
            _userPhotos = new Dictionary<string, byte[]>();
            foreach(var email in session.Emails.Split(';'))
            {
                _userPhotos.Add(email, null);
            }
            _messages = new List<MessageContext>();
        }

        public async Task Initialize()
        {
            var keys = _userPhotos.Keys.ToList();
            foreach(var email in keys)
            {
                var imageResult = await ImagesApi.GetProfileImagePersistent(email, true);
                if(imageResult.IsSuccessful)
                {
                    _userPhotos[email] = imageResult.Content;
                }
            }
        }
        
        public async Task RefreshMessages(string jwt)
        {
            var messagesResult = await MessagesApi.GetMessages(jwt, Session.Id);
            if (messagesResult.IsSuccessful && messagesResult.Content != null)
            {
                var messages = messagesResult.Content;
                if (messages.Length > Messages.Count)
                {
                    _messages.InsertRange(0,
                        messages.Take(messages.Length - Messages.Count).Select(
                            (Message message) =>
                            {
                                var messageContext = new MessageContext
                                {
                                    TimeStamp = message.TimeStamp,
                                    Email = message.Email,
                                    Content = message.Content,
                                    IsUser = message.Email == UserEmail,
                                    ProfilePhoto = ImageSource.FromStream(() => new MemoryStream(_userPhotos[message.Email]))
                                };
                                return messageContext;
                            }
                        )
                    ); 

                    OnPropertyChanged(nameof(Messages));
                }
            }
        }

        /* Static Properties */
        public MessageSession Session { get; private set; }
            
        public string UserEmail { get; set; }
        public bool UserIsOwner => UserEmail == Session.Emails.Split(';')[0];

        public string Title { get; set; }

        public string Description { get; set; }

        private Dictionary<string, byte[]> _userPhotos;

        /* Dynamic Properties */
        private List<MessageContext> _messages;
        public ReadOnlyCollection<MessageContext> Messages => _messages.AsReadOnly();
    }

    public class MessageContext
    {  
        /* Static Properties */
        public DateTime TimeStamp { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public bool IsUser { get; set; }
        public bool IsOther => !IsUser;
        public ImageSource ProfilePhoto { get; set; } = null;
    }
}