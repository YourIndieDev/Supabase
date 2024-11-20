using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Text;

namespace Indie.Supabase.Editor
{
    public class SupabaseEditorWindow : EditorWindow
    {
        #region Private Fields
        private string m_Endpoint;
        private string m_JsonContent;
        private string m_Response;
        private Vector2 m_ScrollPosition;
        private bool m_IsInitialized;
        private int m_FileLimit = 10;
        private string m_Email = "";
        private string m_Password = "";
        private bool m_ShowAuthentication = true;
        private bool m_ShowApiTesting = true;
        private bool m_ShowFileOperations = true;
        private GUIStyle m_HeaderStyle;
        private GUIStyle m_SectionStyle;
        #endregion

        #region Menu Item
        [MenuItem("Tools/Indie/Supabase")]
        public static void ShowWindow()
        {
            GetWindow<SupabaseEditorWindow>("Supabase Tester");
        }
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            // Initialize GUI Styles
            m_HeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                margin = new RectOffset(0, 0, 10, 10)
            };

            m_SectionStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, 5, 5)
            };
        }
        #endregion

        #region OnGUI
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Supabase API Tester", m_HeaderStyle);
            
            EditorGUILayout.Space(10);
            
            DrawInitializationSection();
            EditorGUILayout.Space(10);
            
            DrawAuthenticationSection();
            EditorGUILayout.Space(10);
            
            DrawFileOperationsSection();
            EditorGUILayout.Space(10);
            
            DrawApiTestingSection();
            EditorGUILayout.Space(10);
            
            DrawResponseSection();
        }
        #endregion

        #region GUI Sections
        private void DrawInitializationSection()
        {
            if (GUILayout.Button("Initialize Supabase"))
            {
                InitializeSupabase();
            }

            if (m_IsInitialized)
            {
                EditorGUILayout.HelpBox("Supabase is initialized", MessageType.Info);
            }
        }

        private void DrawAuthenticationSection()
        {
            m_ShowAuthentication = EditorGUILayout.Foldout(m_ShowAuthentication, "Authentication", m_SectionStyle);
            if (m_ShowAuthentication)
            {
                EditorGUI.indentLevel++;
                
                // Only enable authentication controls if initialized and not authenticated
                GUI.enabled = m_IsInitialized && !SupabaseREST.IsAuthenticated;
                
                m_Email = EditorGUILayout.TextField("Email", m_Email);
                m_Password = EditorGUILayout.PasswordField("Password", m_Password);
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Sign In"))
                {
                    SignIn();
                }
                if (GUILayout.Button("Sign Up"))
                {
                    SignUp();
                }
                EditorGUILayout.EndHorizontal();
                
                GUI.enabled = true;

                // Show current authentication status and user info
                if (SupabaseREST.IsAuthenticated)
                {
                    EditorGUILayout.HelpBox($"Authenticated as: {SupabaseREST.CurrentUser?.Email}", MessageType.Info);
                    
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Print User Info"))
                    {
                        PrintUserInfo();
                    }
                    if (GUILayout.Button("Sign Out"))
                    {
                        SignOut();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUI.indentLevel--;
            }
        }

        private void DrawFileOperationsSection()
        {
            m_ShowFileOperations = EditorGUILayout.Foldout(m_ShowFileOperations, "File Operations", m_SectionStyle);
            if (m_ShowFileOperations)
            {
                EditorGUI.indentLevel++;
                GUI.enabled = m_IsInitialized;
                
                m_FileLimit = EditorGUILayout.IntField("Number of Files to Fetch", m_FileLimit);
                if (GUILayout.Button("Fetch Files"))
                {
                    FetchFiles(m_FileLimit);
                }
                
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
        }

        private void DrawApiTestingSection()
        {
            m_ShowApiTesting = EditorGUILayout.Foldout(m_ShowApiTesting, "API Testing", m_SectionStyle);
            if (m_ShowApiTesting)
            {
                EditorGUI.indentLevel++;
                GUI.enabled = m_IsInitialized;
                
                m_Endpoint = EditorGUILayout.TextField("Endpoint", m_Endpoint);
                m_JsonContent = EditorGUILayout.TextArea(m_JsonContent, GUILayout.Height(60));
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("GET")) ExecuteGet();
                if (GUILayout.Button("POST")) ExecutePost();
                if (GUILayout.Button("PUT")) ExecutePut();
                if (GUILayout.Button("DELETE")) ExecuteDelete();
                EditorGUILayout.EndHorizontal();
                
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
        }

        private void DrawResponseSection()
        {
            GUILayout.Label("Response:", m_HeaderStyle);
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Height(200));
            EditorGUILayout.TextArea(m_Response, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region API Calls
        private async void InitializeSupabase()
        {
            m_Response = "Initializing...";
            try
            {
                await SupabaseREST.Initialize();
                m_IsInitialized = true;
                m_Response = "Supabase initialized successfully.";
            }
            catch (Exception ex)
            {
                m_Response = $"Failed to initialize Supabase: {ex.Message}";
            }
            Repaint();
        }

        private async void SignIn()
        {
            m_Response = "Signing in...";
            try
            {
                var response = await SupabaseREST.SignIn(m_Email, m_Password);
                m_Response = $"Signed in successfully as {response.User.Email}";
                m_Password = ""; // Clear password for security
            }
            catch (Exception ex)
            {
                m_Response = $"Sign in failed:\n{ex.Message}";
                Debug.LogError(ex);
            }
            Repaint();
        }

        private async void SignUp()
        {
            m_Response = "Signing up...";
            try
            {
                var response = await SupabaseREST.SignUp(m_Email, m_Password);
                m_Response = $"Signed up successfully as {response.User.Email}";
                m_Password = ""; // Clear password for security
            }
            catch (Exception ex)
            {
                m_Response = $"Sign up failed:\n{ex.Message}";
                Debug.LogError(ex);
            }
            Repaint();
        }

        private void SignOut()
        {
            SupabaseREST.SignOut();
            m_Response = "Signed out successfully";
            Repaint();
        }

        private async void FetchFiles(int limit)
        {
            m_Response = "Fetching files...";
            try
            {
                m_Response = await SupabaseREST.GetFiles(limit);
            }
            catch (Exception ex)
            {
                m_Response = $"Failed to fetch files: {ex.Message}";
                Debug.LogError(ex);
            }
            Repaint();
        }

        private async void ExecuteGet()
        {
            m_Response = "Getting data...";
            try
            {
                m_Response = await SupabaseREST.Get(m_Endpoint);
            }
            catch (Exception ex)
            {
                m_Response = $"GET request failed: {ex.Message}";
                Debug.LogError(ex);
            }
            Repaint();
        }

        private async void ExecutePost()
        {
            m_Response = "Posting data...";
            try
            {
                m_Response = await SupabaseREST.Post(m_Endpoint, m_JsonContent);
            }
            catch (Exception ex)
            {
                m_Response = $"POST request failed: {ex.Message}";
                Debug.LogError(ex);
            }
            Repaint();
        }

        private async void ExecutePut()
        {
            m_Response = "Updating data...";
            try
            {
                m_Response = await SupabaseREST.Put(m_Endpoint, m_JsonContent);
            }
            catch (Exception ex)
            {
                m_Response = $"PUT request failed: {ex.Message}";
                Debug.LogError(ex);
            }
            Repaint();
        }

        private async void ExecuteDelete()
        {
            m_Response = "Deleting data...";
            try
            {
                m_Response = await SupabaseREST.Delete(m_Endpoint);
            }
            catch (Exception ex)
            {
                m_Response = $"DELETE request failed: {ex.Message}";
                Debug.LogError(ex);
            }
            Repaint();
        }

        private void PrintUserInfo()
        {
            if (!SupabaseREST.IsAuthenticated || SupabaseREST.CurrentUser == null)
            {
                m_Response = "No user is currently authenticated.";
                return;
            }

            var user = SupabaseREST.CurrentUser;
            var sb = new StringBuilder();
            sb.AppendLine("Current User Information:");
            sb.AppendLine("------------------------");
            sb.AppendLine($"User ID: {user.Id}");
            sb.AppendLine($"Email: {user.Email}");
            sb.AppendLine($"Role: {user.Role}");
            sb.AppendLine($"Phone: {user.Phone ?? "Not provided"}");
            sb.AppendLine($"Created At: {user.CreatedAt.ToLocalTime()}");
            sb.AppendLine($"Updated At: {user.UpdatedAt.ToLocalTime()}");
            sb.AppendLine($"Is Anonymous: {user.IsAnonymous}");
            
            if (user.ConfirmationSentAt.HasValue)
            {
                sb.AppendLine($"Confirmation Sent At: {user.ConfirmationSentAt.Value.ToLocalTime()}");
            }

            if (user.LastSignInAt.HasValue)
            {
                sb.AppendLine($"Last Sign In: {user.LastSignInAt.Value.ToLocalTime()}");
            }

            sb.AppendLine("\nApp Metadata:");
            sb.AppendLine($"Provider: {user.AppMetadata.Provider}");
            sb.AppendLine($"Providers: {string.Join(", ", user.AppMetadata.Providers)}");

            sb.AppendLine("\nUser Metadata:");
            sb.AppendLine($"Email Verified: {user.UserMetadata.EmailVerified}");
            sb.AppendLine($"Phone Verified: {user.UserMetadata.PhoneVerified}");

            if (user.Identities?.Length > 0)
            {
                sb.AppendLine("\nIdentity Information:");
                var identity = user.Identities[0];
                sb.AppendLine($"Identity ID: {identity.IdentityId}");
                sb.AppendLine($"Provider: {identity.Provider}");
                sb.AppendLine($"Created At: {identity.CreatedAt.ToLocalTime()}");
                sb.AppendLine($"Last Sign In: {identity.LastSignInAt.ToLocalTime()}");
            }

            m_Response = sb.ToString();
            Repaint();
        }
        #endregion
    }
} 