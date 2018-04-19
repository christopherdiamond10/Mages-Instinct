using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fireball_AttackMagic : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*= CONSTANTS
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	const float IMPACT_ANIMATION_TIME = 1.0f;
	const float DISTANCE_TOLERANCE = 2.3f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*= Static Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static List<Fireball_AttackMagic> sm_lAttackList = new List<Fireball_AttackMagic>(); 
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int						m_iImpactDamage	= 40;
	public float					m_fSpeed		= 3.0f;
	public float					m_fFadeinTime	= 0.01f;
	public float					m_fFadeoutTime	= 0.02f;
	public Vector2					m_vTargetPos;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector2					m_vDirection;
	private int						m_iImpactParamHashID;
	private Collider2D				m_ImpactTargetCollider		= null;
	private bool					m_bImpacted					= false;
	private float					m_fFadeoutStartAlpha		= 1.0f;
	private AttackPhase				m_eAttackPhase				= AttackPhase.FADEIN;
	private TimeTracker				m_ttFadeinTimer				= new TimeTracker(1.0f);
	private TimeTracker				m_ttFadeoutTimer			= new TimeTracker(1.0f);
	private TransformInterpreter	m_tiTransformInterpreter;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Color											SpriteColour	{ get { return GetComponent<SpriteRenderer>().color; }  set { GetComponent<SpriteRenderer>().color = value; } }
	private bool										   CollisionDetector{ set { GetComponent<BoxCollider2D>().enabled = value; } }
	private TransformInterpreter.LocalPositionInterpreter	LocalPosition	{ get { return m_tiTransformInterpreter.LocalPosition; } }
	private TransformInterpreter.WorldPositionInterpreter	Position		{ get { return m_tiTransformInterpreter.WorldPosition; } }
	private TransformInterpreter.LocalRotationInterpreter	Rotation		{ get { return m_tiTransformInterpreter.LocalRotation; } }
	private TransformInterpreter.ScaleInterpreter			Scale			{ get { return m_tiTransformInterpreter.Scale; } }
	
	private bool Impacted
	{
		get
		{
			return m_bImpacted;
		}
		set
		{
			m_bImpacted = value;
			GetComponent<Animator>().SetBool(m_iImpactParamHashID, m_bImpacted);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum AttackPhase
	{
		FADEIN,
		MOVING,
		FADEOUT,
		IMPACT,
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Start()
	{
		sm_lAttackList.Add(this);

		m_tiTransformInterpreter = new TransformInterpreter(this);
		m_ttFadeinTimer.FinishTime = m_fFadeinTime;
		m_ttFadeoutTimer.FinishTime = m_fFadeoutTime;
		m_iImpactParamHashID = Animator.StringToHash("Impacted");
		Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Reset()
	{
		m_ttFadeinTimer.Reset();
		m_ttFadeoutTimer.Reset();
		Impacted = false;
		m_ImpactTargetCollider = null;
		CollisionDetector = true;
		m_eAttackPhase = AttackPhase.FADEIN;
		m_vDirection = (m_vTargetPos - (Vector2)transform.position).normalized;
		
		Color alpha = SpriteColour;
		alpha.a = 0.0f;
		SpriteColour = alpha;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Update()
	{
		switch (m_eAttackPhase)
		{
			case AttackPhase.FADEIN:
			{
				MoveTowardsTarget();
				UpdateMovementEndCheck();
				FadeIn();
				break;
			}

			case AttackPhase.MOVING:
			{
				MoveTowardsTarget();
				UpdateMovementEndCheck();
				break;
			}

			case AttackPhase.FADEOUT:
			{
				//MoveTowardsTarget();
				UpdateMovementEndCheck();
				FadeOut();
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Fade in
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void FadeIn()
	{
		m_ttFadeinTimer.Update();
		if (m_ttFadeinTimer.TimeUp())
		{
			SpriteColour = Color.white;
			m_eAttackPhase = AttackPhase.MOVING;
		}
		else
		{
			Color alpha = SpriteColour;
			alpha.a = Mathf.Lerp(0.0f, 1.0f, m_ttFadeinTimer.GetCompletionPercentage());
			SpriteColour = alpha;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Fade out
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void FadeOut()
	{
		m_ttFadeoutTimer.Update();
		if (m_ttFadeoutTimer.TimeUp())
		{
			this.gameObject.SetActive(false);
		}
		else
		{
			Color alpha = SpriteColour;
			alpha.a = Mathf.Lerp(m_fFadeoutStartAlpha, 0.0f, m_ttFadeoutTimer.GetCompletionPercentage());
			SpriteColour = alpha;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Move Towards Target Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void MoveTowardsTarget()
	{
		Position.Set((Vector2)Position.Get() + (m_vDirection * m_fSpeed * Time.deltaTime));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Movement End Check
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateMovementEndCheck()
	{
		// If it's expected that we will reach the target vector within the next frame or so. Set to fadeout animation.
		if (Vector2.Distance(Position.Get(), m_vTargetPos) < (m_fSpeed * Time.deltaTime * DISTANCE_TOLERANCE))
		{
			m_fFadeoutStartAlpha = SpriteColour.a;
			m_eAttackPhase = AttackPhase.FADEOUT;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Impact
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator OnImpact(Enemy_Base enemy, Vector3 impactPosition)
	{
		// Show Half of the Animation first
		SpriteColour = Color.white;
		yield return new WaitForSeconds(IMPACT_ANIMATION_TIME * 0.3f);

		// Now that we're halfway done, Hurt thy enemy
		enemy.IsFrozen = false;
		Enemy_Base.DeathType eDeath = ((Random.Range(0, 2) == 0) ? Enemy_Base.DeathType.BURNT : (Random.Range(0, 2) == 0 ? Enemy_Base.DeathType.CRUSHED : Enemy_Base.DeathType.ELECTROCUTED));
		enemy.ReduceHealth(m_iImpactDamage, eDeath, impactPosition);
		yield return new WaitForSeconds(IMPACT_ANIMATION_TIME * 0.7f);
		m_ImpactTargetCollider = null;
		this.gameObject.SetActive(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Impact is Possible?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool ImpactPossible(Collider2D col)
	{
		foreach(Fireball_AttackMagic a in sm_lAttackList)
		{
			if(a.m_ImpactTargetCollider == col)
				return false;
		}
		return true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Collision
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!Impacted && collider.tag.Contains("Enemy"))
		{
			Enemy_Base enemy = collider.gameObject.GetComponent<Enemy_Base>();
			if (enemy.Hurtable && ImpactPossible(collider))
            {
				m_ImpactTargetCollider = collider;

				enemy.IsFrozen = true;			// Also freeze it from moving
				Vector3 impactPosition = Position;
				Position.Set(enemy.Position);	// Lock fireball impact animation to the current position of the enemy
				CollisionDetector = false;		// Turn off the Collision Detector


				Impacted = true;
				m_eAttackPhase = AttackPhase.IMPACT;
				StartCoroutine(OnImpact(enemy, impactPosition));
            }
		}
	}
}
