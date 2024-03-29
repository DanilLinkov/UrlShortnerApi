﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace UrlShortner.KeyGenerators
{
    public class KeyResponse
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int Size { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime TakenDate { get; set; }
    }
    
    public class KeysResponse
    {
        public string[] Key { get; set; }
    }
    
    public class KeyGenerator: IKeyGenerator
    {
        private readonly string _apiUrl;
        private readonly string _apiKey;

        public KeyGenerator(string apiUrl, string apiKey)
        {
            _apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }
        
        public async Task<string> GenerateKeyAsync(int size)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("API-KEY", _apiKey);
            
            var response = await client.GetAsync($"{_apiUrl}/api/key?size=${size}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<KeyResponse>();
                
                return result.Key;
            }

            return null;
        }

        public async Task<string[]> GenerateKeyAsync(int count, int size)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("API-KEY", _apiKey);
            
            var response = await client.GetAsync($"{_apiUrl}/api/keys/${count}?size={size}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<KeyResponse[]>();
                
                return result.Select(x => x.Key).ToArray();
            }

            return null;
        }
    }
}