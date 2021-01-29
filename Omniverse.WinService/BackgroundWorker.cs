using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omniverse.WinService
{
    public class BackgroundWorker
    {
        private readonly ILogger _logger;
        private readonly BackendServiceClient _backendServiceClient;

        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public BackgroundWorker(ILogger logger, BackendServiceClient backendServiceClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _backendServiceClient = backendServiceClient ?? throw new ArgumentNullException(nameof(backendServiceClient));

            _cancellationTokenSource = new CancellationTokenSource();
           _task = new Task(async (_) => await Run(), null, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        }


        private async Task Run()
        {
            var runsCount = 0;

            do
            {
                _logger.Info($"Background worker run #{++runsCount}");

                try
                {
                    var todo = await _backendServiceClient.GetTodoAsync(_cancellationTokenSource.Token);

                    _logger.Info(todo);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(60));

            } while (!_cancellationTokenSource.IsCancellationRequested);

        }

        public void Start()
        {
            _logger.Info("Background worker started!");
            _task.Start();
        }

        public void Stop()
        {
            _logger.Info("Background worker stopped!");
            _cancellationTokenSource.Cancel();
        }
    }
}
