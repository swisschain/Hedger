﻿namespace Hedger.Common.Configuration
{
    public class AppConfig
    {
        public DbConfig Db { get; set; }

        public RabbitMqConfig RabbitMq { get; set; }

        public LykkeHftClientConfig LykkeHftClient { get; set; }
    }
}
