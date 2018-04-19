using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Grid_Location = System.Collections.Generic.KeyValuePair<int, int>;
using ConnectedLocation = System.Collections.Generic.KeyValuePair<bool, System.Collections.Generic.KeyValuePair<int, int>>;

public class PlatformCollisionBoxes : MonoBehaviour 
{
	private class PlatformSetup
	{
		public GameObject PlatformObject = null;
		public bool IsPlatform = false;
	};

	private class PlatformProperties
	{
		public int X { get { return Location.Key; } }
		public int Y { get { return Location.Value; } }
		public bool IsShort { get { return Short; } }

		Grid_Location Location;
		bool Short;

		public PlatformProperties(int x, int y, bool shortened)
		{
			Location = new Grid_Location(x, y);
			Short = shortened;
		}
	}

	private PlatformSetup[,] m_PlatformsOnGrid;
	private List<List<PlatformProperties>> m_lPlatformConnections;
	// Use this for initialization
	void Start () 
	{
		ResetPlatformColliders();
	}

	// Update is called once per frame
	void Update()
	{
	
	}

	public void ResetPlatformColliders()
	{
		m_lPlatformConnections = new List<List<PlatformProperties>>();
		DeletePlatformColliders();
		FindPlatforms();
		CreatePlatformBoxColliders();
	}

	void DeletePlatformColliders()
	{
		Transform tPlatformColliders = gameObject.transform.FindChild("Platform Colliders");
		if (tPlatformColliders != null)
		{
			DestroyImmediate(tPlatformColliders.gameObject);
		}
	}

	void FindPlatforms()
	{
		m_PlatformsOnGrid = new PlatformSetup[LevelGrid.TileCountHorz, LevelGrid.TileCountVertz];
		for (int y = 0; y < LevelGrid.TileCountVertz; ++y)
		{
			for (int x = 0; x < LevelGrid.TileCountHorz; ++x)
			{
				if (m_PlatformsOnGrid[x, y] != null)
					continue;

				TileDisplay pTileDisplay = transform.GetChild(x).GetChild(y).GetComponent<TileDisplay>();
				if (pTileDisplay.TileType == TileDisplay.eTileType.PLATFORM || pTileDisplay.TileType == TileDisplay.eTileType.SHORT_PLATFORM)
				{
					m_PlatformsOnGrid[x, y]			= new PlatformSetup();
					PlatformSetup pPlatformSetup	= m_PlatformsOnGrid[x, y];
					pPlatformSetup.PlatformObject	= transform.GetChild(x).GetChild(y).gameObject;
					pPlatformSetup.IsPlatform		= true;


					List<PlatformProperties> lConnectedPlatforms = new List<PlatformProperties>();
					lConnectedPlatforms.Add( new PlatformProperties(x, y, pTileDisplay.TileType == TileDisplay.eTileType.SHORT_PLATFORM) );
					while (x < (LevelGrid.TileCountHorz - 1))
					{
						x += 1;
						if (transform.GetChild(x).GetChild(y).GetComponent<TileDisplay>().IsPlatform)
						{
							m_PlatformsOnGrid[x, y]			= new PlatformSetup();
							pPlatformSetup					= m_PlatformsOnGrid[x, y];
							pPlatformSetup.PlatformObject	= transform.GetChild(x).GetChild(y).gameObject;
							pPlatformSetup.IsPlatform		= true;
							lConnectedPlatforms.Add( new PlatformProperties(x, y, pTileDisplay.TileType == TileDisplay.eTileType.SHORT_PLATFORM) );
						}
						else
						{
							break;
						}
					}
					m_lPlatformConnections.Add(lConnectedPlatforms);
				}
			}
		}
	}

	ConnectedLocation CheckForLeftConnectedPlatform(int x, int y)
	{
		if (x > 0)
		{
			if (transform.GetChild(x - 1).GetChild(y).GetComponent<TileDisplay>().TileType == TileDisplay.eTileType.PLATFORM)
			{
				return new ConnectedLocation(true, new Grid_Location((x - 1), y));
			}
		}
		return new ConnectedLocation(false, new Grid_Location(0, 0));
	}

	ConnectedLocation CheckForRightConnectedPlatform(int x, int y)
	{
		if (x < (LevelGrid.TileCountHorz - 1))
		{
			if (transform.GetChild(x + 1).GetChild(y).GetComponent<TileDisplay>().TileType == TileDisplay.eTileType.PLATFORM)
			{
				return new ConnectedLocation(true, new Grid_Location((x + 1), y));
			}
		}
		return new ConnectedLocation(false, new Grid_Location(0, 0));
	}

	void CreatePlatformBoxColliders()
	{
		GameObject goBoxCollidersParent = new GameObject("Platform Colliders");
		goBoxCollidersParent.transform.parent = gameObject.transform;

		int iCurrentPlatformCollisionID = 0;
		float fShortPlatformsSizeScaler = 0.25f;
		float fShortPlatformsPositionOffset = 0.25f;
		foreach (List<PlatformProperties> lConnections in m_lPlatformConnections)
		{
			// Increment Collision ID
			iCurrentPlatformCollisionID += 1;

			// Create GameObject
			GameObject goPlatformCollider = new GameObject("Platform Collider");
			goPlatformCollider.transform.parent = goBoxCollidersParent.transform;

			// Add Box Collider
			BoxCollider2D BCollider = goPlatformCollider.AddComponent<BoxCollider2D>();
			float xSize = LevelGrid.GetTileSize().x * lConnections.Count;
			float ySize = (lConnections[0].IsShort ? LevelGrid.GetTileSize().y * fShortPlatformsSizeScaler : LevelGrid.GetTileSize().y);
			BCollider.size = new Vector2(xSize, ySize);

			// Set Position
			PlatformProperties glFirstElement = lConnections[0];
			PlatformProperties glLastElement = lConnections[lConnections.Count - 1];
			float xStartPosition = (m_PlatformsOnGrid[glFirstElement.X, glFirstElement.Y].PlatformObject.transform.position.x - (LevelGrid.GetTileSize().x * 0.5f));
			float xEndPosition	 = (m_PlatformsOnGrid[glLastElement.X, glLastElement.Y].PlatformObject.transform.position.x + (LevelGrid.GetTileSize().x * 0.5f));
			float xPosition		 = (xStartPosition + ((xEndPosition - xStartPosition) * 0.5f));
			float yPosition		 = (m_PlatformsOnGrid[glFirstElement.X, glFirstElement.Y].PlatformObject.transform.position.y);
			if (lConnections[0].IsShort)
			{
				yPosition -= (LevelGrid.GetTileSize().y * fShortPlatformsPositionOffset);
			}
			goPlatformCollider.transform.position = new Vector3(xPosition, yPosition);

			// Add Individual Platform Informations to the GameObject
			PlatformInformationBridge PInfoBridge = goPlatformCollider.AddComponent<PlatformInformationBridge>();
			GameObject[] agoPlatforms = new GameObject[lConnections.Count];
			for(int i = 0; i < lConnections.Count; ++i)
			{
				PlatformProperties GL = lConnections[i];
				agoPlatforms[i] = m_PlatformsOnGrid[GL.X, GL.Y].PlatformObject;
			}
			PInfoBridge.AddPlatformObjects(agoPlatforms, iCurrentPlatformCollisionID);
		}
	}
}
