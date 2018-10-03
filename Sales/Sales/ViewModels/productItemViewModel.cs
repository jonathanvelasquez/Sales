namespace Sales.ViewModels
{
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Sales.Herlpers;
    using Sales.Services;
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class productItemViewModel : Product
    {
        #region Attributes

        private ApiService apiService;

        #endregion

        #region Constructor

        public productItemViewModel()
        {
            apiService = new ApiService();
        }

        #endregion

        #region Commands

        public ICommand DeleteProductCommand
        {
            get
            {
                return new RelayCommand(DeleteProduct);
            }
        }


        #endregion

        #region Methods

        private async void DeleteProduct()
        {
            var answer = await Application.Current.MainPage.DisplayAlert(
                Languages.Confirm, 
                Languages.DeleteConfirmation, 
                Languages.yes, 
                Languages.No);
            if (!answer)
            {
                return;
            }

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {                
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await apiService.Delete(url, prefix, controller, ProductId);

            if (!response.IsSuccess)
            {                
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            var productsViewModel = ProductsViewModel.GetInstce();
            var deletedProdcut = productsViewModel.Products.Where(p => p.ProductId == ProductId).FirstOrDefault();
            if (deletedProdcut != null)
            {
                productsViewModel.Products.Remove(deletedProdcut);
            }
        } 

        #endregion
    }
}
