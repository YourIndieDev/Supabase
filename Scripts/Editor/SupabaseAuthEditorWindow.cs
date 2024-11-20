using UnityEngine;
using UnityEditor;
using Indie;
using System.Threading.Tasks;

namespace Indie
{
    public class SupabaseAuthEditorWindow : EditorWindow
    {
        private string email = "";
        private string password = "";
        private string signInMessage = "";
        private string sessionInfo = "";
        private string userInfo = "";
        private bool isInitialized = false;

        // Enum to define different auth methods
        private enum AuthMethod { EmailPassword, MagicLink, Google }
        private AuthMethod selectedAuthMethod = AuthMethod.EmailPassword;

        // Open the editor window from the Unity menu
        [MenuItem("Tools/Indie/Supabase Auth Test")]
        public static void ShowWindow()
        {
            GetWindow<SupabaseAuthEditorWindow>("Supabase Auth Test");
        }

        private void OnGUI()
        {
            GUILayout.Label("Supabase Authentication Test", EditorStyles.boldLabel);

            // Initialize button
            if (GUILayout.Button("Initialize Supabase Client"))
            {
                InitializeClient();
            }

            GUILayout.Space(10);

            // Disable other fields and buttons if not initialized
            EditorGUI.BeginDisabledGroup(!isInitialized);

            // Authentication method selector
            GUILayout.Label("Select Authentication Method:", EditorStyles.label);
            selectedAuthMethod = (AuthMethod)EditorGUILayout.EnumPopup(selectedAuthMethod);

            // Display input fields depending on the selected method
            if (selectedAuthMethod == AuthMethod.EmailPassword || selectedAuthMethod == AuthMethod.MagicLink)
            {
                email = EditorGUILayout.TextField("Email", email);
            }

            if (selectedAuthMethod == AuthMethod.EmailPassword)
            {
                password = EditorGUILayout.PasswordField("Password", password);
            }

            GUILayout.Space(10);

            // Buttons for each method
            if (selectedAuthMethod == AuthMethod.EmailPassword && GUILayout.Button("Sign Up with Email & Password"))
            {
                SignUpWithEmail();
            }

            if (selectedAuthMethod == AuthMethod.MagicLink && GUILayout.Button("Sign Up with Magic Link"))
            {
                SignUpWithMagicLink();
            }

            if (selectedAuthMethod == AuthMethod.Google && GUILayout.Button("Sign Up with Google"))
            {
                SignUpWithGoogle();
            }

            if (selectedAuthMethod == AuthMethod.EmailPassword && GUILayout.Button("Sign In with Email & Password"))
            {
                SignInWithEmail();
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);
            GUILayout.Label("Messages", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(signInMessage, MessageType.Info);

            GUILayout.Label("Session Info", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(sessionInfo, MessageType.None);

            GUILayout.Label("User Info", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(userInfo, MessageType.None);
        }

        private async void InitializeClient()
        {
            signInMessage = "Initializing Supabase client...";
            Repaint();

            SupabaseCSharp.Initialize();

            // Wait a moment for async operation to complete
            await Task.Delay(1000);

            signInMessage = "Supabase client initialized.";
            isInitialized = true;
            Repaint();
        }

        private async void SignUpWithEmail()
        {
            signInMessage = "Signing up with Email & Password...";
            Repaint();

            SupabaseCSharp.SignUp(email, password);

            await Task.Delay(1000);
            signInMessage = "Sign up attempt with Email & Password complete.";
            Repaint();
        }

        private async void SignUpWithMagicLink()
        {
            signInMessage = "Sending Magic Link...";
            Repaint();

            SupabaseCSharp.SignInOTP(email);

            await Task.Delay(1000);
            signInMessage = "Magic Link sent.";
            Repaint();
        }

        private async void SignUpWithGoogle()
        {
            signInMessage = "Signing up with Google...";
            Repaint();

            SupabaseCSharp.SignInOAuth();

            await Task.Delay(1000);
            signInMessage = "Google sign-up attempt complete.";
            Repaint();
        }

        private async void SignInWithEmail()
        {
            signInMessage = "Signing in with Email & Password...";
            Repaint();

            SupabaseCSharp.Signin(email, password);

            await Task.Delay(1000);
            signInMessage = "Sign in attempt with Email & Password complete.";
            Repaint();
        }

        private void GetSessionInfo()
        {
            var session = SupabaseCSharp.GetSession();
            if (session != null)
            {
                sessionInfo = $"Access Token: {session.AccessToken}\nExpires: {session.ExpiresIn}";
            }
            else
            {
                sessionInfo = "No active session.";
            }
            Repaint();
        }

        private void GetUserInfo()
        {
            var user = SupabaseCSharp.GetUser();
            if (user != null)
            {
                userInfo = $"User ID: {user.Id}\nEmail: {user.Email}\nRole: {user.Role}";
            }
            else
            {
                userInfo = "No user is signed in.";
            }
            Repaint();
        }
    }
}
