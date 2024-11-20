using UnityEditor;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Indie.Supabase.Editor
{
    public class ProjectManagementWindow : EditorWindow
    {
        #region Private Fields
        private string m_ProjectName;
        private string m_VectorData;
        private string m_Response;
        private Vector2 m_ScrollPosition;
        private Project[] m_Projects;
        private VectorStore[] m_VectorStores;
        #endregion

        #region Menu Item
        [MenuItem("Tools/Indie/Project Management")]
        public static void ShowWindow()
        {
            GetWindow<ProjectManagementWindow>("Project Management");
        }
        #endregion

        #region OnGUI
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Project Management", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            DrawProjectSection();
            EditorGUILayout.Space(10);

            DrawVectorStoreSection();
            EditorGUILayout.Space(10);

            DrawResponseSection();
        }
        #endregion

        #region Project Section
        private void DrawProjectSection()
        {
            GUILayout.Label("Projects", EditorStyles.boldLabel);
            m_ProjectName = EditorGUILayout.TextField("Project Name", m_ProjectName);

            if (GUILayout.Button("Create Project"))
            {
                CreateProject();
            }

            if (GUILayout.Button("Retrieve Projects"))
            {
                RetrieveProjects();
            }

            if (m_Projects != null)
            {
                foreach (var project in m_Projects)
                {
                    GUILayout.Label($"Project: {project.ProjectName} (ID: {project.Id})");
                }
            }
        }

        private async void CreateProject()
        {
            m_Response = "Creating project...";
            try
            {
                var newProject = new Project
                {
                    UserId = Guid.Parse(SupabaseREST.CurrentUser.Id),
                    ProjectName = m_ProjectName,
                    CreatedAt = DateTime.UtcNow
                };
                await new SupabaseTable<Project>("projects").Insert(newProject);
                m_Response = "Project created successfully.";
            }
            catch (Exception ex)
            {
                m_Response = $"Failed to create project: {ex.Message}";
            }
            Repaint();
        }

        private async void RetrieveProjects()
        {
            m_Response = "Retrieving projects...";
            try
            {
                m_Projects = await new SupabaseTable<Project>("projects").GetAll();
                m_Response = "Projects retrieved successfully.";
            }
            catch (Exception ex)
            {
                m_Response = $"Failed to retrieve projects: {ex.Message}";
            }
            Repaint();
        }
        #endregion

        #region Vector Store Section
        private void DrawVectorStoreSection()
        {
            GUILayout.Label("Vector Stores", EditorStyles.boldLabel);
            m_VectorData = EditorGUILayout.TextField("Vector Data (JSON)", m_VectorData);

            if (GUILayout.Button("Create Vector Store"))
            {
                CreateVectorStore();
            }

            if (m_VectorStores != null)
            {
                foreach (var vectorStore in m_VectorStores)
                {
                    GUILayout.Label($"Vector Store ID: {vectorStore.Id} (Project ID: {vectorStore.ProjectId})");
                }
            }
        }

        private async void CreateVectorStore()
        {
            m_Response = "Creating vector store...";
            try
            {
                var newVectorStore = new VectorStore
                {
                    ProjectId = m_Projects[0].Id, // Assuming the first project for simplicity
                    VectorData = JObject.Parse(m_VectorData),
                    CreatedAt = DateTime.UtcNow
                };
                await new SupabaseTable<VectorStore>("vector_stores").Insert(newVectorStore);
                m_Response = "Vector store created successfully.";
            }
            catch (Exception ex)
            {
                m_Response = $"Failed to create vector store: {ex.Message}";
            }
            Repaint();
        }
        #endregion

        #region Response Section
        private void DrawResponseSection()
        {
            GUILayout.Label("Response:", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(m_Response, GUILayout.Height(100));
        }
        #endregion
    }
} 