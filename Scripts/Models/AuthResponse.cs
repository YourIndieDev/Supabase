using System;
using Newtonsoft.Json;

namespace Indie
{
    [Serializable]
    public class AuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
} 