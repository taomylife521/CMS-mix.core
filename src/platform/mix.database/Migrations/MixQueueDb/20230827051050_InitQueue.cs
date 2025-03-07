﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mix.Database.Migrations.MixQueueDb
{
    /// <inheritdoc />
    public partial class InitQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MixQueueMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QueueMessageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TopicId = table.Column<string>(type: "varchar(250)", nullable: true),
                    SubscriptionId = table.Column<string>(type: "varchar(250)", nullable: true),
                    Action = table.Column<string>(type: "varchar(250)", nullable: true),
                    StringData = table.Column<string>(type: "ntext", nullable: true),
                    ObjectData = table.Column<string>(type: "ntext", nullable: true),
                    Exception = table.Column<string>(type: "ntext", nullable: true),
                    Subscriptions = table.Column<string>(type: "ntext", nullable: true),
                    DataTypeFullName = table.Column<string>(type: "varchar(250)", nullable: true),
                    Note = table.Column<string>(type: "varchar(250)", nullable: true),
                    State = table.Column<string>(type: "varchar(50)", nullable: false),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(250)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(250)", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixQueueMessage", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MixQueueMessage");
        }
    }
}
