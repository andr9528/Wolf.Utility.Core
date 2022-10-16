using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
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
    /// <summary>
    /// TODO: Improve error messages when requests fails.
    /// </summary>
    public abstract class EntityControllerProxy<TEntity, TSearchable, TDto> : ControllerProxy,
        IEntityControllerProxy<TEntity, TSearchable, TDto> where TEntity : class, IEntity
        where TSearchable : class, ISearchableEntity
        where TDto : class, IDto
    {
        /// <summary>
        /// All all the properties to the request query, that should be searchable.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="entity"></param>
        protected abstract void AddPropertiesToQuery(RestRequest req, TSearchable entity);

        /// <inheritdoc />
        protected EntityControllerProxy(string baseAddress, params string[] controllerSegments) : base(baseAddress,
            controllerSegments)
        {
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> GetEntities(TSearchable entity = default)
        {
            var req = new RestRequest();
            if (entity != default)
                AddPropertiesToQuery(req, entity);

            IRestResponse res = await client.ExecuteAsync(req, Method.GET);

            if (!res.IsSuccessful)
                throw new Exception($"Request Failed - Status Code: {res.StatusCode}");

            var obj = JsonConvert.DeserializeObject<IEnumerable<TEntity>>(res.Content);
            return obj;
        }

        /// <inheritdoc />
        public async Task<TEntity> GetEntity(int id)
        {
            const string segmentName = "id";
            var req = new RestRequest($"{{{segmentName}}}");
            req.AddUrlSegment(segmentName, id);

            IRestResponse res = await client.ExecuteAsync(req, Method.GET);

            return DeserializeEntity(res);
        }

        /// <inheritdoc />
        public async Task<TEntity> GetOrAddEntity(TDto entity)
        {
            var req = new RestRequest();
            req.AddJsonBody(entity);

            IRestResponse res = await client.ExecuteAsync(req, Method.POST);

            return DeserializeEntity(res);
        }

        /// <inheritdoc />
        public async Task<TEntity> UpdateAndGetEntity(TEntity entity)
        {
            var req = new RestRequest();
            req.AddJsonBody(entity);

            IRestResponse res = await client.ExecuteAsync(req, Method.PUT);

            return DeserializeEntity(res);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEntity(int id)
        {
            const string segmentName = "id";
            var req = new RestRequest($"{{{segmentName}}}");
            req.AddUrlSegment(segmentName, id);

            IRestResponse res = await client.ExecuteAsync(req, Method.DELETE);

            if (!res.IsSuccessful)
                throw new Exception($"Request Failed - Status Code: {res.StatusCode}");

            return true;
        }

        private TEntity DeserializeEntity(IRestResponse response)
        {
            if (!response.IsSuccessful)
                throw new Exception($"Request Failed - Status Code: {response.StatusCode}");

            var obj = JsonConvert.DeserializeObject<TEntity>(response.Content);
            return obj;
        }
    }
}