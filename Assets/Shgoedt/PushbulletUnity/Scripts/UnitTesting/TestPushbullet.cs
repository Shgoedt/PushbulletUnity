using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// TestPushbullet class.
/// </summary>
public class TestPushbullet : MonoBehaviour
{
	
	public string title;
	[Multiline]
	public string body;

	public bool sendToDevice;
	public string targetDevice;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{

	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if( Input.GetKeyDown( KeyCode.Space ) )
		{

		}
		if( Input.GetKeyDown( KeyCode.Return ) )
		{
			if( sendToDevice )
				Pushbullet.Push( title, body, targetDevice );
			else
				Pushbullet.Push( title, body );
		}
	}
}
