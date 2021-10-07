using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Persistence.EntityFramework
{
    public abstract class BaseContext : DbContext
    {
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is IEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((IEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((IEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
