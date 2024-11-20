using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Indie
{
    public static class SupabaseREST
    {
        #region Private Fields
        private static string s_SupabaseUrl;
        private static string s_SupabaseKey;
        private static HttpClient s_HttpClient;
        private const string c_AuthEndpoint = "/auth/v1";
        private static AuthResponse s_CurrentSession;
        #endregion

        #region Public Properties
        public static bool IsInitialized { get; private set; }
        public static bool IsAuthenticated => s_CurrentSession != null;
        public static User CurrentUser => s_CurrentSession?.User;
        #endregion

        #region Initialization
        public static async Task Initialize()
        {
            if (IsInitialized) return;

            LoadConfiguration();
            SetupHttpClient();
            
            IsInitialized = true;
            Debug.Log("Supabase initialized successfully");
        }

        private static void LoadConfiguration()
        {
            s_SupabaseUrl ="https://nsvffitxxnupheynzlnc.supabase.co";
            s_SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5zdmZmaXR4eG51cGhleW56bG5jIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzEwOTgxOTcsImV4cCI6MjA0NjY3NDE5N30.NuuSAkNtTuHyNH4tf5gOTxnuEarSVDlZZQtwlChDLY0";
            
            if (string.IsNullOrEmpty(s_SupabaseUrl) || string.IsNullOrEmpty(s_SupabaseKey))
            {
                throw new Exception("Supabase configuration missing. Please check .env file");
            }
        }

        private static void SetupHttpClient()
        {
            s_HttpClient = new HttpClient
            {
                BaseAddress = new Uri(s_SupabaseUrl)
            };
            s_HttpClient.DefaultRequestHeaders.Add("apikey", s_SupabaseKey);
            s_HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {s_SupabaseKey}");
        }
        #endregion

        #region Database Operations
        public static async Task<string> Get(string endpoint)
        {
            EnsureInitialized();
            
            try
            {
                var response = await s_HttpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GET request failed: {ex.Message}");
                throw;
            }
        }

        public static async Task<string> Post(string endpoint, string jsonContent)
        {
            EnsureInitialized();
            
            try
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await s_HttpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"POST request failed: {ex.Message}");
                throw;
            }
        }

        public static async Task<string> Put(string endpoint, string jsonContent)
        {
            EnsureInitialized();
            
            try
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await s_HttpClient.PutAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"PUT request failed: {ex.Message}");
                throw;
            }
        }

        public static async Task<string> Delete(string endpoint)
        {
            EnsureInitialized();
            
            try
            {
                var response = await s_HttpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"DELETE request failed: {ex.Message}");
                throw;
            }
        }

        public static async Task<string> GetTableNames()
        {
            EnsureInitialized();
            
            try
            {
                // Query the information schema for table names
                var endpoint = "/rest/v1/";
                var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
                var response = await s_HttpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                
                // Parse and format the response
                var tables = JsonConvert.DeserializeObject<List<string>>(jsonResponse);
                
                // Build a formatted string
                var formattedResponse = new StringBuilder();
                formattedResponse.AppendLine("Database Tables:");
                formattedResponse.AppendLine("----------------");
                
                if (tables != null && tables.Count > 0)
                {
                    foreach (var tableName in tables)
                    {
                        formattedResponse.AppendLine($"• {tableName}");
                    }
                }
                else
                {
                    formattedResponse.AppendLine("No tables found in the database.");
                }
                
                return formattedResponse.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to fetch table names: {ex.Message}");
                throw;
            }
        }
        #endregion

    

        #region Helper Methods
        private static void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new Exception("Supabase is not initialized. Call Initialize() first.");
            }
        }
        #endregion

        #region Files Operations
        public static async Task<string> GetFiles(int limit)
        {
            EnsureInitialized();

            try
            {
                // Define the endpoint for fetching files
                var endpoint = $"/storage/v1/object/list?limit={limit}";
                var response = await s_HttpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Parse and format the response
                var files = JsonConvert.DeserializeObject<List<string>>(jsonResponse);
                var formattedResponse = new StringBuilder();
                formattedResponse.AppendLine("Fetched Files:");
                formattedResponse.AppendLine("----------------");

                if (files != null && files.Count > 0)
                {
                    foreach (var fileName in files)
                    {
                        formattedResponse.AppendLine($"• {fileName}");
                    }
                }
                else
                {
                    formattedResponse.AppendLine("No files found.");
                }

                return formattedResponse.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to fetch files: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Authentication
        public static async Task<AuthResponse> SignUp(string email, string password)
        {
            EnsureInitialized();

            var payload = new
            {
                email,
                password
            };

            var jsonContent = JsonConvert.SerializeObject(payload);
            Debug.Log(jsonContent);
            var response = await Post($"{c_AuthEndpoint}/signup", jsonContent);
            
            Debug.Log(response);

            s_CurrentSession = new AuthResponse();
            s_CurrentSession.User = JsonConvert.DeserializeObject<User>(response);

            
            if (s_CurrentSession?.User != null)
            {
                UpdateAuthHeader(s_CurrentSession.AccessToken);
            }

            return s_CurrentSession;
        }

        public static async Task<AuthResponse> SignIn(string email, string password)
        {
            EnsureInitialized();

            var payload = new
            {
                email,
                password
            };

            var jsonContent = JsonConvert.SerializeObject(payload);
            var response = await Post($"{c_AuthEndpoint}/token?grant_type=password", jsonContent);
            s_CurrentSession = JsonConvert.DeserializeObject<AuthResponse>(response);
            
            if (s_CurrentSession?.User != null)
            {
                UpdateAuthHeader(s_CurrentSession.AccessToken);
            }

            return s_CurrentSession;
        }

        public static void SignOut()
        {
            s_CurrentSession = null;
            UpdateAuthHeader(s_SupabaseKey); // Reset to anonymous key
        }

        private static void UpdateAuthHeader(string token)
        {
            // Remove existing Authorization header if present
            if (s_HttpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                s_HttpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            
            // Add new Authorization header
            s_HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        #endregion
    }
}
