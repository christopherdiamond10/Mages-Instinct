using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGrid : MonoBehaviour 
{
	public static LevelGrid Instance { get{return Object.FindObjectOfType(typeof(LevelGrid)) as LevelGrid;} }

	public struct Coords
	{
		public Vector3 PointOne, PointTwo, PointThree, PointFour;
	}

	public Vector2	m_v2StartPos		= new Vector2(-0.03f, -0.02f);
	public Vector2	m_v2EndPos			= new Vector2(3.64f, 2.0f);
	public int		m_iTileCountHorz	= 17;
	public int		m_iTileCountVertz	= 17;

	
	[SerializeField] public static Vector2			sm_v2StartPos		= new Vector2(-0.03f, -0.02f);
	[SerializeField] public static Vector2			sm_v2EndPos			= new Vector2(3.64f, 2.0f);
	[SerializeField] public static int				sm_iTileCountHorz	= 17;
	[SerializeField] public static int				sm_iTileCountVertz	= 17;
	[SerializeField] private static Vector2			sm_v2TileSize		= CreateTileSize();
	[SerializeField] private static List<Coords>	sm_lCoordinates		= CreateGrid();
	[SerializeField] private static GameObject[,]	sm_aTileMap;

	public static int TileHorzBeginID		{ get { return ((TileHorzOffsetCount / 2) * -1);			} }
	public static int TileVertzBeginID		{ get { return 0;											} }
	public static int TileHorzOffsetCount	{ get { return 2;											} }
	public static int TileVertzOffsetCount	{ get { return 1;											} }
	public static int TileCountHorz			{ get { return sm_iTileCountHorz + TileHorzOffsetCount;		} } // Plus 2, for the Offscreen Tiles
	public static int TileCountVertz		{ get { return sm_iTileCountVertz + TileVertzOffsetCount;	} } // Plus 1, for the Offscreen Tile

	void Awake()
	{
		Reset();
		CreateTilemap();
	}


	void Start () 
	{

	}


	void Update() 
	{
		
	}

	public void Reset()
	{
		sm_v2StartPos		= m_v2StartPos;
		sm_v2EndPos			= m_v2EndPos;
		sm_iTileCountHorz	= m_iTileCountHorz;
		sm_iTileCountVertz	= m_iTileCountVertz;
		sm_v2TileSize		= CreateTileSize();
		sm_lCoordinates		= CreateGrid();
	}

	void OnDrawGizmos()
	{
		CheckData();
		foreach (Coords C in sm_lCoordinates)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(C.PointOne, C.PointTwo);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(C.PointTwo, C.PointThree);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(C.PointThree, C.PointFour);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(C.PointFour, C.PointOne);
		}
	}

	static private void CheckData()
	{
		if (sm_lCoordinates.Count == 0)
		{
			Instance.Reset();
		}
	}

	static private List<Coords> CreateGrid()
	{
		List<Coords> Grid = new List<Coords>();
		Vector2 Offset = GetTileSize();
		for (int x = -1; x < (sm_iTileCountHorz + 1); ++x)
		{
			for (int y = 0; y < (sm_iTileCountVertz + 1); ++y)
			{
				Coords C;
				C.PointOne		= new Vector3(sm_v2StartPos.x + (Offset.x * x),		sm_v2StartPos.y + (Offset.y * (y+1)));
				C.PointTwo		= new Vector3(sm_v2StartPos.x + (Offset.x * (x+1)),	sm_v2StartPos.y + (Offset.y * (y+1)));
				C.PointThree	= new Vector3(sm_v2StartPos.x + (Offset.x * (x+1)),	sm_v2StartPos.y + (Offset.y * y));
				C.PointFour		= new Vector3(sm_v2StartPos.x + (Offset.x * x),		sm_v2StartPos.y + (Offset.y * y));
				Grid.Add(C);
			}
		}
		return Grid;
	}

	static private void CreateTilemap()
	{
		sm_aTileMap = new GameObject[TileCountHorz, TileCountVertz];
		GameObject goLevelMap = GameObject.Find("Level Map");
		for (int x = 0; x < TileCountHorz; ++x)
		{
			Transform Row = goLevelMap.transform.GetChild(x);
			for (int y = 0; y < TileCountVertz; ++y)
			{
				sm_aTileMap[x, y] = Row.GetChild(y).gameObject;
			}
		}
	}

	static private Vector2 CreateTileSize()
	{
		return new Vector2(((sm_v2EndPos.x + sm_v2StartPos.x) / sm_iTileCountHorz), ((sm_v2EndPos.y - sm_v2StartPos.y) / sm_iTileCountVertz));
	}

	static public Vector2 GetTileSize()
	{
		return sm_v2TileSize;
	}
	
	static public List<Coords> GetGrid()
	{
		return sm_lCoordinates;
	}

	static public bool ValidTile(int X, int Y)
	{
		return (X >= 0 && X < TileCountHorz) && (Y >= 0 && Y < TileCountVertz);
	}

	static public GameObject GetTile(int X, int Y)
	{
		return sm_aTileMap[X, Y];
	}
}
