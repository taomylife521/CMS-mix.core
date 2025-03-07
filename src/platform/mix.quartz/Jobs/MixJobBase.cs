﻿using Mix.Mq.Lib.Models;
using Mix.Queue.Interfaces;
using System;
using System.Threading.Tasks;

namespace Mix.Quartz.Jobs
{
    public abstract class MixJobBase : IJob
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly IMemoryQueueService<MessageQueueModel> QueueService;
        protected MixJobBase(
            IServiceProvider serviceProvider,
            IMemoryQueueService<MessageQueueModel> queueService)
        {
            ServiceProvider = serviceProvider;
            JobType = GetType();
            JobName = JobType.FullName;
            QueueService = queueService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await ExecuteHandler(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string JobName { get; set; }
        public string Group { get; set; }
        public Type JobType { get; set; }
        public JobSchedule Schedule { get; set; }
        public abstract Task ExecuteHandler(IJobExecutionContext context);
    }
}
