namespace CastManager.Templates.Worker
{
    using CastManager.Core;
    using CastManager.Worker;

    public class TemplatesWorkers : IWorker
    {
        public Action OnStart { get; set; }
        public Action OnStop { get; set; }

        List<IWorker> workers;

        public Task[] AllTasks => workers.SelectMany(x => x.AllTasks).ToArray();

        public TemplatesWorkers(ITemplatesService templatesService)
        {
            this.workers = new List<IWorker>()          
            {
                new AssemblyTemplateImageWorker(templatesService),
                new TatamiImagesDownloaderWorker(templatesService)
            };
        }
        public bool IsRunning() => workers.Any(x => x.IsRunning());

        public bool Start()
        {
            return workers.Select(x => x.Start()).All(x => x);
        }

        public bool Stop()
        {
            return workers.Select(x => x.Stop()).All(x => x);
        }
    }
}
