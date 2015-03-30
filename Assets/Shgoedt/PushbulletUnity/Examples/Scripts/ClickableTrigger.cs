using UnityEngine;
using System.Collections;

/// <summary>
/// ClickableTrigger class.
/// </summary>
[RequireComponent( typeof( BoxCollider ) )]
public class ClickableTrigger : MonoBehaviour
{

	public string title, body;

	/// <summary>
	/// Reset this instance.
	/// </summary>
	void Reset()
	{
		GetComponent<BoxCollider>().isTrigger = false;
	}

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
	
	}

	void OnMouseDown()
	{
		if( string.IsNullOrEmpty( title ) )
			title = name;
		if( string.IsNullOrEmpty( body ) )
			body = "was pressed!";

		Pushbullet.Push( title, body );
	}
}
