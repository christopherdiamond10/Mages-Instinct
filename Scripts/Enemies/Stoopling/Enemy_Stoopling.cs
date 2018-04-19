using UnityEngine;
using System.Collections;

public class Enemy_Stoopling : Enemy_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		if (!IsFrozen)
		{
			base.Update();
			GetAnimator().SetBool( GetParamHashIDs().FallingParamID, (IsFalling() && !m_bDamageJumpActive) );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Set Face Direction To Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void SetFaceLeft()
	{
		base.SetFaceLeft();
		GetAnimator().SetBool( GetParamHashIDs().WalkingRightParamID, false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Set Face Direction to Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void SetFaceRight()
	{
		base.SetFaceRight();
		GetAnimator().SetBool( GetParamHashIDs().WalkingRightParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Run Damage Jump Indication
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void RunDamageJumpIndication(float fPointOfImpact)
	{
		base.RunDamageJumpIndication(fPointOfImpact);
		GetAnimator().SetBool( GetParamHashIDs().DamageJumpParamID, true );
		GetAnimator().SetBool( GetParamHashIDs().DamageJumpRightParamID, (fPointOfImpact > Position.x) );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Stop Damage Jump Indication
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void StopDamageJumpIndication()
	{
		base.StopDamageJumpIndication();
		GetAnimator().SetBool( GetParamHashIDs().DamageJumpParamID, false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Play Death Burnt Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void PlayDeathBurntAnimation()
	{
		base.PlayDeathBurntAnimation();
		GetAnimator().SetBool( GetParamHashIDs().DeathBurntParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Play Death Electrocuted Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void PlayDeathElectrocutedAnimation()
	{
		base.PlayDeathElectrocutedAnimation();
		GetAnimator().SetBool( GetParamHashIDs().DeathElectrocutedParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Play Death Crushed Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void PlayDeathCrushedAnimation()
	{
		base.PlayDeathCrushedAnimation();
		GetAnimator().SetBool( GetParamHashIDs().DeathCrushedParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static EnemyStooplingAnimationHashIDs.AnimationStateHashIDs GetStateHashIDs()
	{
		return EnemyStooplingAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Param Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static EnemyStooplingAnimationHashIDs.AnimationParamHashIDs GetParamHashIDs()
	{
		return EnemyStooplingAnimationHashIDs.GetParamHashIDs();
	}
}
