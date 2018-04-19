using UnityEngine;
using System.Collections;

public class Fireball_ScreenAttackMagic : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Class Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static AnimationParamHashIDs m_ParamHashIDs = SetupParamsHashIDs();
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float m_fIntroTime = 0.3f;
	public int m_iContactDamage = 80; 

	public Vector3 m_vIntroStartScale;
	public Vector3 m_vIntroEndScale;
	public Vector3 m_vExplosionScale;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TransformInterpreter m_TransInterInstance;
	private TimeTracker m_ttIntroTimer;
	private VisualPhase m_eVisualPhase = VisualPhase.INTRO;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TransformInterpreter.WorldPositionInterpreter	Position				{ get { return m_TransInterInstance.WorldPosition;	} }
	public TransformInterpreter.LocalRotationInterpreter	Rotation				{ get { return m_TransInterInstance.LocalRotation;	} }
	public TransformInterpreter.ScaleInterpreter			Scale					{ get { return m_TransInterInstance.Scale;			} }
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public struct AnimationParamHashIDs
	{
		public int ExplodingParamID;
	};

	private enum VisualPhase
	{
		INTRO,
		IDLE,
		EXPLODING,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Awake() 
	{
		m_ttIntroTimer = new TimeTracker(m_fIntroTime);
		m_TransInterInstance = new TransformInterpreter(this);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Reset()
	{
		m_eVisualPhase = VisualPhase.INTRO;
		m_ttIntroTimer.Reset();
		GetComponent<Collider2D>().enabled = false;
		GetAnimator().SetBool(m_ParamHashIDs.ExplodingParamID, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Update() 
	{
		switch (m_eVisualPhase)
		{
			case VisualPhase.INTRO:
			{
				UpdateIntroPhase();
				break;
			}

			case VisualPhase.IDLE:
			{
				UpdateIdlePhase();
				break;
			}

			case VisualPhase.EXPLODING:
			{
				UpdateExplodingPhase();
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Intro Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateIntroPhase()
	{
		m_ttIntroTimer.Update();
		if (m_ttIntroTimer.TimeUp())
		{
			m_eVisualPhase = VisualPhase.IDLE;
			Scale.Set(m_vIntroEndScale);
		}
		else
		{
			Scale.Set( Vector3.Lerp(m_vIntroStartScale, m_vIntroEndScale, m_ttIntroTimer.GetCompletionPercentage()) );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Idle Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateIdlePhase()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Exploding Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateExplodingPhase()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Attack
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginAttack()
	{
		Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Enter Exploding Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void EnterExplodingPhase()
	{
		m_eVisualPhase = VisualPhase.EXPLODING;
		GetComponent<Collider2D>().enabled = true;
		GetAnimator().SetBool(m_ParamHashIDs.ExplodingParamID, true);
		Scale.Set(m_vExplosionScale);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animator
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Animator GetAnimator()
	{
		return gameObject.GetComponent<Animator>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Static Method: Setup Animation Params Hash IDs
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
    {
		AnimationParamHashIDs ParamIDs;

        ParamIDs.ExplodingParamID = Animator.StringToHash( "Exploding" );

		return ParamIDs;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger Enter 2D
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag.Contains("Enemy"))
		{
			collider.gameObject.GetComponent<Enemy_Base>().ReduceHealth(m_iContactDamage, Enemy_Base.DeathType.BURNT, transform.position);
		}
	}
}
