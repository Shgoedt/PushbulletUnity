using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System.Collections.Generic;

/// <summary>
/// PushbulletFeedEditorWindow class.
/// Downloads and displays the user's feed.
/// </summary>
public class PushbulletFeedEditorWindow : EditorWindow
{

	#region Properties

	/// <summary>
	/// The cached window reference.
	/// </summary>
	static PushbulletFeedEditorWindow window;

	/// <summary>
	/// The download query.
	/// </summary>
	WWW www;

	/// <summary>
	/// The feed data.
	/// </summary>
	JSONNode feedData;

	/// <summary>
	/// The scroll position.
	/// </summary>
	Vector2 scrollPosition = Vector2.zero;

	List<PushbulletFeedPost> posts = new List<PushbulletFeedPost>();
	PushbulletFeedPost selectedPost = null;

	string newPostTitle, newPostBody;

	#endregion

	#region Static Methods

	/// <summary>
	/// Opens the Pushbullet Device editor.
	/// </summary>
	[MenuItem( "Pushbullet/Feed", false, 0 )]
	static void OpenPushbulletDeviceEditor()
	{
		window = EditorWindow.GetWindow<PushbulletFeedEditorWindow>( true, "Pushbullet Feed", true );
		window.Show();
		window.OnCreate();
	}

	#endregion

	#region Built-in Methods

	/// <summary>
	/// Called every frame.
	/// </summary>
	void OnGUI()
	{
//		Repaint();

		if( !www.isDone || ( feedData == null || feedData["pushes"].Count == 0 ) )
		{
			EditorGUILayout.LabelField( "Downloading feed..." );
			return;
		}

		EditorGUILayout.BeginVertical( "AS TextArea", GUILayout.ExpandHeight( false ) );
		newPostTitle = EditorGUILayout.TextField( "Title:", newPostTitle );
		EditorGUILayout.PrefixLabel( "Body" );
		newPostBody = EditorGUILayout.TextArea( newPostBody );
		EditorGUILayout.EndVertical();

		if( GUILayout.Button( "Post" ) )
			Pushbullet.Push( newPostTitle, newPostBody );

		if( GUILayout.Button( "Refresh" ) ) window.OnCreate();

		scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition );
		EditorGUILayout.BeginVertical();
		for( int i = 0; i < posts.Count; ++i )
		{
			if( posts[i] == selectedPost )
				DrawFeedFull( posts[i] );
			else
				DrawFeedPreview( posts[i] );
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Downloads the feed data.
	/// </summary>
	public void DownloadFeedData()
	{
		// If the download is done...
		if( www.isDone )
		{
#if PUSHBULLET_DEBUG
			Debug.Log( ( JSONNode.Parse( www.text ).ToString( "" ) ) );
#endif
			// remove it from the delegate.
			EditorApplication.update -= DownloadFeedData;
			// Process the download.
			OnDownloadComplete();
		}
	}
	
	/// <summary>
	/// Called after the window is created.
	/// </summary>
	public void OnCreate()
	{
		EditorApplication.update += DownloadFeedData;
		www = new WWW( Pushbullet.HOST + "/pushes", null, Pushbullet.header );
		selectedPost = null;
	}

	/// <summary>
	/// Called after the download is complete.
	/// </summary>
	public void OnDownloadComplete()
	{
		feedData = JSONNode.Parse( www.text );
		for( int i = 0; i < feedData["pushes"].Count; ++i )
		{
			JSONNode note = feedData["pushes"][i];
			PushbulletFeedPost post = PushbulletFeedPost.Parse( note );

			if( post.type == "note" ) posts.Add( post );
		}
		Repaint();
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Draws the feed block.
	/// </summary>
	/// <param name="device">Device.</param>
	private void DrawFeedFull( PushbulletFeedPost post )
	{
		GUILayout.Space( 3f );
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 3f );
		GUI.backgroundColor = new Color( 0.8f, 0.8f, 0.8f );
		EditorGUILayout.BeginHorizontal( "AS TextArea" );
		GUI.backgroundColor = Color.white;
		GUILayout.Space( 3f );
		
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField( post.title, EditorStyles.boldLabel );
		EditorGUILayout.LabelField( post.body, EditorStyles.wordWrappedLabel );
		GUILayout.Space( 3f );
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();
		GUILayout.Space( 3f );
		EditorGUILayout.EndHorizontal();
		GUILayout.Space( 3f );
	}

	/// <summary>
	/// Draws the feed block.
	/// </summary>
	/// <param name="device">Device.</param>
	private void DrawFeedPreview( PushbulletFeedPost post )
	{
		GUILayout.Space( 3f );
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 3f );
		EditorGUILayout.BeginHorizontal( "AS TextArea", GUILayout.Height( 32 ), GUILayout.ExpandHeight( false ) );
		GUILayout.Space( 3f );

		EditorGUILayout.BeginVertical( GUILayout.ExpandHeight( false ) );
		EditorGUILayout.LabelField( "(" + post.type + ") "+ post.title, EditorStyles.boldLabel );

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( Cut( post.body, 24 ) );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

		if( GUILayout.Button( "See more" ) )
			selectedPost = post;

		EditorGUILayout.EndHorizontal();
		GUILayout.Space( 3f );
		EditorGUILayout.EndHorizontal();
		GUILayout.Space( 3f );
	}

	private string Cut( string line, int maxLength)
	{
		return line.Length > maxLength ? line.Substring(0, maxLength - 3) + "..." : line;
	}

	#endregion

}

public class PushbulletFeedPost
{

	public bool active = false;
	public string identifier = "";
	public string type = "";
	public string title = "";
	public string body = "";

	public PushbulletFeedPost() { }

	public static PushbulletFeedPost Parse( JSONNode node )
	{
		PushbulletFeedPost output = new PushbulletFeedPost();
		output.active = NodeValueToBool( node["active"] );
		output.identifier = NodeValueToString( node["iden"] );
		output.type = NodeValueToString( node["type"] );
		output.title = NodeValueToString( node["title"] );
		output.body = NodeValueToString( node["body"] );
		return output;
	}

	/// <summary>
	/// Cleans the JSON node.
	/// </summary>
	/// <returns>The JSON node.</returns>
	/// <param name="node">Node.</param>
	private static string NodeValueToString( JSONNode node )
	{
		return node.ToString().Trim('"');
	}

	private static bool NodeValueToBool( JSONNode node )
	{
		bool v = false;
		if( bool.TryParse( NodeValueToString( node ), out v ) )
			return v;
		return !string.IsNullOrEmpty( NodeValueToString( node ) );
	}
}


