using UnityEngine;
using System.Collections;

public class ImportantObjectsManager : MonoBehaviour 
{
	public GameObject m_goPlayerObject;
	public GameObject m_goLevelMapObject;

	private static ImportantObjectsManager sm_Instance = null;


	public static GameObject Player		{ get { return sm_Instance.m_goPlayerObject;	} }
	public static GameObject LevelMap	{ get { return sm_Instance.m_goLevelMapObject;	} }

	
	void Awake()
	{
		sm_Instance = this;
	}
}
