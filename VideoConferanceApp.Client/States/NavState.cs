namespace VideoConferanceApp.Client.States
{
    public class NavState
    {
        public bool Clicked { get; private set; } = false;
        public Action? ButtonAction;
        public void ClickButton()
        {
            Clicked = true;
            ButtonAction?.Invoke();
        }
    }
}
