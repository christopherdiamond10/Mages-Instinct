using UnityEngine;
using System.Collections;

public class Enemy_Base : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int					m_iHealth					= 100;							// Starting Health
	public float				m_fMoveSpeedGain			= 150.0f;						// Movement Speed Gain
	public float				m_fMaxMovementSpeed			= 0.5f;							// Maximum Movement Speed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected int					m_iEnemyID				= -1;							// Enemy ID in AI_Manager
	protected int					m_iPlatformCollisionID	= -1;							// This Changes depending on which Block of platforms this Enemy is currently on. Should a Romling destroy a platform on this collision block, all Enemies standing on it need to recalculate their specialties
	protected bool					m_bDamageJumpActive		= false;						// Is Showing Damaged Indication?
	protected bool					m_bDirectionGiven		= false;						// Has been given direction by the platform it's standing on?
	protected bool					m_bIsAlive				= true;							// Is Alive?
	protected bool					m_bIsFrozen				= false;						// Is Frozen? Occurs when player initiates Screen Attack
	protected bool					m_bTempInvincibility	= false;						// Temporarily Invincible? Happens when enemy survives hit
	protected DamageFlashEffect		m_DamageFlashEffect		= new DamageFlashEffect();		// Flash Effect Instance;
	protected DeathType				m_eDeathType			= DeathType.BURNT;				// Death Animation
	protected MovementDirection		m_eDirection			= MovementDirection.RIGHT;		// Movement Direction
	protected TransformInterpreter	m_TransInterInstance	= null;							// Transform Interpreter
	protected TypeOfEnemy			m_eEnemyType			= TypeOfEnemy.STOOPLING;		// Enemy Type
	protected TimeTracker			m_ttDeathTimer			= new TimeTracker(1.0f);		// Timer Until Enemy is Destroyed After Death
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int												EnemyID					{ get { return m_iEnemyID;							} }
	public bool												IsAlive					{ get { return m_bIsAlive;							} }
	public bool												Hurtable				{ get { return !m_bTempInvincibility && IsAlive;	} }
	public MovementDirection								MovementDirectionFacing { get { return m_eDirection;						} }
	public TypeOfEnemy										EnemyType				{ get { return m_eEnemyType;						} }
	public TransformInterpreter.WorldPositionInterpreter	Position				{ get { return m_TransInterInstance.WorldPosition;	} }
	public Vector2											Velocity				{ get { return GetComponent<Rigidbody2D>().velocity;				} }
	public TransformInterpreter.LocalRotationInterpreter	Rotation				{ get { return m_TransInterInstance.LocalRotation;	} }
	public TransformInterpreter.ScaleInterpreter			Scale					{ get { return m_TransInterInstance.Scale;			} }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int PlatformCollisionID
	{
		get { return m_iPlatformCollisionID; }
		set { m_iPlatformCollisionID = value; }
	}

	public bool IsFrozen
	{
		get 
		{ 
			return m_bIsFrozen; 
		}
		set
		{
			if (value)
				Freeze();
			else
				UnFreeze();
		}
	}
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum TypeOfEnemy
	{
		STOOPLING,
		LACELING,
		ROMLING,
	};

	public enum MovementDirection
	{
		LEFT,				// Move to the Left
		RIGHT,				// Move to the Right
		TOWARDS_PLAYER,
	};

	public enum DeathType
	{
		BURNT,
		ELECTROCUTED,
		CRUSHED,
	};

	protected struct DamageFlashEffect
	{
		public Color OriginalColour;				// Sprite's Natural Colour
		public Color DamageColour;					// Sprite's Damage Colour
		public bool  ShowingDamageColour;			// Currently Showing Damage Colour or Normal Colour?
		public TimeTracker FlashTimer;				// Wait Timer Between Flashes
		public TimeTracker TotalFlashTimer;			// Total Flash Time Effect;
	};
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Awake()
	{
		m_TransInterInstance = new TransformInterpreter(this);
		m_iEnemyID			 = AI_Manager.Add_AI(this);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Destroy
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnDestroy()
	{
		AI_Manager.RemoveAI(EnemyID);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Start() 
	{
		SetupDamageFlashEffect();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Update() 
	{
		if (!IsAlive)
		{
			UpdateDeath();
			return;
		}

		if (IsInvincible())
		{
			UpdateDamageFlashEffect();
		}

		UpdateMovement();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Flash Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void SetupDamageFlashEffect()
	{
		float fFlashTime = 2.0f;
		float fTotalFlashes = 18.0f;
		m_DamageFlashEffect.OriginalColour		= GetComponent<Renderer>().material.color;
		m_DamageFlashEffect.DamageColour		= Color.red;
		m_DamageFlashEffect.ShowingDamageColour = false;
		m_DamageFlashEffect.FlashTimer			= new TimeTracker(fFlashTime / fTotalFlashes);	// Eighteen Flashes over Two Seconds. Nine Red Flashes
		m_DamageFlashEffect.TotalFlashTimer		= new TimeTracker(fFlashTime);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Death
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateDeath()
	{
		m_ttDeathTimer.Update();
		if (m_ttDeathTimer.TimeUp())
		{
			Destroy(gameObject);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Flash Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateDamageFlashEffect()
	{
		// Update and Check Total Flash Effect Time, If TimeUp - Turn off Flash Effect
		m_DamageFlashEffect.TotalFlashTimer.Update();
		if (m_DamageFlashEffect.TotalFlashTimer.TimeUp())
		{
			m_bTempInvincibility = false;
			GetComponent<Renderer>().material.color = m_DamageFlashEffect.OriginalColour;
			m_DamageFlashEffect.ShowingDamageColour = false;
			m_DamageFlashEffect.TotalFlashTimer.Reset();
		}

		else
		{
			// Update and check FlashTimer
			m_DamageFlashEffect.FlashTimer.Update();
			if( m_DamageFlashEffect.FlashTimer.TimeUp() )
			{
				m_DamageFlashEffect.FlashTimer.Reset();
				if (m_DamageFlashEffect.ShowingDamageColour)
				{
					GetComponent<Renderer>().material.color = m_DamageFlashEffect.OriginalColour;
					m_DamageFlashEffect.ShowingDamageColour = false;
				}
				else
				{
					GetComponent<Renderer>().material.color = m_DamageFlashEffect.DamageColour;
					m_DamageFlashEffect.ShowingDamageColour = true;
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateMovement()
	{
		if (IsGrounded())
		{
			if (m_bDamageJumpActive) 
			{ 
				StopDamageJumpIndication(); 
			}


			switch (MovementDirectionFacing)
			{
				case MovementDirection.LEFT:
				{
					MoveToLeft();
					break;
				}
				case MovementDirection.RIGHT:
				{
					MoveToRight();
					break;
				}
				case MovementDirection.TOWARDS_PLAYER:
				{
					if (GetPlayerPosition().x < Position.x)
					{
						SetFaceLeft();
						m_eDirection = MovementDirection.TOWARDS_PLAYER;
						MoveToLeft();
					}
					else
					{
						SetFaceRight();
						m_eDirection = MovementDirection.TOWARDS_PLAYER;
						MoveToRight();
					}

					break;
				}
				default:
				{
					break;
				}
			}
		}
		else
		{
			m_bDirectionGiven = false;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Enemy On the Ground?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsGrounded()
	{
		return (GetComponent<Rigidbody2D>().velocity.y > -0.001f) && (GetComponent<Rigidbody2D>().velocity.y < 0.001f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Enemy Falling?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsFalling()
	{
		return (GetComponent<Rigidbody2D>().velocity.y < -0.001f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Enemy Invincible?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsInvincible()
	{
		return m_bTempInvincibility;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Move Enemy Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void MoveToLeft()
	{
		if(GetComponent<Rigidbody2D>().velocity.x > -m_fMaxMovementSpeed)
			GetComponent<Rigidbody2D>().AddForce(-Vector2.right * m_fMoveSpeedGain * Time.deltaTime);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Move Enemy Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void MoveToRight()
	{
		if(GetComponent<Rigidbody2D>().velocity.x < m_fMaxMovementSpeed)
			GetComponent<Rigidbody2D>().AddForce(Vector2.right * m_fMoveSpeedGain * Time.deltaTime);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animator
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Animator GetAnimator()
	{
		return gameObject.GetComponent<Animator>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Box Collider
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public BoxCollider2D GetCollider()
	{
		return gameObject.GetComponent<BoxCollider2D>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Specialty
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void ResetSpecialty()
	{		 
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Facing Direction
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetFacingDirection(MovementDirection eDirection)
	{
		if (m_eDirection != eDirection)
		{
			if (eDirection == MovementDirection.TOWARDS_PLAYER)
			{
				m_eDirection = MovementDirection.TOWARDS_PLAYER;
			}
			else if (eDirection == MovementDirection.LEFT)
			{
				SetFaceLeft();
			}
			else
			{
				SetFaceRight();
			}

			m_bDirectionGiven = true;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Face Direction To Left
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void SetFaceLeft()
	{
		m_eDirection = MovementDirection.LEFT;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Face Direction to Right
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void SetFaceRight()
	{
		m_eDirection = MovementDirection.RIGHT;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Freeze
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Freeze()
	{
		m_bIsFrozen = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: UnFreeze
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void UnFreeze()
	{
		m_bIsFrozen = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reduce Health
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ReduceHealth(int iReductionCost, DeathType eDeathType, Vector2 vImpactObjectPosition)
	{
		if (!Hurtable)
			return;

		m_iHealth -= iReductionCost;
		if (m_iHealth < 1)
		{
			m_bIsAlive = false;
			PlayDeathAnimation(eDeathType);
			OnDeath();
		}
		else
		{
			m_bTempInvincibility = true;
			RunDamageJumpIndication(vImpactObjectPosition.x);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Run Damage Jump Indication
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void RunDamageJumpIndication(float fPointOfImpact)
	{
		// Make Enemy Jump Slightly Backwards
		float xVelocity = (25.0f * Time.deltaTime);
		float yVelocity = (50.0f * Time.deltaTime); 

		GetComponent<Rigidbody2D>().velocity = new Vector2( ((fPointOfImpact < Position.x) ? xVelocity : -xVelocity), yVelocity );

		m_bDamageJumpActive = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Damage Jump Indication
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void StopDamageJumpIndication()
	{
		m_bDamageJumpActive = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Death
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnDeath()
	{
		GetComponent<Rigidbody2D>().isKinematic = true;
		GetComponent<Rigidbody2D>().Sleep();
		GetCollider().enabled = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Death Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void PlayDeathAnimation(DeathType eDeathType)
	{
		switch (eDeathType)
		{
			case DeathType.BURNT:
			{
				PlayDeathBurntAnimation();
				break;
			}
			case DeathType.ELECTROCUTED:
			{
				PlayDeathElectrocutedAnimation();
				break;
			}
			case DeathType.CRUSHED:
			{
				PlayDeathCrushedAnimation();
				break;
			}
			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Death Burnt Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void PlayDeathBurntAnimation()
	{
		m_eDeathType = DeathType.BURNT;
		m_ttDeathTimer.FinishTime = 5.0f;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Death Electrocuted Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void PlayDeathElectrocutedAnimation()
	{
		m_eDeathType = DeathType.ELECTROCUTED;
		m_ttDeathTimer.FinishTime = 5.0f;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Death Crushed Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void PlayDeathCrushedAnimation()
	{
		m_eDeathType = DeathType.CRUSHED;
		m_ttDeathTimer.FinishTime = 5.0f;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Player Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static Vector2 GetPlayerPosition()
	{
		return ImportantObjectsManager.Player.transform.position;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Collision
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnCollisionEnter2D(Collision2D collision)
	{
		if (!Hurtable)
			return;

		if(collision.gameObject.tag.Contains("Magic"))
		{
			ReduceHealth(40, DeathType.BURNT, collision.transform.position);
		}
	}
}
