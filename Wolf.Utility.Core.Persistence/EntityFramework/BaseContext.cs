using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Persistence.EntityFramework
{
    // https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs
    public abstract class BaseContext : DbContext
    {
        protected readonly DbContextOptions options;

        protected BaseContext()
        {
        }

        protected BaseContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
            this.options = options;
        }

        public override int SaveChanges()
        {
            UpdateDatetimes();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateDatetimes();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateDatetimes();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            UpdateDatetimes();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateDatetimes()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IEntity && e.State is EntityState.Added or EntityState.Modified);

            foreach (EntityEntry entityEntry in entries)
            {
                ((IEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added) ((IEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
            }
        }
    }
}