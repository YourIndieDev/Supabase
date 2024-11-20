using UnityEngine;
using System.Threading.Tasks;

namespace Indie
{
    public class AuthExample : MonoBehaviour
    {
        private async void Start()
        {
            await SupabaseREST.Initialize();
            
            // Example of signing up
            try
            {
                var signUpResponse = await SupabaseREST.SignUp("test@example.com", "password123");
                Debug.Log($"Signed up user: {signUpResponse.User.Email}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Sign up failed: {e.Message}");
            }

            // Example of signing in
            try
            {
                var signInResponse = await SupabaseREST.SignIn("test@example.com", "password123");
                Debug.Log($"Signed in user: {signInResponse.User.Email}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Sign in failed: {e.Message}");
            }

            // Check if user is authenticated
            if (SupabaseREST.IsAuthenticated)
            {
                Debug.Log($"Currently logged in as: {SupabaseREST.CurrentUser.Email}");
            }

            // Sign out
            SupabaseREST.SignOut();
            Debug.Log("Signed out successfully");
        }
    }
} 