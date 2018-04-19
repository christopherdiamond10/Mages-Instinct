using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Romling : Enemy_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool		  m_bDestroyPlatformEvent		= false;
	public int		  m_iPlatformDestroyChance		= 10;					// Chance to Destroy a Platform from the Get-Go? Also used as the Main Variable for this Event
	public int		  m_iPlatformDestroyChanceGain	= 5;					// Every time the Destroy Chance is Queried but Fails, How Much Should the Chance Be Increased?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected PlatformBreakStructure m_PlatformBreakStructure = new PlatformBreakStructure();
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected enum PlatformDestroyState
	{
		STALLING,			// Waiting above Platform Briefly
		DESTROYING,			// Attacking Platform
	};

	protected class PlatformBreakStructure
	{
		public static float				sfBreakWaitTime			= 2.5f;								// How long will the Romling lean down and point its arm in the air before hittng the Platform? Basically how long will the Romling Signal that it's going to break this platform?
		
		public bool						bHasBrokenPlatform		= false;							// This Romling has not previously broken a platform?
		public bool						bHasBreakableTarget		= false;							// This Romling has a Platform Targeted for it to Break?
		public bool						bHasInitiatedBreak		= false;							// The Romling has Entered "Break Platform" Mode?
		public PlatformDestroyState		ePlatformDestroyState	= PlatformDestroyState.STALLING;	// The Current Execution State
		public float					fPlatformPositionX		= 0.0f;								// The Position for the Romling to begin the Attack Animation
		public GameObject				goTargettedPlatform		= null;								// The Platform that this Romling will be Breaking.
		public TimeTracker				ttStallTimer			= new TimeTracker(sfBreakWaitTime);	// The Stall Timer used when Romling points its arm in the air.
	};
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();
		m_eEnemyType = TypeOfEnemy.ROMLING;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		if(!IsFrozen)
		{
			// Update Platform Destroy
			if (m_PlatformBreakStructure.bHasInitiatedBreak)
			{
				if (!IsGrounded())
				{
					TurnOffPlatformDestroy();
				}
				else
				{
					UpdatePlatformDestruction();
				}
			}

			// Update Movement
			else
			{
				base.Update();
				GetAnimator().SetBool( GetParamHashIDs().FallingParamID, (IsFalling() && !m_bDamageJumpActive) );

				if (m_PlatformBreakStructure.bHasBreakableTarget)
				{
					if (!IsGrounded())
					{
						m_PlatformBreakStructure.bHasBreakableTarget = false;
					}
					else
					{
						UpdatePlatformDestroyBeginCheck();
					}
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Platform Destruction
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdatePlatformDestruction()
	{
		switch (m_PlatformBreakStructure.ePlatformDestroyState)
		{
			case PlatformDestroyState.STALLING:
			{
				m_PlatformBreakStructure.ttStallTimer.Update();
				if (m_PlatformBreakStructure.ttStallTimer.TimeUp())
				{
					m_PlatformBreakStructure.ePlatformDestroyState = PlatformDestroyState.DESTROYING;
					GetAnimator().SetBool( GetParamHashIDs().DestroyPlatformParamID, true );
					GetAnimator().SetBool( GetParamHashIDs().DestroyPlatformStallingParamID, false );
				}
				break;
			}

			case PlatformDestroyState.DESTROYING:
			{
				if (m_bDestroyPlatformEvent)
				{
					TurnOffPlatformDestroy();

					if (m_PlatformBreakStructure.goTargettedPlatform != null)
					{
						m_PlatformBreakStructure.goTargettedPlatform.GetComponent<TileDisplay>().BreakPlatform();
						
						GameObject goLevelMap = GameObject.Find("Level Map");
						if (goLevelMap != null)
						{
							AI_Manager.ResetEnemySpecialties(m_iPlatformCollisionID);
							goLevelMap.GetComponent<PlatformCollisionBoxes>().ResetPlatformColliders();
						}
					}
				}
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Dash Begin Check
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdatePlatformDestroyBeginCheck()
	{
		// If Romling is around Start Position
		if (Mathf.Abs(Position.x - m_PlatformBreakStructure.fPlatformPositionX) < Mathf.Abs(Velocity.x * Time.deltaTime * 2.3f))
		{
			Position.x										= m_PlatformBreakStructure.fPlatformPositionX;
			m_PlatformBreakStructure.bHasInitiatedBreak		= true;
			m_PlatformBreakStructure.ePlatformDestroyState	= PlatformDestroyState.STALLING;

			GetAnimator().SetBool( GetParamHashIDs().DestroyPlatformStallingParamID, true );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Turn Off Platform Destroy
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void TurnOffPlatformDestroy()
	{
		m_PlatformBreakStructure.bHasBrokenPlatform = true;
		m_PlatformBreakStructure.bHasInitiatedBreak = false;
		m_PlatformBreakStructure.bHasBreakableTarget = false;

		GetAnimator().SetBool(GetParamHashIDs().DestroyPlatformParamID, false);
		GetAnimator().SetBool(GetParamHashIDs().DestroyPlatformStallingParamID, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Reset Specialty
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void ResetSpecialty()
	{
		base.ResetSpecialty();

		// Romling has no need to ReCalculate its special, because it won't calculate to any platform that has been targeted already 

		//m_PlatformBreakStructure.bHasBreakableTarget = false;
		//m_PlatformBreakStructure.ttStallTimer.Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Can Break Platform
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CanBreakPlatform()
	{
		// If Already Broken a Platform, Cannot Break Another One || Already has a Target
		if (m_PlatformBreakStructure.bHasBrokenPlatform || m_PlatformBreakStructure.bHasBreakableTarget) 
		{ 
			return false;
		}


		// If Randomly Allowed To Break Platform
		if (Random.Range(0, 100) < m_iPlatformDestroyChance)
		{
			return true;
		}

		// Otherwise Increase the Chance for Next Time
		else
		{
			m_iPlatformDestroyChance += m_iPlatformDestroyChanceGain;
			return false;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Platform Destroy
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetupPlatformDestroy(float fTargetX, GameObject TargetPlatform)
	{
		if( !PlatformTargetedByAnotherRomling(TargetPlatform) )
		{
			m_PlatformBreakStructure.fPlatformPositionX = fTargetX;
			m_PlatformBreakStructure.goTargettedPlatform = TargetPlatform;
			m_PlatformBreakStructure.bHasBreakableTarget = true;
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
	//	* Redefined Method: On Death
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnDeath()
	{
		base.OnDeath();
		if (m_PlatformBreakStructure.bHasInitiatedBreak)
		{
			TurnOffPlatformDestroy();
		}
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
	//	* New Method: Is a Platform Being Targeted By Another Romling?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static bool PlatformTargetedByAnotherRomling(GameObject Target)
	{
		List<Enemy_Romling> lRomlings = AI_Manager.GetRomlingEnemies();
		foreach (Enemy_Romling pRomling in lRomlings)
		{
			if (pRomling.m_PlatformBreakStructure.goTargettedPlatform == Target)
				return true;
		}
		return false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static EnemyRomlingAnimationHashIDs.AnimationStateHashIDs GetStateHashIDs()
	{
		return EnemyRomlingAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Param Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static EnemyRomlingAnimationHashIDs.AnimationParamHashIDs GetParamHashIDs()
	{
		return EnemyRomlingAnimationHashIDs.GetParamHashIDs();
	}
}
