namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Views;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class MainViewModel
    {
        public ProductsViewModel Products { get; set; }

        public AddProductViewModel AddProduct { get; set; }

        public MainViewModel()
        {
            Products = new ProductsViewModel();
        }

        public ICommand AddProdCommand
        {
            get
            {
                return new RelayCommand(GoToAddProd);
            }
        }

        private async void GoToAddProd()
        {
            AddProduct = new AddProductViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new AddProductPage());
        }
    }
}
