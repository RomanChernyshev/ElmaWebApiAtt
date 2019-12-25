using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElmaWebApi.App.ExtentionAPI;
using ElmaWebApi.ElmaAPI.ElmaApiModels;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Auth;
using Newtonsoft.Json;

namespace ElmaWebApi.ElmaAPI.Managers
{
    /// <summary>
    /// Used to work with all available entities ELMA
    /// </summary>
    public class ElmaEntityManager : IService
    {
        private const string EntityServiceRoot = "Entity/";
        private readonly ElmaRestClient client;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="client">ELMA REST-client</param>
        public ElmaEntityManager(ElmaRestClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Insert new entity
        /// </summary>
        /// <returns>Unique entity identifier</returns>
        public async Task<long> Insert(ElmaEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var result = await client.Post($"{EntityServiceRoot}Insert/{obj.TypeUid}", ElmaAuthorizationService.AuthInfo, obj);
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Status code: {result.StatusCode}");
            }
            return JsonConvert.DeserializeObject<long>(await result.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Update existing object
        /// </summary>
        public async Task Update(ElmaEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var result = await client.Post($"{EntityServiceRoot}Update/{obj.TypeUid}/{obj.Id}", ElmaAuthorizationService.AuthInfo, obj);
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Status code: {result.StatusCode}");
            }
        }

        /// <summary>
        /// Query to match the specified object 
        /// </summary>
        /// <param name="obj">Specified object</param>
        /// <param name="authResponse">ELMA auth info</param>
        public Task<IEnumerable<T>> QueryByMatch<T>(T obj, AuthResponse authResponse) where T: ElmaEntity
        {
            return Query<T>(obj.TypeUid, EqlSerialize(obj), authResponse);
        }

        /// <summary>
        /// Query by uid
        /// </summary>
        /// <param name="typeUid">ELMA-entity type GUID</param>
        /// <param name="eql">EQL-filter string</param>
        /// <param name="authResponse">ELMA auth info</param>
        /// <returns>Result list</returns>
        public async Task<IEnumerable<T>> Query<T>(Guid typeUid, string eql, AuthResponse authResponse) where T : ElmaEntity
        {
            var result = await client.AutorizedGet($"{EntityServiceRoot}/Query?type={typeUid}&q={eql}", authResponse);
            if (result == null)
            {
                throw new InvalidOperationException();
            }

            return JsonConvert.DeserializeObject<IEnumerable<T>>(result);
        }

        private string EqlSerialize(ElmaEntity entity)
        {
            var allEntityProps = entity
                .GetType()
                .GetProperties();

            var builder = new StringBuilder();

            var stringProps = allEntityProps
                .Where(i => i.PropertyType == typeof(string) 
                    && i.GetValue(entity) != null);

            foreach (var propInfo in stringProps)
            {
                if (propInfo == stringProps.Last())
                {
                    builder.Append($"{propInfo.Name} = '{propInfo.GetValue(entity)}'");
                    break;
                }
                builder.Append($"{propInfo.Name} = '{propInfo.GetValue(entity)}' AND");
            }

            return builder.ToString();
        }
    }
}
