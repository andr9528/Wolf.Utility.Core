using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Web
{
    public interface IEntityControllerProxy<TEntity> where TEntity : class, IEntity
    {
        Task<IEnumerable<TEntity>> GetEntities(TEntity entity);
        Task<TEntity> GetOrAddEntity(TEntity entity);
        Task<TEntity> UpdateAndGetEntity(TEntity entity);
        Task<bool> DeleteEntity(TEntity entity);
    }
}
