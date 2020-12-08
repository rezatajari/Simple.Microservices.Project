using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EventBusRabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            if (!IsConnected)
                TryConnect();
        }

        /// <summary>
        /// بررسی شرط برقراری ارتباط
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        /// <summary>
        /// اگه ارتباط برقرار نباشد در این متد برقراری ارتباط انجام می گیرد
        /// </summary>
        /// <returns></returns>
        public bool TryConnect()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                Thread.Sleep(millisecondsTimeout: 2000);
                _connection = _connectionFactory.CreateConnection();
            }

            if (IsConnected)
                return true;
            else
                return false;
        }

        /// <summary>
        /// مدل ارتباطی
        /// </summary>
        /// <returns></returns>
        public IModel CreateModel()
        {
            if (!IsConnected)
                throw new InvalidOperationException(message:"No rabbit connection");

            return _connection.CreateModel();
        }

        /// <summary>
        /// پاکسازی منابع غیر قابل استفاده در حافظه
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                _connection.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
