using UnityEngine;
using UnityEditor;
using SimpleJSON;

/// <summary>
/// PushbulletDeviceEditorWindow class.
/// Downloads and displays the user's device data.
/// </summary>
public class PushbulletDeviceEditorWindow : EditorWindow
{

	#region Properties

	/// <summary>
	/// The cached window reference.
	/// </summary>
	static PushbulletDeviceEditorWindow window;

	/// <summary>
	/// The phone icon.
	/// </summary>
	const string PHONE_ICON = "Assets/Shgoedt/PushbulletUnity/Scripts/Editor/Icons/phone.png";
	/// <summary>
	/// The browser icon.
	/// </summary>
	const string BROWSER_ICON = "Assets/Shgoedt/PushbulletUnity/Scripts/Editor/Icons/browser.png";

	/// <summary>
	/// The www.
	/// </summary>
	WWW www;

	JSONNode deviceData;

	#endregion

	#region Static Methods

	/// <summary>
	/// Opens the Pushbullet Device editor.
	/// </summary>
	[MenuItem( "Pushbullet/Devices...", false, 0 )]
	static void OpenPushbulletDeviceEditor()
	{
		window = EditorWindow.GetWindow<PushbulletDeviceEditorWindow>( "Pushbullet", true );
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
		if( !www.isDone ) return;

		EditorGUILayout.BeginVertical( new GUIStyle( "AS TextArea" ) );
		for( int i = 0; i < deviceData["devices"].Count; ++i )
		{
			JSONNode device = deviceData["devices"][i];
			if( device["active"].AsBool )
				DrawDeviceBlock( device );
		}
		EditorGUILayout.EndVertical();
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Downloads the device data.
	/// </summary>
	public void DownloadDeviceData()
	{
		// If the download is done...
		if( www.isDone )
		{
			Debug.Log( ( JSONNode.Parse( www.text ).ToString( "" ) ) );
			// remove it from the delegate.
			EditorApplication.update -= DownloadDeviceData;
			// Process the download.
			OnDownloadComplete();
		}
	}
	
	/// <summary>
	/// Called after the window is created.
	/// </summary>
	public void OnCreate()
	{
		EditorApplication.update += DownloadDeviceData;
		www = new WWW( Pushbullet.HOST + "/devices", null, Pushbullet.header );
	}

	/// <summary>
	/// Called after the download is complete.
	/// </summary>
	public void OnDownloadComplete()
	{
		deviceData = JSONNode.Parse( www.text );
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Draws the device block.
	/// </summary>
	/// <param name="device">Device.</param>
	private void DrawDeviceBlock( JSONNode device )
	{
		GUILayout.Space( 3f );
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 3f );
		EditorGUILayout.BeginHorizontal( "AS TextArea", GUILayout.Height( 32 ), GUILayout.ExpandHeight( false ) );

		string type = CleanJSONNode( device["type"] );
		if( type == "ios" || type == "android" )
			EditorGUILayout.LabelField(
				new GUIContent( Resources.LoadAssetAtPath<Texture2D>( PHONE_ICON ) as Texture2D ),
				GUILayout.Height( 64 ), GUILayout.Width( 64 ) );
		else
			EditorGUILayout.LabelField(
				new GUIContent( Resources.LoadAssetAtPath<Texture2D>( BROWSER_ICON ) as Texture2D ),
				GUILayout.Height( 64 ), GUILayout.Width( 64 ) );

		GUILayout.Space( 3f );

		EditorGUILayout.BeginVertical( GUILayout.ExpandHeight( false ) );
		EditorGUILayout.LabelField( CleanJSONNode( device["nickname"] ), EditorStyles.boldLabel );

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel( "Device Identifier: " );
		EditorGUILayout.SelectableLabel( CleanJSONNode( device["iden"] ) );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();
		GUILayout.Space( 3f );
		EditorGUILayout.EndHorizontal();
		GUILayout.Space( 3f );
	}

	/// <summary>
	/// Cleans the JSON node.
	/// </summary>
	/// <returns>The JSON node.</returns>
	/// <param name="node">Node.</param>
	private string CleanJSONNode( JSONNode node )
	{
		return node.ToString().Trim('"');
	}

	#endregion

}
