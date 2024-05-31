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

    public class MqttHostedService : IHostedService
    {
        private readonly IMqttConnectionService mqttConnectionService;

        public MqttHostedService(IMqttConnectionService mqttConnectionService)
        {
            this.mqttConnectionService = mqttConnectionService ?? throw new ArgumentNullException(nameof(mqttConnectionService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return mqttConnectionService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return mqttConnectionService.StopAsync(cancellationToken);
        }
    }
}
