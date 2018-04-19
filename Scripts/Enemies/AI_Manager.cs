using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Manager : MonoBehaviour 
{
	public int m_iTotalEnemiesCount = 10;
	public GameObject m_goStooplingEnemy;
	public GameObject m_goLacelingEnemy;
	public GameObject m_goRomlingEnemy;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static int sm_iEnemyCount = 0;
	private static Dictionary<int, EnemyInfo> sm_lEnemyLookup = new Dictionary<int, EnemyInfo>();

	private AI_Spawner[] m_aAISpawners;

    // Enemy Pooling
    private Enemy_Stoopling[] m_aStooplingPool;
    private Enemy_Laceling[] m_aLacelingPool;
    private Enemy_Romling[] m_aRomlingPool;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private class EnemyInfo
	{
		public GameObject goEnemyObject  = null;
		public Enemy_Base pEnemyInstance = null;
		public Vector2	  vVelocity		 = new Vector2(0.0f, 0.0f);
	};



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		FindAISpawners();


	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		UpdateAISpawning();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update AI Spawning
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateAISpawning()
	{
		if ((GetEnemyCount() + AI_Spawner.GetSpawnQueueCount()) < m_iTotalEnemiesCount)
		{
			int iSpawnerID = Random.Range(0, m_aAISpawners.Length);
			m_aAISpawners[iSpawnerID].AddToSpawnList((Random.Range(0, 2) == 0 ? m_goLacelingEnemy : Random.Range(0, 2) == 0 ? m_goRomlingEnemy : m_goStooplingEnemy));
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Find AI Spawners
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void FindAISpawners()
	{
		List<AI_Spawner> lAISpawners = new List<AI_Spawner>();
		GameObject goLevelMap = GameObject.Find("Level Map");
		if (goLevelMap != null)
		{
			foreach (Transform Row in goLevelMap.transform)
			{
				foreach (Transform Col in Row)
				{
					if (Col.GetComponent<AI_Spawner>() != null)
					{
						lAISpawners.Add( Col.GetComponent<AI_Spawner>() );
					}
				}
			}
		}

		m_aAISpawners = new AI_Spawner[lAISpawners.Count];
		for (int i = 0; i < lAISpawners.Count; ++i)
		{
			m_aAISpawners[i] = lAISpawners[i];
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add AI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static int Add_AI(Enemy_Base pEnemy)
	{
		// Find an Empty Slot in Lookup
		int iNewIndex = -1;
		for (int i = 0; i < sm_lEnemyLookup.Count; ++i)
		{
			if (sm_lEnemyLookup[i] == null)
			{
				iNewIndex = i;
				break;
			}
		}

		// If None Found, Create A New One
		if (iNewIndex == -1)
		{
			iNewIndex = sm_lEnemyLookup.Count;
			sm_lEnemyLookup.Add(iNewIndex, null);
		}

		// Insert Item
		EnemyInfo pEnemyInfo = new EnemyInfo();
		pEnemyInfo.goEnemyObject = pEnemy.gameObject;
		pEnemyInfo.pEnemyInstance = pEnemy;
		sm_lEnemyLookup[iNewIndex] = pEnemyInfo;

		// Increment Enemy Count
		sm_iEnemyCount += 1;

		// Return Enemy ID
		return iNewIndex;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove AI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void RemoveAI(int ID)
	{
		if (ID >= 0 && sm_lEnemyLookup.Count > ID)
		{
			sm_lEnemyLookup[ID] = null;
			sm_iEnemyCount -= 1;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Enemy Special Abilities
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void ResetEnemySpecialties(int a_iPlatformCollisionID)
	{
		foreach (KeyValuePair<int, EnemyInfo> kvpEnemyInfo in sm_lEnemyLookup)
		{
			EnemyInfo pEnemy = kvpEnemyInfo.Value;
			if (pEnemy != null)
			{
				if (pEnemy.pEnemyInstance.PlatformCollisionID == a_iPlatformCollisionID)
				{
					pEnemy.pEnemyInstance.ResetSpecialty();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Enemy Count
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static int GetEnemyCount()
	{
		return sm_iEnemyCount;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get All Enemies
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static List<Enemy_Base> GetAllEnemies()
	{
		List<Enemy_Base> ReturnList = new List<Enemy_Base>();
		foreach (KeyValuePair<int, EnemyInfo> kvpEnemyInfo in sm_lEnemyLookup)
		{
			EnemyInfo pEnemy = kvpEnemyInfo.Value;
			if (pEnemy != null)
			{
				ReturnList.Add(pEnemy.pEnemyInstance);
			}
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Stoopling Enemies
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static List<Enemy_Stoopling> GetStooplingEnemies()
	{
		List<Enemy_Stoopling> ReturnList = new List<Enemy_Stoopling>();
		foreach (KeyValuePair<int, EnemyInfo> kvpEnemyInfo in sm_lEnemyLookup)
		{
			EnemyInfo pEnemy = kvpEnemyInfo.Value;
			if (pEnemy != null)
			{
				if (pEnemy.pEnemyInstance.EnemyType == Enemy_Base.TypeOfEnemy.STOOPLING)
				{
					ReturnList.Add(pEnemy.goEnemyObject.GetComponent<Enemy_Stoopling>());
				}
			}
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Laceling Enemies
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static List<Enemy_Laceling> GetLacelingEnemies()
	{
		List<Enemy_Laceling> ReturnList = new List<Enemy_Laceling>();
		foreach (KeyValuePair<int, EnemyInfo> kvpEnemyInfo in sm_lEnemyLookup)
		{
			EnemyInfo pEnemy = kvpEnemyInfo.Value;
			if (pEnemy != null)
			{
				if (pEnemy.pEnemyInstance.EnemyType == Enemy_Base.TypeOfEnemy.LACELING)
				{
					ReturnList.Add(pEnemy.goEnemyObject.GetComponent<Enemy_Laceling>());
				}
			}
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Romling Enemies
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static List<Enemy_Romling> GetRomlingEnemies()
	{
		List<Enemy_Romling> ReturnList = new List<Enemy_Romling>();
		foreach (KeyValuePair<int, EnemyInfo> kvpEnemyInfo in sm_lEnemyLookup)
		{
			EnemyInfo pEnemy = kvpEnemyInfo.Value;
			if (pEnemy != null)
			{
				if (pEnemy.pEnemyInstance.EnemyType == Enemy_Base.TypeOfEnemy.ROMLING)
				{
					ReturnList.Add(pEnemy.goEnemyObject.GetComponent<Enemy_Romling>());
				}
			}
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Freeze All Enemies
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void FreezeAllEnemies()
	{
		List<Enemy_Base> lEnemies = GetAllEnemies();
		foreach (Enemy_Base Enemy in lEnemies)
		{
				Enemy.IsFrozen = true;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: UnFreeze All Enemies
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void UnFreezeAllEnemies()
	{
		List<Enemy_Base> lEnemies = GetAllEnemies();
		foreach (Enemy_Base Enemy in lEnemies)
		{
			Enemy.IsFrozen = false;
		}
	}
}
