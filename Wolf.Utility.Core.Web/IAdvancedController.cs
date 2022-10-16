using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Web
{
    public interface IAdvancedController<TEntity, TSearchable, TDto> where TEntity : class, IEntity
        where TSearchable : class, ISearchableEntity
        where TDto : class, IDto
    {
        Task<ActionResult<IEnumerable<TEntity>>> Get(TSearchable entity);
        Task<ActionResult<TEntity>> GetDetails(int id);
        Task<ActionResult<TEntity>> Update(TEntity entity);
        Task<ActionResult<TEntity>> Add(TDto entity);
        Task<IActionResult> Delete(int id);
    }
}