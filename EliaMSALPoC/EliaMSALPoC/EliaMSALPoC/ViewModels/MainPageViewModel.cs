using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using EliaMSALPoC.Services;
using Xamarin.Forms;

namespace EliaMSALPoC.ViewModels
{
	public class MainPageViewModel : INotifyPropertyChanged
	{
        private readonly AuthService _authService;
        private readonly SimpleGraphService _simpleGraphService;

        bool _isSignedIn;
        public bool IsSignedIn { get => _isSignedIn; set { _isSignedIn = value; RaisePropertyChanges(); } }
        bool _isSigningIn;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSigningIn { get => _isSigningIn; set { _isSigningIn = value; RaisePropertyChanges(); } }
        public string Name { get; set; }

        public ICommand SignInCommand { get; set; }
        public ICommand SignOutCommand { get; set; }

        public MainPageViewModel()
        {
            _authService = new AuthService();
             _simpleGraphService = new SimpleGraphService();

            SignInCommand = new Command(async () => await SignInAsync());
            SignOutCommand = new Command(async () => await SignOutAsync());

            SignInAsync();
        }

        async Task SignInAsync()
        {
            IsSigningIn = true;

            if (await _authService.SignInAsync())
            {
                Name = await _simpleGraphService.GetNameAsync();
                IsSignedIn = true;
            }

            IsSigningIn = false;
        }

        async Task SignOutAsync()
        {
            if (await _authService.SignOutAsync())
            {
                IsSignedIn = false;
            }
        }

        void RaisePropertyChanges([CallerMemberName] string propertyname = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
    }
}

