using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CastManager.Utilities
{
    public class DefferedExecution
    {
        Task task;

        CancellationTokenSource _cts;

        object lockObj = new();
        void Wait()
        {
            lock (lockObj)
            {
                if (task != null && task.Status == TaskStatus.Running)
                {
                    task.Wait(100);
                }
                task = null;
                _cts = null;
            }
        }

        void Cancel()
        {
            lock (lockObj)
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                }
            }
        }

        public void Exec<T>(Action<CancellationToken, T> action, T val, TimeSpan delay)
        {
            Cancel();
            Wait();
            lock(lockObj)
            {
                _cts = new CancellationTokenSource();
            }
            task = Task.Run( () =>
            {
                try
                {
                    Task.Delay(delay).Wait(_cts.Token);
                    action?.Invoke(_cts.Token, val);
                }
                catch (OperationCanceledException)
                {
                }
            }, _cts.Token);
        }
    }
}
