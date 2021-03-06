﻿using UnityEngine;
using System.Collections;

public class Fireball_AttackMagicPooling : ObjectPooling_Base<Fireball_AttackMagic>
{
	private static Fireball_AttackMagicPooling sm_rSelfInstance;

	// Use this for initialization
	protected override void Start()
	{
		sm_rSelfInstance = this;
		base.Start();
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
	}

	protected override void ProcessNewObjectAdditions(ObjectPool PoolObj)
	{
		base.ProcessNewObjectAdditions(PoolObj);
		PoolObj.rObj = PoolObj.rGameObj.GetComponent<Fireball_AttackMagic>();
	}
}

