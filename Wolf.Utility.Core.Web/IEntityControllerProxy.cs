using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Web
{
    public interface IEntityControllerProxy<TEntity, TSearchable, TDto> where TEntity : class, IEntity
        where TSearchable : class, ISearchableEntity
        where TDto : class, IDto
    {
        Task<IEnumerable<TEntity>> GetEntities(TSearchable entity = null);
        Task<TEntity> GetEntity(int id);
        Task<TEntity> GetOrAddEntity(TDto entity);
        Task<TEntity> UpdateAndGetEntity(TEntity entity);
        Task<bool> DeleteEntity(int id);
    }
}
