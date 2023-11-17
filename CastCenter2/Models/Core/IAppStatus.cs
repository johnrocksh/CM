namespace CastManager.Models.Core
{
    public interface IAppStatus
    {
        /// <summary>
        /// The error message, be shown warning icon, override the Icon property
        /// </summary>
        string Error { get; }
        /// <summary>
        /// The message text
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Path to image resource, if not set used default 
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Show animation in progress (ui behaviour), override the Icon property
        /// </summary>
        bool SpinnerGo { get; }

        /// <summary>
        /// Pending delay before show status 
        /// </summary>
        TimeSpan PendingDelay { get; }

        /// <summary>
        /// Show status period of time and after will be hidden it(ui panel behaviour)
        /// Set to zero to hide status immediately
        /// </summary>
        TimeSpan ShowDelay { get; }
    }
}
