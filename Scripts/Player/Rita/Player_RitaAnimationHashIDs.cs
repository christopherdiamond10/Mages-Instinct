using UnityEngine;
using System.Collections;

public class Player_RitaAnimationHashIDs 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationStateHashIDs m_StateHashIDs = SetupStateHashIDs();
	private static AnimationParamHashIDs m_ParamHashIDs = SetupParamsHashIDs();
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public struct AnimationStateHashIDs
	{
		public int NormalAnimationStateID;
		public int ScreenAttackPowerUpStateID;
		public int ScreenAttackWaitStateID;
	};

	public struct AnimationParamHashIDs
	{
		public int PowerUpParamID;
	};
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationStateHashIDs SetupStateHashIDs()
	{
		AnimationStateHashIDs StateIDs;

		StateIDs.NormalAnimationStateID				=	Animator.StringToHash(	"Base Layer.Rita_PlayerAnimation"					 );
		StateIDs.ScreenAttackPowerUpStateID			=	Animator.StringToHash(	"Base Layer.Rita_PlayerScreenAttackPowerUpAnimation" );
		StateIDs.ScreenAttackWaitStateID			=	Animator.StringToHash(	"Base Layer.Rita_PlayerScreenAttackWait"			 );

		return StateIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
	{
		AnimationParamHashIDs ParamIDs;

		ParamIDs.PowerUpParamID	= Animator.StringToHash( "PowerUp" );

		return ParamIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static AnimationStateHashIDs GetStateHashIDs()
	{
		return m_StateHashIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Param Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static AnimationParamHashIDs GetParamHashIDs()
	{
		return m_ParamHashIDs;
	}
}