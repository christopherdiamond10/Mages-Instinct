using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Spawner : MonoBehaviour 
{
	public float					m_fSpawnCooldownMin			= 1.5f;
	public float					m_fSpawnCooldownMax			= 3.5f;
	public int						m_iChanceToSpawnStoopling	= 80;
	public int						m_iChanceToSpawnLaceling	= 40;
	public int						m_iChanceToSpawnRomling		= 40;
	public Transform				m_tSpawnParent;

	private TimeTracker m_ttSpawnCooldownTimer;
	private List<GameObject> m_lgoSpawnObjects;

	private static int sm_iSpawnQueueCount = 0;									// This is used in conjunction with the Current Enemy Count to see how many additional enemies need to be spawned


	// Use this for initialization
	void Start()
	{
		m_ttSpawnCooldownTimer = new TimeTracker( GetSpawnCooldownTime() );
		m_ttSpawnCooldownTimer.ForceTimerFinish();

		m_lgoSpawnObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (m_ttSpawnCooldownTimer.TimeUp())
		{
			if (m_lgoSpawnObjects.Count > 0)
			{
				GameObject NewSpawn = (Instantiate(m_lgoSpawnObjects[0]) as GameObject);	// Spawn Object
				NewSpawn.transform.position = transform.position;							// Set Position
				NewSpawn.transform.parent = m_tSpawnParent;									// Set Parent

				m_lgoSpawnObjects.RemoveAt(0);												// Pop Front Element on Spawn List
				sm_iSpawnQueueCount -= 1;													// Decrease Spawn Queue Count

				m_ttSpawnCooldownTimer.Reset();												// Reset Cooldown Timer
				m_ttSpawnCooldownTimer.FinishTime = GetSpawnCooldownTime();					// Modify Cooldown Time
			}
		}

		else
		{
			m_ttSpawnCooldownTimer.Update();
		}
	}

	public bool CanSpawnHere(Enemy_Base.TypeOfEnemy eEnemyType)
	{
		switch (eEnemyType)
		{
			case Enemy_Base.TypeOfEnemy.STOOPLING:	return Random.Range(0, 100) < m_iChanceToSpawnStoopling;
			case Enemy_Base.TypeOfEnemy.LACELING:	return Random.Range(0, 100) < m_iChanceToSpawnLaceling;
			case Enemy_Base.TypeOfEnemy.ROMLING:	return Random.Range(0, 100) < m_iChanceToSpawnRomling;
			default:								return false;
		}
	}

	private float GetSpawnCooldownTime()
	{
		return Random.Range(m_fSpawnCooldownMin, m_fSpawnCooldownMax);
	}

	public void AddToSpawnList(GameObject NewSpawn)
	{
		m_lgoSpawnObjects.Add(NewSpawn);
		sm_iSpawnQueueCount += 1;
	}

	public static int GetSpawnQueueCount()
	{
		return sm_iSpawnQueueCount;
	}
}
