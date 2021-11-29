namespace Wolf.Utility.Xamarin.Services
{
    public interface IKeyboardService
    {
        bool IsKeyboardShown { get; }
        void HideKeyboard();
        void ReInitializeInputMethod();
    }
}
