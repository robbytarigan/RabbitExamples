﻿using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
		var factory = new ConnectionFactory { HostName = "172.17.0.3" };

		using (var connection = factory.CreateConnection())
		using (var channel = connection.CreateModel()) {
			channel.QueueDeclare(queue: "task_queue2", durable: true, exclusive: false, autoDelete: false, arguments: null);
			channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

			var consumer = new EventingBasicConsumer(channel);

			consumer.Received += (model, ea) => {
				var body = ea.Body;
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine(" [x] Received {0}", message);

				int dots = message.Split('.').Length - 1;
				Thread.Sleep(dots * 1000);

				Console.WriteLine(" [x] Done, sleep was {0}", dots);

				channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
			};

			channel.BasicConsume(queue: "task_queue2", autoAck: false, consumer: consumer);

			Console.WriteLine("[*] Waiting for messages. To exit press Ctrl+C");
			Console.ReadLine();
		}
        }
    }
}
