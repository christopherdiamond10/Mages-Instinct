using UnityEngine;
using System.Collections;

public class Rita_ArmsManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Class Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static AnimationParamHashIDs m_ParamHashIDs = SetupParamsHashIDs();
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float m_fArmExtensionTime = 1.0f;

	public Vector3 m_vLeftArmEndPosition;
	public Vector3 m_vRightArmEndPosition;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Animator	m_rAnimator;
	private bool		m_bExtendedLeftArm = false;
	private bool		m_bExtendedRightArm = false;
	private TimeTracker m_ttLeftArmTimer;
	private TimeTracker m_ttRightArmTimer;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 LeftArmPos	{ get { return m_vLeftArmEndPosition; } }
	public Vector3 RightArmPos	{ get { return m_vRightArmEndPosition; } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public struct AnimationParamHashIDs
	{
		public int ExtendedLeftArmParamID;
		public int ExtendedRightArmParamID;
	};



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		m_rAnimator = GetComponent<Animator>();
		m_ttLeftArmTimer = new TimeTracker(m_fArmExtensionTime);
		m_ttRightArmTimer = new TimeTracker(m_fArmExtensionTime);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_bExtendedLeftArm)
		{
			m_ttLeftArmTimer.Update();
			if (m_ttLeftArmTimer.TimeUp())
			{
				ContractLeftArm();
			}
		}
		
		if (m_bExtendedRightArm)
		{
			m_ttRightArmTimer.Update();
			if (m_ttRightArmTimer.TimeUp())
			{
				ContractRightArm();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Extend Left Arm
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ExtendLeftArm()
	{
		m_ttLeftArmTimer.Reset();
		m_bExtendedLeftArm = true;

		m_rAnimator.SetBool( m_ParamHashIDs.ExtendedLeftArmParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Extend Right Arm
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ExtendRightArm()
	{
		m_ttRightArmTimer.Reset();
		m_bExtendedRightArm = true;

		m_rAnimator.SetBool( m_ParamHashIDs.ExtendedRightArmParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Contract Left Arm
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ContractLeftArm()
	{
		m_bExtendedLeftArm = false;

		m_rAnimator.SetBool( m_ParamHashIDs.ExtendedLeftArmParamID, false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Contract Right Arm
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ContractRightArm()
	{
		m_bExtendedRightArm = false;

		m_rAnimator.SetBool( m_ParamHashIDs.ExtendedRightArmParamID, false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Static Method: Setup Animation Params Hash IDs
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
    {
		AnimationParamHashIDs ParamIDs;

        ParamIDs.ExtendedLeftArmParamID		= Animator.StringToHash( "ExtendedLeftArm"  );
		ParamIDs.ExtendedRightArmParamID	= Animator.StringToHash( "ExtendedRightArm" );

		return ParamIDs;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Draw Gizmos
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(m_vLeftArmEndPosition, Vector3.one * 0.025f);
		Gizmos.DrawCube(m_vRightArmEndPosition, Vector3.one * 0.025f);
	}
}
