﻿namespace Sales.Herlpers
{
    using Xamarin.Forms;
    using Interfaces;
    using Resources;

    public static class Languages
    {
        static Languages()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Resource.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string ImageSource
        {
            get { return Resource.ImageSource; }
        }

        public static string FromGallery
        {
            get { return Resource.FromGallery; }
        }

        public static string NewPicture
        {
            get { return Resource.NewPicture; }
        }

        public static string Cancel
        {
            get { return Resource.Cancel; }
        }  

        public static string Price_Error
        {
            get { return Resource.Price_Error; }
        }

        public static string DescriptionError
        {
            get { return Resource.DescriptionError; }
        }

        public static string ChangeImage
        {
            get { return Resource.ChangeImage; }
        }

        public static string Remarks
        {
            get { return Resource.Remarks; }
        }

        public static string Save
        {
            get { return Resource.Save; }
        }

        public static string DescriptionPlaceHolder
        {
            get { return Resource.DescriptionPlaceHolder; }
        }

        public static string PricePlaceHolder
        {
            get { return Resource.PricePlaceHolder; }
        }

        public static string Price
        {
            get { return Resource.Price; }
        }

        public static string Description
        {
            get { return Resource.Description; }
        }

        public static string AddProduct
        {
            get { return Resource.AddProduct; }
        }

        public static string Accept
        {
            get { return Resource.Accept; }
        }

        public static string Error
        {
            get { return Resource.Error; }
        }

        public static string NoInternet
        {
            get { return Resource.NoInternet; }
        }

        public static string Products
        {
            get { return Resource.Products; }
        }

        public static string TurnOnInternet
        {
            get { return Resource.TurnOnInternet; }
        }
        
    }
}