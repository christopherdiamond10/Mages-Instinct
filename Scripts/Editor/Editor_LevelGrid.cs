using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelGrid))]
public class Editor_LevelGrid : Editor
{
	private LevelGrid Target { get { return target as LevelGrid; } } 
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnInspectorGUI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void OnInspectorGUI()
	{
		DrawResetLevelOption();
		DrawLevelGridOptions();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(Target);
			Target.Reset();
		}
	}

	private void DrawResetLevelOption()
	{
		if (GUILayout.Button("Reset Level"))
		{
			RunResetLevelCommand();
		}
	}

	private void DrawLevelGridOptions()
	{
		Target.m_v2StartPos			= EditorGUILayout.Vector2Field("Start Position: ", Target.m_v2StartPos);
		Target.m_v2EndPos			= EditorGUILayout.Vector2Field("End Position: ", Target.m_v2EndPos);
		Target.m_iTileCountHorz		= EditorGUILayout.IntField("Horizontal Tiles: ", Target.m_iTileCountHorz);
 		Target.m_iTileCountVertz	= EditorGUILayout.IntField("Vertical Tiles: ", Target.m_iTileCountVertz);
	}

	private void RunResetLevelCommand()
	{
		GameObject LevelMap = GameObject.Find("Level Map");
		if (LevelMap != null) { DestroyImmediate(LevelMap); }

		// Create Level Map
		LevelMap = new GameObject("Level Map");
		LevelMap.AddComponent<PlatformCollisionBoxes>();
		LevelMap.transform.position = new Vector3(-10.0f, 0.0f);

		// Create TileMap
		for (int x = LevelGrid.TileHorzBeginID; x < (LevelGrid.TileCountHorz); ++x)
		{
			GameObject NewRow = new GameObject("Row " + (x+1).ToString());
			NewRow.transform.parent = LevelMap.transform;
			NewRow.transform.position = new Vector3(-10.0f, 0.0f);

			for (int y = LevelGrid.TileVertzBeginID; y < (LevelGrid.TileCountVertz); ++y)
			{
				GameObject NewCol = new GameObject("Col " + (y+1).ToString());
				NewCol.tag = "Stage_Tile";
				NewCol.transform.parent = NewRow.transform;
				float XPos = (Target.m_v2StartPos.x + (x * LevelGrid.GetTileSize().x)) + (LevelGrid.GetTileSize().x * 0.5f);
				float YPos = (Target.m_v2StartPos.y + (y * LevelGrid.GetTileSize().y)) + (LevelGrid.GetTileSize().y * 0.5f);
				NewCol.transform.position = new Vector3(XPos, YPos);
				TileDisplay pTileDisplay = NewCol.AddComponent<TileDisplay>();
#if UNITY_EDITOR
				// Assign Identities 
				pTileDisplay.COL = x + 1;
				pTileDisplay.ROW = y;
#endif

				if( (x < 0) || (y >= Target.m_iTileCountVertz) || (x >= Target.m_iTileCountHorz) )
				{
					pTileDisplay.SetAsOffscreenTile();
				}
			}
		}

		// Add LevelMap to Object Manager
		GameObject ObjectManager = GameObject.Find("Level Manager");
		if (ObjectManager != null)
		{
			ObjectManager.GetComponent<ImportantObjectsManager>().m_goLevelMapObject = LevelMap;
		}
	}
}