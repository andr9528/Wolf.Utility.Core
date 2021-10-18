using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Web
{
    public interface IAdvancedController<TEntity> where TEntity : IEntity
    {
        Task<ActionResult<IEnumerable<TEntity>>> Get(TEntity entity);
        Task<ActionResult<TEntity>> Put(TEntity entity);
        Task<ActionResult<TEntity>> Post(TEntity entity);
        Task<IActionResult> Delete(TEntity entity);
    }
}
