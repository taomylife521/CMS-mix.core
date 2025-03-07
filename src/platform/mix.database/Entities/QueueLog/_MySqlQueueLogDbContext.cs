﻿using Mix.Database.Entities.QueueLog.EntityConfigurations;
using Mix.Database.Services.MixGlobalSettings;
using Mix.Heart.Services;

namespace Mix.Database.Entities.QueueLog
{
    public class MySqlQueueLogDbContext : QueueLogDbContext
    {
        public MySqlQueueLogDbContext(DatabaseService databaseService) : base(databaseService)
        {
        }
    }
}
