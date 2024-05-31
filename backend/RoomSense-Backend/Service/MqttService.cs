// Ignore Spelling: Mqtt

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using RoomSense_Backend.Message;

namespace RoomSense_Backend.Service
{

    public class MqttService : IHostedService
    {
        private readonly IMqttService _mqttService;

        public MqttService(ILogger<IMqttService> logger, MqttConnectionOptions options, IMessageProcessor messageProcessor)
        {
            _mqttService = new MqttConnectionService(logger, options, messageProcessor);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _mqttService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _mqttService.StopAsync(cancellationToken);
        }
    }
}
