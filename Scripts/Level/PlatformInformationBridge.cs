using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformInformationBridge : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private GameObject[]			m_ConnectingPlatformObjects;
	private float[]					m_fPlatformXPositions;
	private PlatformInformation[]	m_PlatformGuides;
	private int						m_iClosestPlatform;
	private int						m_iPlatformCollisionBoxID;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Retrieve Platform Objects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void AddPlatformObjects(GameObject[] Platforms, int iPlatformCollisionBoxID)
	{
		m_ConnectingPlatformObjects = Platforms;
		m_fPlatformXPositions		= new float[Platforms.Length];
		m_PlatformGuides			= new PlatformInformation[Platforms.Length];
		m_iPlatformCollisionBoxID	= iPlatformCollisionBoxID;
		
		for (int i = 0; i < Platforms.Length; ++i)
		{
			m_fPlatformXPositions[i] = Platforms[i].transform.position.x;
			m_PlatformGuides[i] = Platforms[i].GetComponent<PlatformInformation>();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Closest Platform Object
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int GetClosestPlatform(Vector2 vPosition)
	{
		int iReturnIndex = 0;
		float fBestDistance = 10000.0f;

		for( int i = 0; i < m_fPlatformXPositions.Length; ++i )
		{
			float fDistance = Mathf.Abs(vPosition.x - m_fPlatformXPositions[i]);
			if (fDistance < fBestDistance)
			{
				fBestDistance = fDistance;
				iReturnIndex = i;
			}
		}
		return iReturnIndex;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Indexes of Platforms to Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<int> GetListOfPlatformsToLeftIndexes(int iStart)
	{
		List<int> ReturnList = new List<int>();
		for (int i = 0; i < iStart; ++i)
		{
			ReturnList.Add(i);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Indexes of Platforms to Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<int> GetListOfPlatformsToRightIndexes(int iStart)
	{
		List<int> ReturnList = new List<int>();
		for (int i = (iStart + 1); i < m_ConnectingPlatformObjects.Length; ++i)
		{
			ReturnList.Add(i);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Platforms to Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<GameObject> GetPlatformsToLeft()
	{
		List<GameObject> ReturnList = new List<GameObject>();
		List<int> lIndexes = GetListOfPlatformsToLeftIndexes(m_iClosestPlatform);
 		foreach(int i in lIndexes)
		{
			ReturnList.Add(m_ConnectingPlatformObjects[i]);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Platforms to Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<GameObject> GetPlatformsToRight()
	{
		List<GameObject> ReturnList = new List<GameObject>();
		List<int> lIndexes = GetListOfPlatformsToRightIndexes(m_iClosestPlatform);
 		foreach(int i in lIndexes)
		{
			ReturnList.Add(m_ConnectingPlatformObjects[i]);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Platform Positions to Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<float> GetPlatformPositionsToLeft()
	{
		List<float> ReturnList = new List<float>();
		List<int> lIndexes = GetListOfPlatformsToLeftIndexes(m_iClosestPlatform);
 		foreach(int i in lIndexes)
		{
			ReturnList.Add(m_fPlatformXPositions[i]);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Platform Positions to Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<float> GetPlatformPositionsToRight()
	{
		List<float> ReturnList = new List<float>();
		List<int> lIndexes = GetListOfPlatformsToRightIndexes(m_iClosestPlatform);
 		foreach(int i in lIndexes)
		{
			ReturnList.Add(m_fPlatformXPositions[i]);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Dashable Platforms To Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<int> GetDashablePlatformsToLeft(int iStart, int iEndOffset = 0)
	{
		List<int> ReturnList = new List<int>();
		for (int i = 0; i < (iStart + iEndOffset); ++i)
		{
			if(m_PlatformGuides[i].m_bCanDash)
				ReturnList.Add(i);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Dashable Platforms To Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<int> GetDashablePlatformsToRight(int iStart, int iStartOffset = 1)
	{
		List<int> ReturnList = new List<int>();
		for (int i = (iStart + iStartOffset); i < m_ConnectingPlatformObjects.Length; ++i)
		{
			if (m_PlatformGuides[i].m_bCanDash)
				ReturnList.Add(i);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get A Dash Target Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private KeyValuePair<bool, int> GetDashTargetPosition(int iCurrentPlatform, Enemy_Base.MovementDirection eDirection)
	{
		List<int> lIndexes = (eDirection == Enemy_Base.MovementDirection.LEFT) ? GetDashablePlatformsToLeft(iCurrentPlatform) : GetDashablePlatformsToRight(iCurrentPlatform);
		if (lIndexes.Count > 0)
		{
			int iPlatformID = lIndexes[ Random.Range(0, lIndexes.Count) ];
			return new KeyValuePair<bool, int>(true, iPlatformID);
		}
		else
		{
			return new KeyValuePair<bool,int>(false, 0);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Breakable Platforms To Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<int> GetBreakablePlatformsToLeft(int iStart, int iEndOffset = 0)
	{
		List<int> ReturnList = new List<int>();
		for (int i = 0; i < (iStart + iEndOffset); ++i)
		{
			if (m_PlatformGuides[i].m_bBreakable)
				ReturnList.Add(i);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Breakable Platforms To Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<int> GetBreakablePlatformsToRight(int iStart, int iStartOffset = 1)
	{
		List<int> ReturnList = new List<int>();
		for (int i = (iStart + iStartOffset); i < m_ConnectingPlatformObjects.Length; ++i)
		{
			if (m_PlatformGuides[i].m_bBreakable)
				ReturnList.Add(i);
		}
		return ReturnList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Random Position Offset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float GetRandomPositionOffset()
	{
		return Random.Range(0.0f, 0.3f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Assign Dash Target
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AssignDashTarget(Enemy_Laceling a_EnemyInstance)
	{
		if( a_EnemyInstance.CanDash() )
		{
			// Get Random Dash Start Position based on Dashable Platforms in the direction we're facing.
			KeyValuePair<bool, int> kStartTarget = GetDashTargetPosition(m_iClosestPlatform, a_EnemyInstance.MovementDirectionFacing);
			if (kStartTarget.Key)
			{
				// If a dashable start position was obtained find another Platform in that direction to end on 
				List<int> lEndTarget;
				if (a_EnemyInstance.MovementDirectionFacing == Enemy_Base.MovementDirection.LEFT)
				{
					lEndTarget = GetListOfPlatformsToLeftIndexes(kStartTarget.Value);
					lEndTarget.Reverse();
				}
				else
				{
					lEndTarget = GetListOfPlatformsToRightIndexes(kStartTarget.Value);
				}

				// If More than one tile away, go ahead and dash.
				if (lEndTarget.Count > 2)
				{
					int iSecondTargetPlatform = lEndTarget[Random.Range(2, lEndTarget.Count)];

					float fRandomOffsetA = (GetRandomPositionOffset() * LevelGrid.GetTileSize().x);
					float fRandomOffsetB = (GetRandomPositionOffset() * LevelGrid.GetTileSize().x);

					bool bPlusForA = (Random.Range(0, 2) == 0);
					bool bPlusForB = (Random.Range(0, 2) == 0);

					float fStartTargetX = (m_fPlatformXPositions[kStartTarget.Value]	+ (bPlusForA ? fRandomOffsetA : -fRandomOffsetA));
					float fEndTargetX	= (m_fPlatformXPositions[iSecondTargetPlatform] + (bPlusForB ? fRandomOffsetB : -fRandomOffsetB));

					// Send to Laceling
					a_EnemyInstance.SetupDash(fStartTargetX, fEndTargetX);
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Assign Breakable Target
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AssignBreakableTarget(Enemy_Romling a_EnemyInstance)
	{
		if(a_EnemyInstance.CanBreakPlatform())
		{
			List<int> lIndexes;
			if (a_EnemyInstance.MovementDirectionFacing == Enemy_Base.MovementDirection.LEFT)
				lIndexes = GetBreakablePlatformsToLeft(m_iClosestPlatform, 1);
			else
				lIndexes = GetBreakablePlatformsToRight(m_iClosestPlatform, 0);


			if (lIndexes.Count > 0)
			{
				int iPlatformID = lIndexes[Random.Range(0, lIndexes.Count)];

				//Send to Romling
				a_EnemyInstance.SetupPlatformDestroy(m_fPlatformXPositions[iPlatformID], m_ConnectingPlatformObjects[iPlatformID]);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Assign Enemy Direction
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AssignEnemyDirection(Enemy_Base a_EnemyInstance)
	{
		switch (m_PlatformGuides[m_iClosestPlatform].m_eDirectionType)
		{
			case PlatformInformation.MovementDirection.FACING:
			{
				a_EnemyInstance.SetFacingDirection( a_EnemyInstance.MovementDirectionFacing );
				break;
			}
			case PlatformInformation.MovementDirection.LEFT:
			{
				a_EnemyInstance.SetFacingDirection(Enemy_Base.MovementDirection.LEFT);
				break;
			}
			case PlatformInformation.MovementDirection.RIGHT:
			{
				a_EnemyInstance.SetFacingDirection(Enemy_Base.MovementDirection.RIGHT);
				break;
			}
			case PlatformInformation.MovementDirection.TOWARDS_PLAYER:
			{
				a_EnemyInstance.SetFacingDirection(Enemy_Base.MovementDirection.TOWARDS_PLAYER);
				break;
			}
			case PlatformInformation.MovementDirection.RANDOM:
			{
				a_EnemyInstance.SetFacingDirection( ((Random.Range(0, 2) == 0) ? Enemy_Base.MovementDirection.LEFT : Enemy_Base.MovementDirection.RIGHT) );
				break;
			}
			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Collision
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag.Contains("Enemy_"))
		{
			m_iClosestPlatform = GetClosestPlatform(collision.transform.position);

			GameObject goEnemy		= collision.gameObject;
			Enemy_Base pEnemyScript = goEnemy.GetComponent<Enemy_Base>();

			pEnemyScript.PlatformCollisionID = m_iPlatformCollisionBoxID;			// Assign Collision Box ID
			AssignEnemyDirection(pEnemyScript);										// Assign Direction

			switch (pEnemyScript.EnemyType)
			{
				case Enemy_Base.TypeOfEnemy.LACELING:
				{
					AssignDashTarget( goEnemy.GetComponent<Enemy_Laceling>() );
					break;
				}
				case Enemy_Base.TypeOfEnemy.ROMLING:
				{
					AssignBreakableTarget( goEnemy.GetComponent<Enemy_Romling>() );
					break;
				}
				default:
				{
					break;
				}
			}
		}
	}
}
