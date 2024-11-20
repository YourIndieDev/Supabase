using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supabase;
using UnityEngine.PlayerLoop;
using System;
using static Supabase.Gotrue.Constants;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;

namespace Indie 
{
    public static class SupabaseCSharp
    {
        private  static Supabase.Client supabase;
        private static Session session;
        private static Supabase.Gotrue.User user;
        private static bool isInitialized;

        public static async void Initialize()
        {
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5zdmZmaXR4eG51cGhleW56bG5jIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzEwOTgxOTcsImV4cCI6MjA0NjY3NDE5N30.NuuSAkNtTuHyNH4tf5gOTxnuEarSVDlZZQtwlChDLY0"; //Environment.GetEnvironmentVariable("SUPABASE_URL");
            var url = "https://nsvffitxxnupheynzlnc.supabase.co"; // Environment.GetEnvironmentVariable("SUPABASE_CLIENT_API_KEY");

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            supabase = new Supabase.Client(url, key, options);
            await supabase.InitializeAsync();

            isInitialized = true;

            supabase.Auth.AddStateChangedListener((sender, changed) =>
            {
                switch (changed)
                {
                    case AuthState.SignedIn:
                        Debug.Log("User signed in");
                        break;
                    case AuthState.SignedOut:
                        Debug.Log("User signed out");
                        break;
                    case AuthState.UserUpdated:
                        Debug.Log("User updated");
                        break;
                    case AuthState.PasswordRecovery:
                        Debug.Log("Password recovery");
                        break;
                    case AuthState.TokenRefreshed:
                        Debug.Log("Token refreshed");
                        break;
                }
            });
        }

        public static Session GetSession()
        {
            session = supabase.Auth.CurrentSession;
            return session;
        }

        public static Supabase.Gotrue.User GetUser()
        {
            user = supabase.Auth.CurrentUser;
            return user;
        }

        public static async Task<Supabase.Gotrue.User> UpdateUser()
        {
            // TODO: Update user attributes
            var attrs = new UserAttributes { Email = "new-email@example.com" };
            return await supabase.Auth.Update(attrs);
        }

        public static async void Signin(string email, string password)
        {
            session = await supabase.Auth.SignIn(email, password);
        }

        public static async void SignInOAuth()
        {
            var signInUrl = await supabase.Auth.SignIn(Provider.Google);
        }

        public static async void SignUp(string email, string password)
        {
            if (!isInitialized)
            {
                Debug.Log("Supabase client is not initialized. Please call Initialize() first. Initializing now...");
                Initialize();
                return;
            }

            try
            {
                session = await supabase.Auth.SignUp(email, password);
                Debug.Log("Sign up complete.");
            }
            catch (GotrueException ex)
            {
                Debug.LogError($"Sign up failed with GotrueException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Sign up failed with exception: {ex.Message}");
            }
        }


        public static async void SignInOTP(string email)
        {
            var options = new SignInOptions { }; //RedirectTo = "http://myredirect.example" };
            var didSendMagicLink = await supabase.Auth.SendMagicLink(email, options);
        }

        public static async void SignInEmail(string email, string password)
        {
            session = await supabase.Auth.SignIn(email, password);
        }
    }
}
