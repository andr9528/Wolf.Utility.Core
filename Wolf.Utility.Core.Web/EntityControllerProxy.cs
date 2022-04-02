using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using RestSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Wolf.Utility.Core.Web
{
    public abstract class EntityControllerProxy<TEntity> : ControllerProxy, IAdvancedController<TEntity> where TEntity : class, IEntity  
    {
        protected EntityControllerProxy(string baseAddress, string controller, IHandler handler = null)
            : base(baseAddress, controller, handler)
        {
        }

        protected EntityControllerProxy(string baseAddress, string controller = null) : base(baseAddress, controller)
        {
        }

        protected abstract RestRequest BuildGetRequestQuery(TEntity entity);

        public async virtual Task<ActionResult<IEnumerable<TEntity>>> Get(TEntity entity) 
        {
            var req = BuildGetRequestQuery(entity);

            var res = await client.ExecuteAsync(req, Method.GET);

            if (res.IsSuccessful)
            {
                var obj = JsonConvert.DeserializeObject<IEnumerable<TEntity>>(res.Content);
                return new OkObjectResult(obj);
            }

            return new StatusCodeResult((int)res.StatusCode);
        }

        public async virtual Task<IActionResult> Delete(TEntity entity)
        {
            var req = new RestRequest();
            req.AddJsonBody(entity);

            var res = await client.ExecuteAsync(req, Method.DELETE);

            if (res.IsSuccessful) return new OkResult();
            return new StatusCodeResult((int)res.StatusCode);
        }

        public async virtual Task<ActionResult<TEntity>> Post(TEntity entity)
        {
            var req = new RestRequest();
            req.AddJsonBody(entity);

            var res = await client.ExecuteAsync(req, Method.POST);

            if (res.IsSuccessful) 
            {
                var obj = JsonConvert.DeserializeObject<TEntity>(res.Content);
                return new OkObjectResult(obj);
            }

            return new StatusCodeResult((int)res.StatusCode);
        }

        public async virtual Task<ActionResult<TEntity>> Put(TEntity entity)
        {
            var req = new RestRequest();
            req.AddJsonBody(entity);

            var res = await client.ExecuteAsync(req, Method.PUT);

            if (res.IsSuccessful)
            {
                var obj = JsonConvert.DeserializeObject<TEntity>(res.Content);
                return new OkObjectResult(obj);
            }

            return new StatusCodeResult((int)res.StatusCode);
        }
    }
}
