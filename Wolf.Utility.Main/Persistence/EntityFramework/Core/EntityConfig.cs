﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolf.Utility.Main.Persistence.Core;

namespace Wolf.Utility.Main.Persistence.EntityFramework.Core
{
    public abstract class EntityConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            var type = typeof(TEntity);
            var idName = $"{type.Name}Id";

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(idName);

            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}
