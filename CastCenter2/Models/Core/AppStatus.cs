namespace CastManager.Models.Core
{
    class AppStatus : IAppStatus
    {
        public string Error { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public bool SpinnerGo { get; set; } = false;

        public TimeSpan PendingDelay { get; set; }

        public TimeSpan ShowDelay { get; set; } = TimeSpan.FromMinutes(1);
    }
}
