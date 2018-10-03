namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Herlpers;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Common.Models;
    using Sales.Services;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class AddProductViewModel : BaseViewModel
    {
        #region Attributes

        private MediaFile file;

        private ImageSource imageSource;

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

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set { SetValue(ref imageSource, value); }
        }

        #endregion

        #region Constructor

        public AddProductViewModel()
        {
            IsEnabled = true;
            apiService = new ApiService();
            imageSource = "notProduct";
        }

        #endregion

        #region Commands

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }

        private async void ChangeImage()
        {

            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.ImageSource,
                Languages.Cancel,
                null,
                Languages.FromGallery,
                Languages.NewPicture);

            if (source == Languages.Cancel)
            {
                file = null;
                return;
            }

            if (source == Languages.NewPicture)
            {
                file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
        }

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

            byte[] imageArray = null;
            if (file != null)
            {
                imageArray = FilesHelper.ReadFully(file.GetStream());
            }


            var product = new Product
            {
                Description = Description,
                Price = price,
                Remarks = Remarks,
                ImageArray = imageArray
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

            var newProduct = (Product)response.Result;
            var viewModel = ProductsViewModel.GetInstce();
            viewModel.Products.Add(new productItemViewModel
            {
                Description = newProduct.Description,
                ImageArray = newProduct.ImageArray,
                ImagePath = newProduct.ImagePath,
                IsAvailable = newProduct.IsAvailable,
                Price = newProduct.Price,
                ProductId = newProduct.ProductId,
                PublishOn = newProduct.PublishOn,
                Remarks = newProduct.Remarks,
            });
            IsRunning = false;
            IsEnabled = true;

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion
    }
}
