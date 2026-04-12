using System;
using System.Threading.Tasks;

namespace FotoFlow.Core
{
    public interface IFotoFlowService
    {
        event Action<int> ProgressChanged;
        event Action<string> StatusChanged;
        event Action<string> ErrorOccurred;

        Task StartAsync(string destino, bool validateDelete);
        void Stop();
        bool IsRunning { get; }
    }
}
