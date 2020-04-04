﻿using Brokers.DAL.Configurations;
using Brokers.DAL.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Producers
{
    public class RabbitMQProducer : IProducer
    {
        static IModel channel;
        static IConnection connection;
        private string queueName;

        public RabbitMQProducer(RabbitMQSettings config)
        {
            try
            {
                InitConnection(config);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable connect to RabbitMQ");
            }
        }
        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }

        public void Initialize()
        {
            var cf = new ConnectionFactory();
            connection = cf.CreateConnection();

            channel = connection.CreateModel();
            channel.QueueDeclare("messages", true, false, false, null);
        }

        public void SendRequest(string message)
        {
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("main", "", null, messageBodyBytes);
        }

        private void InitConnection(RabbitMQSettings config)
        {
            var cf = GetConnectionFactory(config);

            queueName = config.QueueName;

            connection = cf.CreateConnection();

            channel = connection.CreateModel();
            channel.ExchangeDeclare("main", "fanout");
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, "main", "", null);
        }

        private ConnectionFactory GetConnectionFactory(RabbitMQSettings config)
        {
            var cf = new ConnectionFactory();
            cf.AutomaticRecoveryEnabled = config.AutomaticRecoveryEnabled;
            cf.ContinuationTimeout = config.ContinuationTimeout;
            cf.DispatchConsumersAsync = config.DispatchConsumersAsync;
            cf.HandshakeContinuationTimeout = config.HandshakeContinuationTimeout;
            cf.HostName = config.HostName;
            cf.NetworkRecoveryInterval = config.NetworkRecoveryInterval;
            cf.Password = config.Password;
            cf.Port = config.Port;
            cf.RequestedChannelMax = config.RequestedChannelMax;
            cf.RequestedConnectionTimeout = config.RequestedConnectionTimeout;
            cf.RequestedFrameMax = config.RequestedFrameMax;
            cf.RequestedHeartbeat = config.RequestedHeartbeat;
            cf.SocketReadTimeout = config.SocketReadTimeout;
            cf.TopologyRecoveryEnabled = config.TopologyRecoveryEnabled;
            cf.UseBackgroundThreadsForIO = config.UseBackgroundThreadsForIO;
            cf.UserName = config.UserName;
            cf.VirtualHost = config.VirtualHost;

            return cf;
        }
    }
}
