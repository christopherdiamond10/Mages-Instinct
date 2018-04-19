using UnityEngine;
using System.Collections;

public class Enemy_Laceling : Enemy_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject m_goDashTrailLeftObject;
	public GameObject m_goDashTrailRightObject;
	public int		  m_iDashChance		= 10;					// Chance to Dash from the Get-Go? Also used as the Main Variable for this Event
	public int		  m_iDashChanceGain = 5;					// Every time the Dash Chance is Queried but Fails, How Much Should the Chance Be Increased?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected DashStructure m_DashStructure = new DashStructure();
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected class DashStructure
	{
		public static int	 siTotalDashTrailObjects	= 10;														// How Many Trail Objects will be shown when Dashing?
		public static float  sfTotalDashSequenceTime	= 0.5f;														// How Long Will it take for the Laceling to Dash from Point A to Point B?
		public static float	 sfDashTrailTimePoints		= (sfTotalDashSequenceTime / siTotalDashTrailObjects);		// Just a quick calculation showing which point in the timer a trail object should be spawned.
		
		public bool			bHasDashed					= false;													// Laceling has Dashed Previously? Stops it from dashing again
		public bool			bHasDashTarget				= false;													// Laceling has a Dash Target?
		public bool			bHasInitiatedDash			= false;													// Laceling has Initialised Dash?
		public int			iCurrentTrailObjectCount	= 0;														// Current Amount of Rendered Trails
		public float		fStartDashPositionX			= 0.0f;														// The Start Position of the Dash
		public float		fEndDashPositionX			= 0.0f;														// The End Position of the Dash
		public float		fStartEndDifference			= 0.0f;														// The Difference Between the Start and End Positions
		public float		fCurrentDashTime			= 0.0f;														// The Current Dash Time, this ties in with the Dash Trail Time Points
		public TimeTracker	ttDashSequenceTimer			= new TimeTracker(sfTotalDashSequenceTime);					// The Timer for the Dash Itself
	};
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();
		m_eEnemyType = TypeOfEnemy.LACELING;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		if(!IsFrozen)
		{
			if (m_DashStructure.bHasInitiatedDash)
			{
				UpdateDash();
			}
			else
			{
				base.Update();
				GetAnimator().SetBool( GetParamHashIDs().FallingParamID, (IsFalling() && !m_bDamageJumpActive) );

				if (m_DashStructure.bHasDashTarget)
				{
					if (!IsGrounded())
					{
						m_DashStructure.bHasDashTarget = false;
					}
					else
					{
						UpdateDashBeginCheck();
					}
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Dash
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateDash()
	{
		// Check Dash Time, If TimeUp, Complete Dash
		m_DashStructure.ttDashSequenceTimer.Update();
		if (m_DashStructure.ttDashSequenceTimer.TimeUp())
		{
			TurnOffDash();
			Position.x = m_DashStructure.fEndDashPositionX;
		}

		// Otherwise Update Dash Timer, and the Position the Enemy Should be at during this period. Also if it's time to do so, Instantiate a Dash Trail Object
		else
		{
			Position.x = (m_DashStructure.fStartDashPositionX + (m_DashStructure.fStartEndDifference * m_DashStructure.ttDashSequenceTimer.GetCompletionPercentage()));

			if((m_DashStructure.ttDashSequenceTimer.CurrentTime - m_DashStructure.fCurrentDashTime) > DashStructure.sfDashTrailTimePoints)
			{
				m_DashStructure.fCurrentDashTime += DashStructure.sfDashTrailTimePoints;
				m_DashStructure.iCurrentTrailObjectCount += 1;

				Vector3 vDashTrailPosition	= transform.position;
				Color	SpriteColour		= GetComponent<Renderer>().material.color;
				SpriteColour.a				= ((float)m_DashStructure.iCurrentTrailObjectCount / DashStructure.siTotalDashTrailObjects);

				GameObject goDashTrailPrefab	= ((MovementDirectionFacing == MovementDirection.LEFT) ? m_goDashTrailLeftObject : m_goDashTrailRightObject);
				GameObject goDashTrailObj		= (Instantiate(goDashTrailPrefab) as GameObject);
				goDashTrailObj.transform.parent = this.transform.parent;
				goDashTrailObj.GetComponent<LacelingTrailObject>().Setup(vDashTrailPosition, SpriteColour, DashStructure.sfTotalDashSequenceTime);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Dash Begin Check
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateDashBeginCheck()
	{
		// If for whatever reason, this Laceling has begun falling (perhaps a Romling Destroyed a Platform in front of it). Cancel the Dash
		if (IsFalling())
		{
			m_DashStructure.bHasDashTarget = false;
			return;
		}

		// Otherwise, If Laceling is around Start Position
		if (Mathf.Abs(Position.x - m_DashStructure.fStartDashPositionX) < Mathf.Abs(Velocity.x * Time.deltaTime * 2.3f))
		{
			m_DashStructure.bHasInitiatedDash = true;
			GetAnimator().SetBool( GetParamHashIDs().DashingParamID, true );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Reset Specialty
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void ResetSpecialty()
	{
		base.ResetSpecialty();

		m_DashStructure.bHasDashTarget = false;
		m_DashStructure.ttDashSequenceTimer.Reset();
		GetAnimator().SetBool( GetParamHashIDs().DashingParamID, false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Turn Off Dash
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void TurnOffDash()
	{
		m_DashStructure.bHasDashed = true;
		m_DashStructure.bHasDashTarget = false;
		m_DashStructure.bHasInitiatedDash = false;
		GetAnimator().SetBool(GetParamHashIDs().DashingParamID, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Can Dash
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CanDash()
	{
		// If Already Dashed, Cannot Dash Again || Or has already got a Dash Target
		if (m_DashStructure.bHasDashed || m_DashStructure.bHasDashTarget) 
		{ 
			return false;
		}


		// If Randomly Allowed To Dash
		if (Random.Range(0, 100) < m_iDashChance)
		{
			return true;
		}

		// Otherwise Increase the Dash Chance for Next Time
		else
		{
			m_iDashChance += m_iDashChanceGain;
			return false;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Dash
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetupDash(float StartX, float TargetX)
	{
		m_DashStructure.fStartDashPositionX = StartX;
		m_DashStructure.fEndDashPositionX = TargetX;
		m_DashStructure.fStartEndDifference = (TargetX - StartX);
		m_DashStructure.bHasDashTarget = true;
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

		if (m_DashStructure.bHasInitiatedDash)
		{
			TurnOffDash();
		}
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
	public static EnemyLacelingAnimationHashIDs.AnimationStateHashIDs GetStateHashIDs()
	{
		return EnemyLacelingAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Param Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static EnemyLacelingAnimationHashIDs.AnimationParamHashIDs GetParamHashIDs()
	{
		return EnemyLacelingAnimationHashIDs.GetParamHashIDs();
	}
}
