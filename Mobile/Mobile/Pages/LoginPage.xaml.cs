using Microsoft.AspNetCore.Hosting.Internal;
using Mobile.Pages;
using Mobile.Resources;
using Mobile.WebApi;
using Newtonsoft.Json;
using Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mobile.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RetrieveLoginInfo();            
        }
 
        private async Task AnimateConfirmMessage()
        {
            ConfirmMessage.Layout(new Rectangle(0, -75, Application.Current.MainPage.Width, 75));
            ConfirmMessage.TextColor = Color.Black;
            ConfirmMessage.BackgroundColor = Color.LightBlue;
            await ConfirmMessage.LayoutTo(new Rectangle(0, 0, Application.Current.MainPage.Width, 75), 1000, Easing.CubicInOut);
        }

        async void OnLoginPressed(object sender, EventArgs e) => await AttemptLogin();

        async void OnRegisterPressed(object sender, EventArgs e)
        {
            var registerPage = new RegisterPage()
            {
                BindingContext = new RegisterContext() { Email = EntryEmail.Text ?? "" },
            };
            /*registerPage.Disappearing += async (_, __) =>
            {
                if ((registerPage.BindingContext as RegisterContext).Completed)
                {
                    await AnimateConfirmMessage();
                }
            };*/
            await Navigation.PushAsync(registerPage);
        }

        private async Task AttemptLogin()
        {
            LabelError.Text = "";
            ButtonLogin.IsEnabled = false;
            ButtonRegister.IsEnabled = false;
            EntryEmail.IsEnabled = false;
            EntryPassword.IsEnabled = false;

            LayoutLoading.IsVisible = true;
            ActivityIndicatorRunning.IsRunning = true;

            var loginResult = await AccountsApi.GetJwt(EntryEmail.Text, EntryPassword.Text);

            if(loginResult.IsSuccessful)
            {
                var jwt = loginResult.Content;
                await SecureStorage.SetAsync("300MEmail", EntryEmail.Text);
                await SecureStorage.SetAsync("300MPassword", EntryPassword.Text);
                await OpenDashboardPage(jwt);
            }
            else
            {
                LabelError.Text = loginResult.Content;
                EntryEmail.IsEnabled = true;
                EntryPassword.IsEnabled = true;
                ButtonLogin.IsEnabled = true;
                ButtonRegister.IsEnabled = true;

                ActivityIndicatorRunning.IsRunning = false;
                LayoutLoading.IsVisible = false;
            }
        }

        private async Task RetrieveLoginInfo()
        {
            string email = await SecureStorage.GetAsync("300MEmail");
            string password = await SecureStorage.GetAsync("300MPassword");
            if (email != null && password != null)
            {
                EntryEmail.Text = email;
                EntryPassword.Text = password;

                await AttemptLogin(); 
            }
        }

        private async Task OpenDashboardPage(string jwt)
        {
            Navigation.InsertPageBefore(
                new DashboardPage(
                    new SessionSettings(jwt)
                ),
                this) ;
            await Navigation.PopAsync(true);
        }
    }
}
