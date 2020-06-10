using _300Messenger.Shared.ViewModels;
using Mobile.WebApi;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
            
        async void OnRegisterPressed(object sender, EventArgs e)
        {
            LayoutEntries.IsEnabled = false;
            LayoutLoading.IsVisible = true;

            var registerResult = await AccountsApi.Register(
                new RegisterViewModel
                {
                    Email = Email.Text,
                    FirstName = FirstName.Text,
                    LastName = LastName.Text,
                    Password = Password.Text,
                    ConfirmPassword = ConfirmPassword.Text
                }
            );

            if(registerResult.IsSuccessful)
            {
                ErrorCode.Text = "";
                ((RegisterContext)BindingContext).Completed = true;
                await Navigation.PopAsync();
            }
            else
            {
                ErrorCode.Text = registerResult.Content;
                LayoutEntries.IsEnabled = true;
                LayoutLoading.IsVisible = false;
            }
        }
    }

    public class RegisterContext
    {
        public string Email { get; set; }
        public bool Completed { get; set; }
    }
}