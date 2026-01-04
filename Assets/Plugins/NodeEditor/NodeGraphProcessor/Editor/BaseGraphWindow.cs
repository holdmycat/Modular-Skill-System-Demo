using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace GraphProcessor
{
	[System.Serializable]
	public abstract class BaseGraphWindow : EditorWindow
	{
		// File: BaseGraphWindow.cs
		// Summary: Base editor window wiring for loading/saving graph assets and hosting the graph view.
		// Note: Avoid serialization-heavy work in OnEnable due to Unity GC; graph init is deferred.

		protected VisualElement		rootView;
		protected BaseGraphView		graphView;

		[SerializeField]
		protected BaseGraph			graph;

		readonly string				graphWindowStyle = "GraphProcessorStyles/BaseGraphView";

		public bool					isGraphLoaded
		{
			get { return graphView != null && graphView.graph != null; }
		}

		bool						reloadWorkaround = false;

		public event Action< BaseGraph >	graphLoaded;
		public event Action< BaseGraph >	graphUnloaded;

		/// <summary>
		/// Called when the window is enabled/opened (initial open, recompile, or Play Mode).
		/// </summary>
		protected virtual void OnEnable()
		{
			InitializeRootView();
			graphLoaded = baseGraph => { baseGraph?.OnGraphEnable(); }; 
			graphUnloaded = baseGraph => { baseGraph?.OnGraphDisable(); };
			// Avoid heavy serialization in OnEnable; Unity GC runs right after.
			reloadWorkaround = true;
			
			NodeGraphWindowHelper.AddNodeGraphWindow(this.graph, this);
		}

		protected virtual void Update()
		{
			// Workaround for the Refresh option of the editor window:
			// When Refresh is clicked, OnEnable is called before the serialized data in the
			// editor window is deserialized, causing the graph view to not be loaded
			if (reloadWorkaround && graph != null && !Application.isPlaying)
			{
				InitializeGraph(graph);
				reloadWorkaround = false;
			}
		}
		
		/// <summary>
		/// Called by Unity when the window is disabled (happens on domain reload)
		/// </summary>
		protected virtual void OnDisable()
		{
			if (graph != null && graphView != null)
			{
				graphView.SaveGraphToDisk();
				// Unload the graph
				graphUnloaded?.Invoke(this.graph);
				
				NodeGraphWindowHelper.RemoveNodeGraphWindow(this.graph);
			}
		}

		/// <summary>
		/// Called by Unity when the window is closed
		/// </summary>
		protected virtual void OnDestroy()
		{
			graphView?.Dispose();
		}

		void InitializeRootView()
		{
			rootView = base.rootVisualElement;

			rootView.name = "graphRootView";

			rootView.styleSheets.Add(Resources.Load<StyleSheet>(graphWindowStyle));
		}

		/// <summary>
		/// Initialize or reload the graph view with the given graph. Override to customize view creation.
		/// </summary>
		/// <param name="graph">Graph asset to load</param>
		public void InitializeGraph(BaseGraph graph)
		{
			if (this.graph != null && graph != this.graph)
			{
				// Save the graph to the disk
				GraphCreateAndSaveHelper.SaveGraphToDisk(this.graph);
				// Unload the graph
				graphUnloaded?.Invoke(this.graph);
			}

			graphLoaded?.Invoke(graph);
			this.graph = graph;

			if (graphView != null)
			{
				rootView.Remove(graphView);
			}

			InitializeWindow(graph);
			rootView.Add(graphView);
			
			graphView = rootView.Children().FirstOrDefault(e => e is BaseGraphView) as BaseGraphView;

			if (graphView == null)
			{
				Debug.LogError("GraphView has not been added to the BaseGraph root view !");
				return ;
			}

			graphView.Initialize(graph);

			InitializeGraphView(graphView);

			// TOOD: onSceneLinked...

			if (graph.IsLinkedToScene())
				LinkGraphWindowToScene(graph.GetLinkedScene());
			else
				graph.onSceneLinked += LinkGraphWindowToScene;
			
			reloadWorkaround = false;
		}

		void LinkGraphWindowToScene(Scene scene)
		{
			EditorSceneManager.sceneClosed += CloseWindowWhenSceneIsClosed;

			void CloseWindowWhenSceneIsClosed(Scene closedScene)
			{
				if (scene == closedScene)
				{
					Close();
					EditorSceneManager.sceneClosed -= CloseWindowWhenSceneIsClosed;
				}
			}
		}

		public virtual void OnGraphDeleted()
		{
			if (graph != null && graphView != null)
				rootView.Remove(graphView);

			graphView = null;
		}

		/// <summary>
		/// Initialize the editor window based on the BaseGraph and customize the BaseGraphView.
		/// </summary>
		/// <param name="graph"></param>
		protected abstract void	InitializeWindow(BaseGraph graph);
		
		/// <summary>
		/// Called after BaseGraphView initialization to perform additional customization.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void InitializeGraphView(BaseGraphView view) {}
	}
}
