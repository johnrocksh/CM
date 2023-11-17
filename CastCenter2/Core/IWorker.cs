namespace CastManager.Worker
{
    interface IWorker
    {
        bool IsRunning();

        bool Start();

        bool Stop();

        Action OnStart { get; set; }

        Action OnStop { get; set; }

        Task[] AllTasks { get; }
    }
}
