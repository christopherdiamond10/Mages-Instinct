using UnityEngine;
using System.Collections;

public class EnemyRomlingAnimationHashIDs
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
		public int WalkLeftStateID;
		public int WalkRightStateID;
		public int FallingLeftStateID;
		public int FallingRightStateID;
		public int DestroyPlatformStallingLeftStateID;
		public int DestroyPlatformStallingRightStateID;
		public int DestroyPlatformLeftStateID;
		public int DestroyPlatformRightStateID;
		public int DamageJumpStateID;
		public int DamageJumpLeftStateID;
		public int DamageJumpRightStateID;
		public int DeathBurntLeftStateID;
		public int DeathBurntRightStateID;
		public int DeathElectrocutedLeftStateID;
		public int DeathElectrocutedRightStateID;
		public int DeathCrushedLeftStateID;
		public int DeathCrushedRightStateID;
	};

	public struct AnimationParamHashIDs
	{
		public int WalkingRightParamID;
		public int FallingParamID;
		public int DestroyPlatformStallingParamID;
		public int DestroyPlatformParamID;
		public int DamageJumpParamID;
		public int DamageJumpRightParamID;
		public int DeathBurntParamID;
		public int DeathElectrocutedParamID;
		public int DeathCrushedParamID;
	};
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationStateHashIDs SetupStateHashIDs()
	{
		AnimationStateHashIDs StateIDs;

		StateIDs.WalkLeftStateID						=	Animator.StringToHash(	"Base Layer.WalkLeft"			);
		StateIDs.WalkRightStateID						=	Animator.StringToHash(	"Base Layer.WalkRight"			);
		StateIDs.FallingLeftStateID						=	Animator.StringToHash(	"Base Layer.FallingLeft"		);
		StateIDs.FallingRightStateID					=	Animator.StringToHash(	"Base Layer.FallingRight"		);
		StateIDs.DestroyPlatformStallingLeftStateID		=	Animator.StringToHash(	"Base Layer.DestroyPlatformStallingLeft" );
		StateIDs.DestroyPlatformStallingRightStateID	=	Animator.StringToHash(	"Base Layer.DestroyPlatformStallingRight" );
		StateIDs.DestroyPlatformLeftStateID				=	Animator.StringToHash(	"Base Layer.DestroyPlatformLeft" );
		StateIDs.DestroyPlatformRightStateID			=	Animator.StringToHash(	"Base Layer.DestroyPlatformRight" );
		StateIDs.DamageJumpStateID						=	Animator.StringToHash(	"Base Layer.DamageJump"			);
		StateIDs.DamageJumpLeftStateID					=	Animator.StringToHash(	"Base Layer.DamageJumpLeft"		);
		StateIDs.DamageJumpRightStateID					=	Animator.StringToHash(	"Base Layer.DamageJumpRight"	);
		StateIDs.DeathBurntLeftStateID					=	Animator.StringToHash(	"Base Layer.DeathBurntLeft"		);
		StateIDs.DeathBurntRightStateID					=	Animator.StringToHash(	"Base Layer.DeathBurntRight"	);
		StateIDs.DeathElectrocutedLeftStateID			=	Animator.StringToHash(	"Base Layer.DeathElectrocutedLeft"	);
		StateIDs.DeathElectrocutedRightStateID			=	Animator.StringToHash(	"Base Layer.DeathElectrocutedRight"	);
		StateIDs.DeathCrushedLeftStateID				=	Animator.StringToHash(	"Base Layer.DeathCrushedLeft"	);
		StateIDs.DeathCrushedRightStateID				=	Animator.StringToHash(	"Base Layer.DeathCrushedRight"	);

		return StateIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
	{
		AnimationParamHashIDs ParamIDs;

		ParamIDs.WalkingRightParamID			=	Animator.StringToHash(	"WalkingRight"	 );
		ParamIDs.FallingParamID					=	Animator.StringToHash(	"Falling"		 );
		ParamIDs.DestroyPlatformStallingParamID	=	Animator.StringToHash(	"DestroyPlatformStalling" );
		ParamIDs.DestroyPlatformParamID			=	Animator.StringToHash(	"DestroyPlatform");
		ParamIDs.DamageJumpParamID				=	Animator.StringToHash(	"DamageJump"	 );
		ParamIDs.DamageJumpRightParamID			=	Animator.StringToHash(	"DamageJumpRight");
		ParamIDs.DeathBurntParamID				=	Animator.StringToHash(	"DeathBurnt"	 );
		ParamIDs.DeathElectrocutedParamID		=	Animator.StringToHash(	"DeathElectrocuted"	);
		ParamIDs.DeathCrushedParamID			=	Animator.StringToHash(	"DeathCrushed"	 );

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
