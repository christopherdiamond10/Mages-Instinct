using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileDisplay : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TileDisplay[] m_agoLinkedObjects;

#if UNITY_EDITOR
	// Used to Identify this Tile in the Editor. 
	public int COL, ROW;
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[SerializeField] private eTileType	m_eTileType			= eTileType.NOTHING;
	[SerializeField] private bool		m_bIsOffscreenTile	= false;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public eTileType TileType { get { return m_eTileType; } }
	public GameObject Child	  { get { return (transform.childCount > 0 ? transform.GetChild(0).gameObject : null); } }
	public bool IsPlatform { get { return m_eTileType == eTileType.PLATFORM || m_eTileType == eTileType.SHORT_PLATFORM; } }


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum eTileType
	{
		NOTHING,
		SPRITE,
		PLATFORM,
		SHORT_PLATFORM,
		SPAWNER,
	};



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
	//	* New Method: Reset Current Tile Display
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ResetDisplay()
	{
		if (gameObject.GetComponent<SpriteResizer>() != null)
		{
			gameObject.GetComponent<SpriteResizer>().ResizeSprite();
		}

		if (Child != null)
		{
			if (Child.GetComponent<SpriteResizer>() != null)
			{
				Child.GetComponent<SpriteResizer>().ResizeSprite();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change This Tile Type
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ChangeTileType(eTileType eType)
	{
		if (eType != m_eTileType)
		{
			RemoveComponents(m_eTileType);
			AddComponents(eType);
			m_eTileType = eType;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set This Tile as Offscreen Tile
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetAsOffscreenTile()
	{
		m_bIsOffscreenTile = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Break This Platform
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BreakPlatform()
	{
		m_eTileType = eTileType.NOTHING;
		RemoveSpriteRenderer();
		RemoveSpriteResizer();
		DestroyPlatformInformation();

		if (m_agoLinkedObjects != null && m_agoLinkedObjects.Length > 0)
		{
			for (int i = 0; i < m_agoLinkedObjects.Length; ++i)
			{
				if (m_agoLinkedObjects[i] != null)
				{
					m_agoLinkedObjects[i].BreakPlatform();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Components
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AddComponents(eTileType eType)
	{
		switch (eType)
		{
			case eTileType.SPRITE:
			{
				AddSpriteRenderer(eTileType.SPRITE);
				AddSpriteResizer();
				break;
			}
			case eTileType.PLATFORM:
			{
				AddSpriteRenderer(eTileType.PLATFORM);
				AddSpriteResizer();
				AddPlatformInformation();
				break;
			}
			case eTileType.SHORT_PLATFORM:
			{
				AddSpriteRenderer(eTileType.PLATFORM);
				AddSpriteResizer();
				AddPlatformInformation();
				break;
			}
			case eTileType.SPAWNER:
			{
				AddAISpawnerComponent();
				break;
			}
			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove Components
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void RemoveComponents(eTileType eType)
	{
		switch (eType)
		{
		case eTileType.SPRITE:
			{
				RemoveSpriteRenderer();
				RemoveSpriteResizer();
				RemoveChild();
				break;
			}
		case eTileType.PLATFORM:
			{
				RemoveSpriteRenderer();
				RemoveSpriteResizer();
				RemoveChild();
				DestroyPlatformInformation();
				break;
			}
		case eTileType.SHORT_PLATFORM:
			{
				RemoveSpriteRenderer();
				RemoveSpriteResizer();
				RemoveChild();
				DestroyPlatformInformation();
				break;
			}
		case eTileType.SPAWNER:
			{
				DestroyAISpawner();
				break;
			}
		default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Sprite Renderer Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AddSpriteRenderer(eTileType eType)
	{
		SpriteRenderer C	= gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		C.hideFlags			= HideFlags.HideInInspector;
		if (eType == eTileType.SPRITE)
		{
			C.sortingLayerID = 2;
			C.sortingOrder = 1;
		}
		else
		{
			C.sortingLayerID = 3;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Sprite Resizer Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AddSpriteResizer()
	{
		Component C = gameObject.AddComponent(typeof(SpriteResizer));
		C.hideFlags = HideFlags.HideInInspector;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add AI_Spawner Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void AddAISpawnerComponent()
	{
		AI_Spawner C = gameObject.AddComponent(typeof(AI_Spawner)) as AI_Spawner;
		C.hideFlags = HideFlags.HideInInspector;
		
		GameObject EnemyParent = GameObject.Find("Enemies");
		if (EnemyParent != null)
			C.m_tSpawnParent = EnemyParent.transform;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Platform Information Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AddPlatformInformation()
	{
		Component C = gameObject.AddComponent(typeof(PlatformInformation));
		C.hideFlags = HideFlags.HideInInspector;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Child
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void AddChild()
	{
		if (Child == null)
		{
			GameObject NewChild = new GameObject("Background Sprite", typeof(SpriteRenderer), typeof(SpriteResizer));
			NewChild.GetComponent<SpriteRenderer>().sortingLayerID = 2;
			NewChild.transform.parent = this.transform;
			NewChild.transform.localPosition = Vector3.zero;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove Sprite Renderer Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void RemoveSpriteRenderer()
	{
		if(gameObject.GetComponent<SpriteRenderer>() != null)
			DestroyImmediate(gameObject.GetComponent<SpriteRenderer>());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove Sprite Resizer Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void RemoveSpriteResizer()
	{
		if(gameObject.GetComponent<SpriteResizer>() != null)
			DestroyImmediate(gameObject.GetComponent<SpriteResizer>());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Destroy AI_Spawner
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DestroyAISpawner()
	{
		if(gameObject.GetComponent<AI_Spawner>() != null)
			DestroyImmediate(gameObject.GetComponent<AI_Spawner>());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove Platform Information Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DestroyPlatformInformation()
	{
		if(gameObject.GetComponent<PlatformInformation>() != null)
			DestroyImmediate(gameObject.GetComponent<PlatformInformation>());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove Child
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void RemoveChild()
	{
		if (Child != null)
			DestroyImmediate(Child);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Clone Tile
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
	private void CloneTo(int X, int Y)
	{
		if(LevelGrid.ValidTile(X, Y))
		{
			TileDisplay pTile = GameObject.Find("Level Map").transform.GetChild(X).GetChild(Y).GetComponent<TileDisplay>();
			pTile.ChangeTileType(m_eTileType);

			if(gameObject.GetComponent<SpriteRenderer>() != null)
			{
				pTile.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
			}
			if (gameObject.GetComponent<PlatformInformation>() != null)
			{
				pTile.GetComponent<PlatformInformation>().m_eDirectionType = gameObject.GetComponent<PlatformInformation>().m_eDirectionType;
				pTile.GetComponent<PlatformInformation>().m_bBreakable = gameObject.GetComponent<PlatformInformation>().m_bBreakable;
				pTile.GetComponent<PlatformInformation>().m_bCanDash = gameObject.GetComponent<PlatformInformation>().m_bCanDash;
			}
			if(Child != null)
			{
				pTile.AddChild();
				pTile.Child.GetComponent<SpriteRenderer>().sprite = Child.GetComponent<SpriteRenderer>().sprite;
			}
		}
	}

	public void CloneToLeftTile()
	{
		CloneTo(COL - 1, ROW);
	}

	public void CloneToRightTile()
	{
		CloneTo(COL + 1, ROW);
	}
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: OnDrawGizmos
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmos()
	{
		if( (gameObject.GetComponent<SpriteRenderer>() == null) || (gameObject.GetComponent<SpriteRenderer>().sprite == null) )
		{
			if(m_eTileType == eTileType.SPAWNER)
				Gizmos.color = new Color(0.592f, 0.607f, 0.156f, 0.5f);
			else if(m_bIsOffscreenTile)
				Gizmos.color = new Color(0.156f, 0.592f, 0.607f, 0.5f);
			else
				Gizmos.color = new Color(0.607f, 0.156f, 0.592f, 0.5f);

			Gizmos.DrawCube(transform.position, LevelGrid.GetTileSize());
		}
	}
}
