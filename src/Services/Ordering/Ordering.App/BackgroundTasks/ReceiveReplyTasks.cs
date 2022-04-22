namespace ECom.Services.Ordering.App.BackgroundTasks
{
    public class ReceiveReplyTasks : BackgroundService
    {
        private readonly int _socketPort;
        private readonly string _localhostIP;
        private readonly InMemoryRequestManagement _requestManagement;

        public ReceiveReplyTasks(InMemoryRequestManagement requestManagement, IConfiguration configuration)
        {
            _socketPort        = int.Parse(configuration["ExternalAddress"].Split(':')[1]);
            _localhostIP       = configuration["LocalhostIP"];
            _requestManagement = requestManagement;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Factory.StartNew(() =>
            {
                using (var socket = new PullSocket())
                {
                    string socketAddress = $"tcp://{_localhostIP}:{_socketPort}";
                    socket.Bind(socketAddress);
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        string replyStr = socket.ReceiveFrameString();
                        if (!string.IsNullOrEmpty(replyStr))
                        {
                            var replyData = ResponseData.FromString(replyStr);
                            _requestManagement.SetResponse(replyData.RequestId, replyData);
                        }
                    }
                }
            }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }
    }
}
