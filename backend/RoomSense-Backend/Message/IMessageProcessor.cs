using MQTTnet.Client;

namespace RoomSense_Backend.Message
{
    public interface IMessageProcessor
    {
        Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs);
    }
}
