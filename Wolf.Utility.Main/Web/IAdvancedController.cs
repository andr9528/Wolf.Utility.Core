using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wolf.Utility.Main.Persistence.Core;

namespace Wolf.Utility.Main.Web
{
    public interface IAdvancedController<TEntity, TUser> where TEntity : IEntity where TUser : IEntity
    {
        Task<ActionResult<IEnumerable<TEntity>>> Get(TEntity entity);
        Task<IActionResult> Put((TEntity, TUser) data);
        Task<IActionResult> Post((TEntity, TUser) data);
        Task<IActionResult> Delete((TEntity, TUser) data);
    }
}
