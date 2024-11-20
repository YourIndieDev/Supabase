using System;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace Indie
{
    public class SupabaseTable<T> where T : class
    {
        private readonly string _tableName;
        private readonly string _baseEndpoint;

        public SupabaseTable(string tableName)
        {
            _tableName = tableName;
            _baseEndpoint = $"/rest/v1/{_tableName}";
        }

        public async Task<T[]> GetAll()
        {
            var response = await SupabaseREST.Get(_baseEndpoint);
            return JsonConvert.DeserializeObject<T[]>(response);
        }

        public async Task<T> GetById(string id)
        {
            var response = await SupabaseREST.Get($"{_baseEndpoint}?id=eq.{id}");
            var items = JsonConvert.DeserializeObject<T[]>(response);
            return items.Length > 0 ? items[0] : null;
        }

        public async Task<T> Insert(T item)
        {
            var json = JsonConvert.SerializeObject(item);
            var response = await SupabaseREST.Post(_baseEndpoint, json);
            var items = JsonConvert.DeserializeObject<T[]>(response);
            return items[0];
        }

        public async Task<T> Update(string id, T item)
        {
            var json = JsonConvert.SerializeObject(item);
            var response = await SupabaseREST.Put($"{_baseEndpoint}?id=eq.{id}", json);
            var items = JsonConvert.DeserializeObject<T[]>(response);
            return items[0];
        }

        public async Task Delete(string id)
        {
            await SupabaseREST.Delete($"{_baseEndpoint}?id=eq.{id}");
        }
    }
} 