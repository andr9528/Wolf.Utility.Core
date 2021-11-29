namespace Wolf.Utility.Xamarin.Services
{
    public interface IServiceCollection
    {
        IKeyboardService KeyboardService { get; }
        void SetKeyboardService(IKeyboardService service);
    }
}
