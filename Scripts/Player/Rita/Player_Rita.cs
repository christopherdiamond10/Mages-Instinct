using UnityEngine;
using System.Collections;

public class Player_Rita : Player_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*= CONSTANTS
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private const int	TOTAL_FIREBALLS_PER_ATTACK = 3;
	private const float WAIT_TIME_BETWEEN_FIREBALLS = 0.2f;


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Rita_ArmsManager m_rRita_ArmsManager;
	public Fireball_AttackMagicPooling m_rAttackMagicPool;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float m_fMaxDistance = 0.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Rita_ArmsManager Arms { get { return m_rRita_ArmsManager; } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Awake()
	{
		base.Awake();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();
		CreateMagicDistance();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		base.Update();
		UpdateAttackMagicActionInput();
		UpdateMagesInstinctInput();
		UpdateScreenAttackInput();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Magic Distance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateMagicDistance()
	{
		int X			= LevelGrid.TileCountHorz - LevelGrid.TileHorzOffsetCount;
		int Y			= (LevelGrid.TileCountVertz - LevelGrid.TileVertzOffsetCount) - 1; // -1 For Array Index Offset
		m_fMaxDistance	= Vector3.Distance(LevelGrid.GetTile(X, Y).transform.position, transform.position);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Magic Action Input
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateAttackMagicActionInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			int iMagicID = GetAvailableMagicID();
			if(iMagicID != NOT_AVAILABLE)
			{
				Vector3 vMagicTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3 vMagicStartPos;

				// Depending on Where the Magic is heading, show appropritae visuals
				if (vMagicTargetPos.x > Position.x)
				{
					Arms.ExtendLeftArm();
					vMagicStartPos = Arms.LeftArmPos;
				}
				else
				{
					Arms.ExtendRightArm();
					vMagicStartPos = Arms.RightArmPos;
				}
				
				float fRotationAngle = Mathf.Atan2((vMagicTargetPos.y - vMagicStartPos.y), (vMagicTargetPos.x - vMagicStartPos.x)) * Mathf.Rad2Deg;
				StartCoroutine(Shoot_ConsecutiveFireballs(iMagicID, fRotationAngle, vMagicTargetPos, vMagicStartPos));
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Shoot Fireballs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ShootFireball(float fRotationAngle, Vector3 vMagicTargetPos, Vector3 vMagicStartPos)
	{
		Fireball_AttackMagic attackMagicObject = m_rAttackMagicPool.GetFreeObject();
		attackMagicObject.gameObject.SetActive(true);
		attackMagicObject.transform.position = vMagicStartPos;
		attackMagicObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, fRotationAngle));
		attackMagicObject.m_vTargetPos = vMagicTargetPos;
		attackMagicObject.Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Shoot Consecutive Fireballs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator Shoot_ConsecutiveFireballs(int iMagicID, float fRotationAngle, Vector3 vMagicTargetPos, Vector3 vMagicStartPos)
	{
		for (int i = 1; i < TOTAL_FIREBALLS_PER_ATTACK; ++i) // Start 'i' at one so that we may do the last Firball attack after the 'for loop' in order to avoid the wait timer.
		{
			ShootFireball(fRotationAngle, vMagicTargetPos, vMagicStartPos);
			m_attMagicCooldownTimers[iMagicID].Reset();
			m_attMagicCooldownTimers[iMagicID].SetTimerCompletionPercentage(1.0f - (Vector3.Distance(vMagicTargetPos, vMagicStartPos) / m_fMaxDistance));
			yield return new WaitForSeconds(WAIT_TIME_BETWEEN_FIREBALLS);
		}

		ShootFireball(fRotationAngle, vMagicTargetPos, vMagicStartPos);
		m_attMagicCooldownTimers[iMagicID].Reset();
		m_attMagicCooldownTimers[iMagicID].SetTimerCompletionPercentage(1.0f - (Vector3.Distance(vMagicTargetPos, vMagicStartPos) / m_fMaxDistance));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Mages Instinct Input
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateMagesInstinctInput()
	{
		if (Input.GetMouseButtonDown(1))
		{
			ToggleMagesInstinct();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Screen Attack Input
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateScreenAttackInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GetComponent<Rita_ScreenMagicManager>().BeginScreenAttack();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Screen Attack Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void BeginScreenAttackAnimation()
	{
		GetAnimator().SetBool(GetParamHashIDs().PowerUpParamID, true);
		Arms.gameObject.SetActive(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: End Screen Attack Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void EndScreenAttackAnimation()
	{
		GetAnimator().SetBool(GetParamHashIDs().PowerUpParamID, false);
		Arms.gameObject.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Screen Attack Animation Completed?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override bool ScreenAttackAnimationCompleted()
	{
		return (GetAnimator().GetCurrentAnimatorStateInfo(0).nameHash == GetStateHashIDs().ScreenAttackWaitStateID);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Discover Total Timer Count
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override int DiscoverTotalTimerCount()
	{
		return 3;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static Player_RitaAnimationHashIDs.AnimationStateHashIDs GetStateHashIDs()
	{
		return Player_RitaAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Get Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static Player_RitaAnimationHashIDs.AnimationParamHashIDs GetParamHashIDs()
	{
		return Player_RitaAnimationHashIDs.GetParamHashIDs();
	}
}
