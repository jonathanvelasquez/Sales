namespace Sales.Infrastructure
{
    using Sales.ViewModels;

    public class InstaceLocator
    {
        public MainViewModel Main { get; set; }

        public InstaceLocator()
        {
            Main = new MainViewModel();
        }
    }
}
