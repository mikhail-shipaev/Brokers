﻿using Brokers.DAL.Configurations;
using Brokers.DAL.Consumers;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Loggers;
using Brokers.DAL.Model;
using System;
using System.Configuration;
using System.Linq;

namespace Brokers.DAL.Console
{
    class Consumer
    {
        static IMessageConsumer consumer;
        static object locker = new object();

        static void Main(string[] args)
        {
            try
            {
                if (args.Contains("kafka"))
                {
                    consumer = new KafkaConsumer();
                }
                else
                {
                    RabbitMQSettings settings = (RabbitMQSettings)ConfigurationManager.GetSection("rabbitMQSettings");

                    consumer = new RabbitMQConsumer(settings, new Log4Net("loggerLog4net"));
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.Read();
                return;
            }

            //consumer.NewMessage += HandlingMessage;// (работает)
            consumer.StartConsume();

            System.Console.Read();
            consumer.Close();
        }
        private static void HandlingMessage(Message message)
        {
            var props = typeof(Message).GetProperties().Select(p => new { name = p.Name, value = p.GetValue(message) });
            lock (locker)
            {
                foreach (var item in props)
                {
                    System.Console.WriteLine(string.Format($"{item.name} = {item.value}"));
                }
                System.Console.WriteLine();
            }
        }
    }
}
