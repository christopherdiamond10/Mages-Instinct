using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooling_Base<T> : MonoBehaviour
{
	public GameObject m_PoolObj;
	public int m_iObjCount = 1;

	protected List<ObjectPool> m_lPooledObjects = new List<ObjectPool>();

	protected class ObjectPool
	{
		public T rObj;
		public GameObject rGameObj;
		public bool bActive = false;
	}



	// Use this for initialization
	protected virtual void Start()
	{
		AddNewObjects(m_iObjCount);
	}

	// Update is called once per frame
	protected virtual void Update()
	{

	}

	public virtual void Reset()
	{
		foreach (ObjectPool Obj in m_lPooledObjects)
		{
			Obj.bActive = false;
			Obj.rGameObj.SetActive(false);
		}
	}

	public virtual T GetFreeObject()
	{
		// Go Through Object Pool and Get a free Object
		foreach (ObjectPool Obj in m_lPooledObjects)
		{
			if (!Obj.rGameObj.activeInHierarchy)
			{
				return Obj.rObj;
			}
		}

		// If Unable to get a free Object, then we'll need to create a new one
		ObjectPool rNewObj = CreateNewPoolObject();
		rNewObj.bActive = true;
		m_lPooledObjects.Add(rNewObj);
		return rNewObj.rObj;
	}

	public virtual T GetFreeObjectInList()
	{
		// Go Through Object Pool and Get a free Object
		foreach (ObjectPool Obj in m_lPooledObjects)
		{
			if (!Obj.bActive)
			{
				Obj.bActive = true;
				return Obj.rObj;
			}
		}

		// If Unable to get a free Object, then we'll need to create a new one
		ObjectPool rNewObj = CreateNewPoolObject();
		rNewObj.bActive = true;
		m_lPooledObjects.Add(rNewObj);
		return rNewObj.rObj;
	}

	protected virtual void AddNewObjects(int Count)
	{
		for(int i = 0; i < Count; ++i)
		{
			m_lPooledObjects.Add(CreateNewPoolObject());			// Add New Pool Object to List
		}
	}

	protected virtual ObjectPool CreateNewPoolObject()
	{
		if (m_PoolObj != null)
		{
			ObjectPool rNewObj = new ObjectPool();						// Create New Pool Object Instance
			rNewObj.rGameObj = Instantiate(m_PoolObj) as GameObject;	// Instantiate a New GameObject
			rNewObj.rGameObj.transform.parent = this.transform;			// Set New Object's Parent
			rNewObj.rGameObj.SetActive(false);							// Set New Object As Inactive
			ProcessNewObjectAdditions(rNewObj);							// Apply Specific Additions (overriden function)
			return rNewObj;
		}
		return null;
	}

	protected virtual void ProcessNewObjectAdditions(ObjectPool PoolObj)
	{
	}
}
