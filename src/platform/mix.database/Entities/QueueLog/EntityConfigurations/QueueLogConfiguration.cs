﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mix.Database.Entities.QueueLog;
using Mix.Database.EntityConfigurations.Base;
using Mix.Database.Services.MixGlobalSettings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mix.Database.Entities.QueueLog.EntityConfigurations
{
    internal class QueueLogConfiguration : EntityBaseConfiguration<QueueLog, Guid>
    {
        public QueueLogConfiguration(DatabaseService databaseService): base(databaseService)
        {
        }

        public override void Configure(EntityTypeBuilder<QueueLog> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.TenantId)
               .HasColumnName("tenant_id");

            builder.Property(e => e.QueueMessageId)
               .HasColumnName("queue_message_id")
               .HasColumnType(Config.Guid);

            builder.Property(e => e.DataTypeFullName)
               .HasColumnName("data_type_full_name")
               .HasColumnType($"{Config.String}{Config.MediumLength}");

            builder.Property(e => e.TopicId)
                .HasColumnName("topic_id")
                .HasColumnType($"{Config.String}{Config.MediumLength}");

            builder.Property(e => e.SubscriptionId)
                .HasColumnName("subscription_id")
                .HasColumnType($"{Config.String}{Config.MediumLength}");

            builder.Property(e => e.DataTypeFullName)
                .HasColumnName("data_type_full_name")
                .HasColumnType($"{Config.String}{Config.MediumLength}");

            builder.Property(e => e.Action)
                .HasColumnName("action")
                .HasColumnType($"{Config.String}{Config.MediumLength}");

            builder.Property(e => e.Note)
                .HasColumnName("note")
                .HasColumnType($"{Config.String}{Config.MediumLength}");

            builder.Property(e => e.State)
               .IsRequired()
               .HasColumnName("state")
               .HasConversion(new EnumToStringConverter<MixQueueMessageLogState>())
               .HasColumnType($"{Config.String}{Config.SmallLength}")
               .HasCharSet(Config.CharSet);

            builder.Property(e => e.StringData)
                .HasColumnName("string_data")
                .HasColumnType(Config.Text);

            builder.Property(e => e.ObjectData)
                .HasColumnName("object_data")
             .HasConversion(
                 v => v.ToString(Newtonsoft.Json.Formatting.None),
                 v => !string.IsNullOrEmpty(v) ? JObject.Parse(v) : new())
             .IsRequired(false)
             .HasColumnType(Config.Text);

            builder.Property(e => e.Exception)
                .HasColumnName("exception")
            .HasConversion(
                 v => v.ToString(Newtonsoft.Json.Formatting.None),
                 v => !string.IsNullOrEmpty(v) ? JObject.Parse(v) : new())
             .IsRequired(false)
             .HasColumnType(Config.Text);

            builder.Property(e => e.Subscriptions)
                .HasColumnName("subscriptions")
             .HasConversion(
                 v => v.ToString(Newtonsoft.Json.Formatting.None),
                 v => !string.IsNullOrEmpty(v) ? JArray.Parse(v) : new())
             .IsRequired(false)
             .HasColumnType(Config.Text);
        }
    }
}
