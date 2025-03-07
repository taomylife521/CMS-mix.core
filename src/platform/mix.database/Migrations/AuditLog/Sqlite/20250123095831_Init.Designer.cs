﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mix.Database.Entities.AuditLog;

#nullable disable

namespace Mix.Database.Migrations.AuditLog.Sqlite
{
    [DbContext(typeof(SqliteAuditLogDbContext))]
    [Migration("20250123095831_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Mix.Database.Entities.AuditLog.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id")
                        .HasDefaultValueSql("newid()");

                    b.Property<string>("Body")
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("varchar(250)")
                        .HasColumnName("created_by");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime")
                        .HasColumnName("created_date_time");

                    b.Property<string>("Endpoint")
                        .HasColumnType("varchar(4000)")
                        .HasColumnName("endpoint");

                    b.Property<string>("Exception")
                        .HasColumnType("text")
                        .HasColumnName("exception");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime")
                        .HasColumnName("last_modified");

                    b.Property<string>("Method")
                        .HasColumnType("varchar(50)")
                        .HasColumnName("method");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("varchar(250)")
                        .HasColumnName("modified_by");

                    b.Property<int>("Priority")
                        .HasColumnType("integer")
                        .HasColumnName("priority");

                    b.Property<string>("QueryString")
                        .HasColumnType("varchar(4000)")
                        .HasColumnName("query_string");

                    b.Property<string>("RequestIp")
                        .HasColumnType("varchar(50)")
                        .HasColumnName("request_ip");

                    b.Property<string>("Response")
                        .HasColumnType("text")
                        .HasColumnName("response");

                    b.Property<int>("ResponseTime")
                        .HasColumnType("integer")
                        .HasColumnName("response_time");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("status")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<int>("StatusCode")
                        .HasColumnType("integer")
                        .HasColumnName("status_code");

                    b.Property<bool>("Success")
                        .HasColumnType("INTEGER")
                        .HasColumnName("success");

                    b.HasKey("Id")
                        .HasName("pk_audit_log");

                    b.ToTable("audit_log", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
