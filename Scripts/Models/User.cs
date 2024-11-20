using System;
using Newtonsoft.Json;

namespace Indie
{
    [Serializable]
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("aud")]
        public string Audience { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("confirmation_sent_at")]
        public DateTime? ConfirmationSentAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("app_metadata")]
        public AppMetadata AppMetadata { get; set; }

        [JsonProperty("user_metadata")]
        public UserMetadata UserMetadata { get; set; }

        [JsonProperty("identities")]
        public Identity[] Identities { get; set; }

        [JsonProperty("is_anonymous")]
        public bool IsAnonymous { get; set; }

        public DateTime? LastSignInAt => Identities?.Length > 0 ? Identities[0].LastSignInAt : null;
    }

    [Serializable]
    public class AppMetadata
    {
        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("providers")]
        public string[] Providers { get; set; }
    }

    [Serializable]
    public class UserMetadata
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("phone_verified")]
        public bool PhoneVerified { get; set; }

        [JsonProperty("sub")]
        public string Sub { get; set; }
    }

    [Serializable]
    public class Identity
    {
        [JsonProperty("identity_id")]
        public string IdentityId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("identity_data")]
        public IdentityData IdentityData { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("last_sign_in_at")]
        public DateTime LastSignInAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    [Serializable]
    public class IdentityData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("phone_verified")]
        public bool PhoneVerified { get; set; }

        [JsonProperty("sub")]
        public string Sub { get; set; }
    }
} 