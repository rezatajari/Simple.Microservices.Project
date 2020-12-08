using EventBusRabbitMQ.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace EventBusRabbitMQ.Producer
{
    public class EventBusRabbitMQProducer
    {
        private readonly IRabbitMQConnection _connection;

        public EventBusRabbitMQProducer(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// ایجاد کردن کانال برای صف بندی رویداد های مدل شده از سوی سبد 
        /// و فرستادن آن به رببیت ام کیو
        /// </summary>
        /// <param name="queueName">نام صف</param>
        /// <param name="publishModel">مدل درخواستی از سوی سبد</param>
        public void PublishBasketCheckout(string queueName, BasketCheckoutEvent publishModel)
        {
            IModel channel = _connection.CreateModel();
            using (channel)
            {
                // ساخت صف برای ذخیره پیام که همان مدل سبد ما می باشد
                // در هنگام ساخت صف مشخصاتی از قبیل ذخیره شدن صف در دیتابیس 
                // حذف کردن صف به صورت خودکار و یکسری مشخصات دیگه می شه اضافه کرد
                // که آن ها را مقداردهی کرده ایم
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                string message = JsonConvert.SerializeObject(publishModel);
                byte[] body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                /// در اینجا بعد از ساخت صف و درست کردن پیام ارسالی 
                /// زمان آن رسیده است که آن را انتشار بدیم، برای اینکار
                /// یک سری پارامترها رو باید ست کنیم، در اینجا نوع ایکس چنج 
                /// را حالت پیش فرض در نظر گرفته ایم
                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties, body: body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                  {
                      Console.WriteLine("Sent RabbitMQ");
                      // implement ack handle
                  };
                channel.ConfirmSelect();
            }
        }
    }
}
