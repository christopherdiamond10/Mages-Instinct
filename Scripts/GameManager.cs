using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Static Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static float	sm_fPreviousTime	= 0.0f;		// Previous Update Time
	private static float	sm_fCurrentTime		= 0.0f;		// Current Update Time
	private static float	sm_fDeltaTime		= 0.0f;		// Realtime DeltaTime, Alternative to Time.deltaTime
	private static bool		sm_bFrozenLevel		= false;	// When Player is Performing Screen Attack
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Attr_Readers
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static float DeltaTime		{ get { return sm_fDeltaTime;	} }
	public static bool IsFrozenLevel	{ get { return sm_bFrozenLevel; } }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Derived Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		UnFreezeLevel();
		UpdateDeltaTIme();
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Derived Method: Update
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		UpdateDeltaTIme();
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Static Method: Update Delta Time
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void UpdateDeltaTIme()
	{
		sm_fPreviousTime	= sm_fCurrentTime;
		sm_fCurrentTime		= Time.realtimeSinceStartup;
		sm_fDeltaTime		= (sm_fCurrentTime - sm_fPreviousTime);
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Static Method: Freeze Level
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void FreezeLevel()
	{
		sm_bFrozenLevel = true;
		AI_Manager.FreezeAllEnemies();
		Time.timeScale = 0.01f;
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Static Method: Unfreeze Level
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void UnFreezeLevel()
	{
		sm_bFrozenLevel = false;
		AI_Manager.UnFreezeAllEnemies();
		Time.timeScale = 1.0f;
	}
}
