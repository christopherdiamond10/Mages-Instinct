using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rita_ScreenMagicManager : MonoBehaviour
{
	public Fireball_ScreenAttackMagicPooling	m_rMagicPool				= null;
	public int									m_iTileSpawnChance			= 25;
	public float								m_fRevealPieceInitialTime	= 1.0f;

	private int									m_iTileSpawnProbability		= 5;
	private int									m_iCurrentAttackElement		= 0;
	private Player_Rita							m_rPlayerRef				= null;
	private List<Fireball_ScreenAttackMagic>	m_lgoScreenAttackObjects	= new List<Fireball_ScreenAttackMagic>();
	private TimeTracker							m_ttRevealTimer				= null;
	private TimeTracker							m_ttExplodeWaitTimer		= null;
	private ScreenAttackState					m_eScreenAttackState		= ScreenAttackState.IDLE;


	private enum ScreenAttackState
	{
		IDLE,
		REVEALING,
		WAIT_FOR_PLAYER_ANIMATION,
		DAMAGE,
		RESET,
	}

	// Use this for initialization
	void Start()
	{
		m_rPlayerRef = GetComponent<Player_Rita>();
		m_iTileSpawnProbability = Mathf.CeilToInt((float)((LevelGrid.TileCountHorz - LevelGrid.TileHorzOffsetCount) % 5));
		m_ttRevealTimer = new TimeTracker(m_fRevealPieceInitialTime);
		m_ttExplodeWaitTimer = new TimeTracker(1.2f);
	}

	// Update is called once per frame
	void Update()
	{
		switch (m_eScreenAttackState)
		{
			case ScreenAttackState.REVEALING:
			{
				UpdateRevealingState();
				break;
			}

			case ScreenAttackState.WAIT_FOR_PLAYER_ANIMATION:
			{
				UpdateWaitingState();
				break;
			}

			case ScreenAttackState.DAMAGE:
			{
				UpdateDamageState();
				break;
			}

			case ScreenAttackState.RESET:
			{
				UpdateResetState();
				break;
			}

			default:
			{
				break;
			}
		}
	}

	private void UpdateRevealingState()
	{
		m_ttRevealTimer.Update();
		if (m_ttRevealTimer.TimeUp())
		{
			m_ttRevealTimer.FinishTime -= (m_ttRevealTimer.FinishTime * 0.1f);
			m_ttRevealTimer.Reset();

			Fireball_ScreenAttackMagic rAttackObject = m_lgoScreenAttackObjects[m_iCurrentAttackElement];
			rAttackObject.gameObject.SetActive(true);
			rAttackObject.BeginAttack();

			m_iCurrentAttackElement += 1;
			if (m_iCurrentAttackElement == m_lgoScreenAttackObjects.Count)
			{
				m_eScreenAttackState = ScreenAttackState.WAIT_FOR_PLAYER_ANIMATION;
				m_rPlayerRef.BeginScreenAttackAnimation();
			}
		}
	}

	private void UpdateWaitingState()
	{
		if (m_rPlayerRef.ScreenAttackAnimationCompleted())
		{
			m_eScreenAttackState = ScreenAttackState.DAMAGE;
			foreach (Fireball_ScreenAttackMagic FSAM in m_lgoScreenAttackObjects)
			{
				FSAM.EnterExplodingPhase();
			}
		}
	}

	private void UpdateDamageState()
	{
		m_ttExplodeWaitTimer.Update();
		if (m_ttExplodeWaitTimer.TimeUp())
		{
			m_ttExplodeWaitTimer.Reset();
			m_eScreenAttackState = ScreenAttackState.RESET;
		}
	}

	private void UpdateResetState()
	{
		m_rMagicPool.Reset();
		m_rPlayerRef.EndScreenAttackAnimation();
		m_eScreenAttackState = ScreenAttackState.IDLE;
	}

	public void BeginScreenAttack()
	{
		// Reset from previous Screen Attack
		m_iCurrentAttackElement = 0;
		m_lgoScreenAttackObjects.Clear();
		m_ttRevealTimer.FinishTime = m_fRevealPieceInitialTime;
		m_eScreenAttackState = ScreenAttackState.REVEALING;


		int xMax = (LevelGrid.TileCountHorz - LevelGrid.TileHorzOffsetCount);
		int yMax = (LevelGrid.TileCountVertz - LevelGrid.TileVertzOffsetCount) - 1; // -1 For Array Index Offset
		
		int ixSpawnTile = 0;
		int iySpawnTile = (int)((float)LevelGrid.TileCountVertz * 0.5f);
		
		for (int y = 0; y < yMax; ++y)
		{
			if (ixSpawnTile == xMax)
			{
				ixSpawnTile = 0;
			}

			for (int x = Mathf.Abs(LevelGrid.TileHorzBeginID); x <= xMax; ++x)
			{
				// If Guaranteed Spawn, or Spawn Chance hit 
				if ( (x == ixSpawnTile) || ((xMax - x) == ixSpawnTile) || (y == iySpawnTile) || (Random.Range(0, 100) < m_iTileSpawnChance) )
				{
					Fireball_ScreenAttackMagic rAttackObject = m_rMagicPool.GetFreeObjectInList();
					rAttackObject.transform.position = LevelGrid.GetTile(x, y).transform.position;
					m_lgoScreenAttackObjects.Add(rAttackObject);
				}
			}
			
			ixSpawnTile += 1;
		}


		// Shuffle Screen Attack Objects List, so when iterating through later they appear randomly.
		int iRemainingElements = m_lgoScreenAttackObjects.Count;
		while (iRemainingElements > 1)
		{
			int k = (Random.Range(0, iRemainingElements));
			iRemainingElements -= 1;
			Fireball_ScreenAttackMagic value = m_lgoScreenAttackObjects[k];
			m_lgoScreenAttackObjects[k] = m_lgoScreenAttackObjects[iRemainingElements];
			m_lgoScreenAttackObjects[iRemainingElements] = value;
		}
	}
}
