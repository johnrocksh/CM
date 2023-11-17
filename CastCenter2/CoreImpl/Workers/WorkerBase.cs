namespace CastManager.Worker
{
    using System;
    using System.Threading.Tasks;

    using CastManager.Errors;
    using CastManager.Logger;

    public abstract class WorkerBase : IWorker
    {
        public Action<ErrorData> ErrorHandle { get; set; }

        public Task[] AllTasks => new Task[] { workerTask };

        protected CancellationTokenSource _cts;

        protected Task workerTask;

        public Action OnStart { get; set; }

        public Action OnStop { get; set; }

        public bool Start()
        {
            if (_cts != null)
            {
                ErrorHandle?.Invoke(
                    new ErrorData()
                    {
                        error = ErrorNum.Error_WorkerAlreadyRunning
                    });
                return false;
            }
            _cts = new CancellationTokenSource();

            workerTask = Task.Run(async () =>
            {
                try
                {
                    while (_cts.IsCancellationRequested == false)
                    {
                        await DoWorkAsync().ConfigureAwait(false);
                    }
                }
                catch(OperationCanceledException )
                {

                }
                catch(Exception ex)
                {
                    Logger.WriteException(ex.Message);
                }

                OnStop?.Invoke();
            });

            OnStart?.Invoke();
            return true;
        }

        public bool IsRunning()
        {
            return _cts != null && _cts.IsCancellationRequested == false;
        }

        public bool Stop()
        {
            var ret = IsRunning();
            if (ret)
            {
                _cts?.Cancel();
            }
            return ret;
        }

        protected abstract Task DoWorkAsync();
    }
}
