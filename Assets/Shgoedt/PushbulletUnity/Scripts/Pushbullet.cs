#define PUSHBULLET_DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pushbullet class.
/// Responsible for sending Pushbullet notes.
/// </summary>
public sealed class Pushbullet : MonoBehaviour
{

	#region Properties

	/// <summary>
	/// The URL.
	/// </summary>
	public const string HOST = "https://api.pushbullet.com/v2";
	
	/// <summary>
	/// The API key.
	/// </summary>
	public static string apiKey = "jafhxPkv1qGhbYzjZxJTbeEkQuNSyl9f";

	/// <summary>
	/// The authorization header.
	/// </summary>
	public static Dictionary<string, string> header = new Dictionary<string, string>()
	{
		{ "Authorization", "Bearer " + apiKey },
//		{ "Accept", "application/json" },
		{ "User-Agent", "PushbulletUnity" }
	};

	/// <summary>
	/// The instance.
	/// </summary>
	private static Pushbullet instance
	{
		get
		{
			// If the instance is unassigned...
			if( _instance == null )
			{
				// try to find it in the scene.
				Pushbullet pushbullet = Object.FindObjectOfType<Pushbullet>();
				// If no Pushbullet instance is found...
				if( pushbullet == null )
					// make a new GameObject with a Pushbullet component.
					_instance = new GameObject( "Pushbullet", typeof( Pushbullet ) ).GetComponent<Pushbullet>();
				else
					// assign the found Pushbullet instance.
					_instance = pushbullet;
			}
			// Return the cached instance.
			return _instance;
		}
	}
	/// <summary>
	/// The cached instance.
	/// </summary>
	private static Pushbullet _instance;
	
	#endregion

	#region Static Methods
	
	/// <summary>
	/// Pushes a new note with the specified title and body to all devices.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="body">Body.</param>
	public static void Push( string title, string body ) { Push( title, body, "" ); }
	/// <summary>
	/// Pushes a new note with the specified title and body to the speicified device.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="body">Body.</param>
	/// <param name="device">Device.</param>
	public static void Push( string title, string body, string device )
	{
		// Create a new form.
		WWWForm form = new WWWForm();
		// Add the fields.
		form.AddField( "api_key", apiKey );
		form.AddField( "device_iden", device );
		form.AddField( "type", "note" );
		form.AddField( "title", title );
		form.AddField( "body", body );

		// Call "Send" with the form as parameter on the Singleton.
		instance.StartCoroutine( instance.Send( form ) );
	}

	#endregion

	#region IEnumerators

	/// <summary>
	/// Send the specified form to the Pushbullet API.
	/// </summary>
	/// <param name="form">Form.</param>
	private IEnumerator Send( WWWForm form )
	{
		// Create a new WWW.
		WWW w = new WWW( HOST + "/pushes", form.data, header );

		// Wait for it to complete.
		yield return w;

		// If there was an error, log it.
		if( !string.IsNullOrEmpty( w.error ) )
			Debug.Log( w.error );

		// The push was successfully sent!
#if PUSHBULLET_DEBUG
		Debug.Log( w.text );
#endif
	}
	
	#endregion
}
