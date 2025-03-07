﻿using Mix.Database.Entities.QueueLog.EntityConfigurations;
using Mix.Database.Services.MixGlobalSettings;
using Mix.Heart.Services;

namespace Mix.Database.Entities.QueueLog
{
    public class PostgresQueueLogDbContext : QueueLogDbContext
    {
        public PostgresQueueLogDbContext(DatabaseService databaseService) : base(databaseService)
        {
        }
    }
}
