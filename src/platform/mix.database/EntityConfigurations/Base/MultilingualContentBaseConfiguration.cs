﻿namespace Mix.Database.EntityConfigurations.Base
{
    public abstract class MultilingualContentBaseConfiguration<T, TPrimaryKey, TConfig> : EntityBaseConfiguration<T, TPrimaryKey, TConfig>
        where TPrimaryKey : IComparable
        where T : MultilingualContentBase<TPrimaryKey>
         where TConfig : IDatabaseConstants
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Specificulture)
                .IsRequired()
                .HasColumnType($"{Config.NString}{Config.SmallLength}")
                .HasCharSet(Config.CharSet)
                .UseCollation(Config.DatabaseCollation);

            builder.Property(e => e.Icon)
                .IsRequired(false)
                .HasColumnType($"{Config.NString}{Config.SmallLength}")
                .HasCharSet(Config.CharSet)
                .UseCollation(Config.DatabaseCollation);
        }

    }
}
