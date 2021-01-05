using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Web
{
    public interface IAdvancedController<TEntity> where TEntity : IEntity
    {
        Task<ActionResult<IEnumerable<TEntity>>> Get(TEntity entity);
        Task<IActionResult> Put(TEntity data);
        Task<IActionResult> Post(TEntity data);
        Task<IActionResult> Delete(TEntity data);
    }
}
