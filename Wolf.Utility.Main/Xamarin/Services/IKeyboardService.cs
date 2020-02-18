namespace Wolf.Utility.Main.Xamarin.Services
{
    public interface IKeyboardService
    {
        bool IsKeyboardShown { get; }
        void HideKeyboard();
        void ReInitializeInputMethod();
    }
}
