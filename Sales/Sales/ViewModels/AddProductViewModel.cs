namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Herlpers;
    using Sales.Common.Models;
    using Sales.Services;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class AddProductViewModel : BaseViewModel
    {
        #region Attributes

        private bool isRunning;

        private bool isEnabled;

        private ApiService apiService;

        #endregion

        #region Properties

        public string Description { get; set; }

        public string Price { get; set; }

        public string Remarks { get; set; }

        public bool IsRunning
        {
            get { return isRunning; }
            set { SetValue(ref isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetValue(ref isEnabled, value); }
        }

        #endregion

        #region Constructor

        public AddProductViewModel()
        {
            IsEnabled = true;
            apiService = new ApiService();
        }

        #endregion

        #region Commands

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(Description))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error, 
                    Languages.DescriptionError, 
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(Price))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error, 
                    Languages.Price_Error, 
                    Languages.Accept);
                return;
            }

            var price = decimal.Parse(Price);
            if (price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.Price_Error,
                    Languages.Accept);
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error, 
                    connection.Message, 
                    Languages.Accept);
                return;
            }

            var product = new Product
            {
                Description = Description,
                Price = price,
                Remarks = Remarks
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await apiService.Post(url, prefix, controller, product);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            IsRunning = false;
            IsEnabled = true;

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion
    }
}
