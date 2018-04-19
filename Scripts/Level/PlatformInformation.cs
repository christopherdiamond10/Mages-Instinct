using UnityEngine;
using System.Collections;

public class PlatformInformation : MonoBehaviour 
{
	public enum MovementDirection
	{
		FACING,				// Move in the Same Direction You Are Facing
		LEFT,				// Move to the Left
		RIGHT,				// Move to the Right
		TOWARDS_PLAYER,		// Move Towards Player
		RANDOM,				// Move Either to the Left or Right
	};

	public MovementDirection m_eDirectionType	= MovementDirection.FACING;		// Which Direction The Enemy Will Now Face Once Landing on this Platform
	public bool				 m_bBreakable		= true;							// This Platform is Breakable?  For Romling Enemies
	public bool				 m_bCanDash			= true;							// This Platform Allows Dash?	For Laceling Enemies
}
