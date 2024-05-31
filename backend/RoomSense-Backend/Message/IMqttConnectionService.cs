namespace RoomSense_Backend.Message
{
    public interface IMqttConnectionService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
