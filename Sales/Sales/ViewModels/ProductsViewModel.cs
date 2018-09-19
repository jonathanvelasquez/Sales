namespace Sales.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Plugin.Media.Abstractions;
    using Sales.Common.Models;
    using Sales.Herlpers;
    using Sales.Services;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModel
    {
        #region Attributes
        private ApiService apiService;

        private bool isRefreshing;
       
        #endregion

        #region Properties
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetValue(ref isRefreshing, value); }
        }

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { SetValue(ref this.products, value); }
        }
        #endregion

        #region Constructor
        public ProductsViewModel()
        {
            instance = this;
            apiService = new ApiService();
            LoadProducts();
        }
        #endregion

        #region Sigleton
        private static ProductsViewModel instance;

        public static ProductsViewModel GetInstce()
        {
            if (instance == null)
            {
                return new ProductsViewModel(); 
            }

            return instance;
        }


        #endregion

        #region Methods
        private async void LoadProducts()
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await apiService.GetList<Product>(
                url,
                prefix,
                controller);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            var list = (List<Product>)response.Result;
            Products = new ObservableCollection<Product>(list);
            IsRefreshing = false;
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }
        }
        #endregion

        
    }
}
