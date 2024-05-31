namespace RoomSense_Backend.Service
{
    public interface IMqttService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
