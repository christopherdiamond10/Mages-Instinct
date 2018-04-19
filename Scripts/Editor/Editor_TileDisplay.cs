using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

[CanEditMultipleObjects]
[CustomEditor(typeof(TileDisplay))]
public class Editor_TileDisplay : Editor
{
	TileDisplay Target { get { return target as TileDisplay; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnInspectorGUI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Convert To Empty Object"))
		{
			Target.ChangeTileType(TileDisplay.eTileType.NOTHING);
		}
		if (GUILayout.Button("Convert To Sprite"))
		{
			Target.ChangeTileType(TileDisplay.eTileType.SPRITE);
		}
		if (GUILayout.Button("Convert To Platform"))
		{
			Target.ChangeTileType(TileDisplay.eTileType.PLATFORM);
		}
		if (GUILayout.Button("Convert To Short Platform"))
		{
			Target.ChangeTileType(TileDisplay.eTileType.SHORT_PLATFORM);
		}
		if (GUILayout.Button("Convert To AI Spawner"))
		{
			Target.ChangeTileType(TileDisplay.eTileType.SPAWNER);
		}
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		// Clone to Left/Right tile and select that tile for quick level editing :)
		if (GUILayout.Button("Clone To Left Tile"))
		{
			GameObject obj = Target.CloneToLeftTile();
			if (obj != null)
				Selection.activeGameObject = obj;
		}
		if (GUILayout.Button("Clone To Right Tile"))
		{
			GameObject obj = Target.CloneToRightTile();
			if (obj != null)
				Selection.activeGameObject = obj;
		}


		switch (Target.TileType)
		{
			case TileDisplay.eTileType.NOTHING:
			{
				break;
			}
			case TileDisplay.eTileType.SPRITE:
			{
				DrawSpriteOptions();

				if (Target.Child == null)
				{
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					if (GUILayout.Button("Add Background Sprite"))
					{
						Target.AddChild();
					}
				}
				break;
			}
			case TileDisplay.eTileType.PLATFORM:
			{
				DrawPlatformOptions();

				if (Target.Child == null)
				{
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					if (GUILayout.Button("Add Background Sprite"))
					{
						Target.AddChild();
					}
				}
				break;
			}
			case TileDisplay.eTileType.SHORT_PLATFORM:
			{
				DrawPlatformOptions();

				if (Target.Child == null)
				{
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					if (GUILayout.Button("Add Background Sprite"))
					{
						Target.AddChild();
					}
				}
				break;
			}
			case TileDisplay.eTileType.SPAWNER:
			{
				DrawSpawnerOptions();
				break;
			}
			default:
			{
				break;
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(Target);
			Target.ResetDisplay();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Sprite Tile Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawSpriteOptions()
	{
		DrawSpriteRendererOptions(Target.gameObject.GetComponent<SpriteRenderer>() );

		if (Target.Child != null)
		{
			DrawSpriteRendererOptions(Target.Child.gameObject.GetComponent<SpriteRenderer>(), "Background Sprite");
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Platform Tile Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawPlatformOptions()
	{
		DrawPlatformGuidanceOptions();
		DrawSpriteRendererOptions(Target.gameObject.GetComponent<SpriteRenderer>());

		if (Target.Child != null)
		{
			DrawSpriteRendererOptions(Target.Child.gameObject.GetComponent<SpriteRenderer>(), "Background Sprite");
		}

		DrawConnectedPlatforms();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Spawner Tile Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawSpawnerOptions()
	{
		DrawAISpawnerOptions();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Sprite Renderer Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawSpriteRendererOptions(SpriteRenderer SRend, string sLabel = "Sprite Renderer")
	{
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(sLabel, EditorStyles.boldLabel);

		
		SRend.sprite = EditorGUILayout.ObjectField("Sprite: ", SRend.sprite, typeof(Sprite), true) as Sprite;
		SRend.color = EditorGUILayout.ColorField("Colour: ", SRend.color);
		SRend.sharedMaterial = EditorGUILayout.ObjectField("Material: ", SRend.sharedMaterial, typeof(Material), true) as Material;

		// Sorting Layer
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		string[] aSortingLayerNames = (string[])sortingLayersProperty.GetValue(null, new object[0]);
		int iSortingLayerSelectedOption = 0;
		for (int i = 0; i < aSortingLayerNames.Length; ++i)
		{
			if (aSortingLayerNames[i] == SRend.sortingLayerName)
			{
				iSortingLayerSelectedOption = i;
				break;
			}
		}
		iSortingLayerSelectedOption = EditorGUILayout.Popup("Sorting Layer: ", iSortingLayerSelectedOption, aSortingLayerNames);
		SRend.sortingLayerName = aSortingLayerNames[iSortingLayerSelectedOption];

		// Sorting Order
		SRend.sortingOrder = EditorGUILayout.IntField("Order in Layer: ", SRend.sortingOrder);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Platform Information Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawPlatformGuidanceOptions()
	{
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Platform Guidance", EditorStyles.boldLabel);

		PlatformInformation PGuidance	= Target.gameObject.GetComponent<PlatformInformation>();
		PGuidance.m_eDirectionType		= (PlatformInformation.MovementDirection)EditorGUILayout.EnumPopup("Enemy Move Direction: ", PGuidance.m_eDirectionType);
		PGuidance.m_bBreakable			= EditorGUILayout.Toggle("Breakable Platform: ", PGuidance.m_bBreakable);
		PGuidance.m_bCanDash			= EditorGUILayout.Toggle("Can Dash: ", PGuidance.m_bCanDash);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Connected Platforms
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawConnectedPlatforms()
	{
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		serializedObject.Update();
		GUIContent Label = new GUIContent("Linked Objects");
		SerializedProperty LinkedObjects = serializedObject.FindProperty("m_agoLinkedObjects");
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(LinkedObjects, Label, true);
		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw AI Spawner Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawAISpawnerOptions()
	{
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("AI Spawner", EditorStyles.boldLabel);

		AI_Spawner pSpawner = Target.gameObject.GetComponent<AI_Spawner>();
		EditorGUILayout.LabelField("Spawn Cooldown Time");
		EditorGUI.indentLevel += 1;
		{
			pSpawner.m_fSpawnCooldownMin = EditorGUILayout.FloatField("Min: ", pSpawner.m_fSpawnCooldownMin);
			pSpawner.m_fSpawnCooldownMax = EditorGUILayout.FloatField("Max: ", pSpawner.m_fSpawnCooldownMax);
		}
		EditorGUI.indentLevel -= 1;

		pSpawner.m_iChanceToSpawnStoopling	= EditorGUILayout.IntField("Stoopling Spawn Chance: ", pSpawner.m_iChanceToSpawnStoopling);
		pSpawner.m_iChanceToSpawnLaceling	= EditorGUILayout.IntField("Laceling Spawn Chance: ", pSpawner.m_iChanceToSpawnLaceling);
		pSpawner.m_iChanceToSpawnRomling	= EditorGUILayout.IntField("Romling Spawn Chance: ", pSpawner.m_iChanceToSpawnRomling);

		pSpawner.m_tSpawnParent				= EditorGUILayout.ObjectField("Enemy Parent Transform: ", pSpawner.m_tSpawnParent, typeof(Transform), true) as Transform;
	}
}
