namespace Sales.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
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

        private ObservableCollection<productItemViewModel> products;

        #endregion

        #region Properties
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetValue(ref isRefreshing, value); }
        }       

        public ObservableCollection<productItemViewModel> Products
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

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }
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
            var MyList = list.Select(p => new productItemViewModel
            {
                Description = p.Description,
                ImageArray = p.ImageArray,
                ImagePath = p.ImagePath,
                IsAvailable = p.IsAvailable,
                Price = p.Price,
                ProductId = p.ProductId,
                PublishOn = p.PublishOn,
                Remarks = p.Remarks,
            });
            Products = new ObservableCollection<productItemViewModel>(MyList);
            IsRefreshing = false;
        }
        #endregion


    }
}
