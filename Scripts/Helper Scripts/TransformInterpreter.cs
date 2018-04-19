//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Transform Interpreter
//#             Author: DiamondandPlatinum3
//#             Date: February 4, 2014
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script interacts with the Transform Class, allowing easier modifications
//#		made to the Position/Rotation/Scale Values that would normally be tedious to 
//#		try and change.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;


public partial class TransformInterpreter
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private MonoBehaviour				m_rOwnerInstance	= null;
	private Transform					m_tInstance			= null;
	private WorldPositionInterpreter	m_wpiInstance		= null;
	private LocalPositionInterpreter	m_lpiInstance		= null;
	private WorldRotationInterpreter	m_wriInstance		= null;
	private LocalRotationInterpreter	m_lriInstance		= null;
	private ScaleInterpreter			m_siInstance		= null;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Constructors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TransformInterpreter(MonoBehaviour GmObject)
	{
		m_rOwnerInstance = GmObject;
		m_tInstance = GmObject.transform;
		Create();
	}

	public TransformInterpreter(Transform t)
	{
		m_tInstance = t;
		Create();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Create()
	{
		m_wpiInstance	= new WorldPositionInterpreter(this);
		m_lpiInstance	= new LocalPositionInterpreter(this);
		m_wriInstance	= new WorldRotationInterpreter(this);
		m_lriInstance	= new LocalRotationInterpreter(this);
		m_siInstance	= new ScaleInterpreter(this);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Transform
	//----------------------------------------------------
	// It is possible that the reference to the transform will be changed in the original script. 
	// So if the instance is set: get that transform instance instead of our possibly outdated one.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Transform GetTransform()
	{
		return (m_rOwnerInstance.transform != null) ? m_rOwnerInstance.transform :  m_tInstance;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static implicit operator Transform(TransformInterpreter a)
	{
		return a.GetTransform();
	}

	public WorldPositionInterpreter	WorldPosition { get { return m_wpiInstance; } }
	public LocalPositionInterpreter LocalPosition { get { return m_lpiInstance; } }
	public WorldRotationInterpreter	WorldRotation { get { return m_wriInstance; } }
	public LocalRotationInterpreter LocalRotation { get { return m_lriInstance; } }
	public ScaleInterpreter			Scale		  { get { return m_siInstance;  } }
}



//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             World Position Interpreter
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
public partial class TransformInterpreter
{
	public class WorldPositionInterpreter
	{
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	*- Private Instance Variables
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private TransformInterpreter m_tiInstance = null;
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	** Constructor
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public WorldPositionInterpreter(TransformInterpreter tiInstance)
		{
			m_tiInstance = tiInstance;
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* Accessors
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public static implicit operator Vector3(WorldPositionInterpreter a)
		{
			return a.Get();
		}

		public Vector3 Get()
		{
			return m_tiInstance.GetTransform().position;
		}

		public void Set(Vector3 a)
		{
			m_tiInstance.GetTransform().position = a;
		}

		public float x
		{
			get { return m_tiInstance.GetTransform().position.x; }
			set { m_tiInstance.GetTransform().position = new Vector3(value, this.y, this.z); }
		}

		public float y
		{
			get { return m_tiInstance.GetTransform().position.y; }
			set { m_tiInstance.GetTransform().position = new Vector3(this.x, value, this.z); }
		}

		public float z
		{
			get { return m_tiInstance.GetTransform().position.z; }
			set { m_tiInstance.GetTransform().position = new Vector3(this.x, this.y, value); }
		}
	}
}



//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Local Position Interpreter
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
public partial class TransformInterpreter
{
	public class LocalPositionInterpreter
	{
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	*- Private Instance Variables
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private TransformInterpreter m_tiInstance = null;
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	** Constructor
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public LocalPositionInterpreter(TransformInterpreter tiInstance)
		{
			m_tiInstance = tiInstance;
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* Accessors
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public static implicit operator Vector3(LocalPositionInterpreter a)
		{
			return a.Get();
		}

		public Vector3 Get()
		{
			return m_tiInstance.GetTransform().localPosition;
		}

		public void Set(Vector3 a)
		{
			m_tiInstance.GetTransform().localPosition = a;
		}

		public float x
		{
			get { return m_tiInstance.GetTransform().localPosition.x; }
			set { m_tiInstance.GetTransform().localPosition = new Vector3(value, this.y, this.z); }
		}

		public float y
		{
			get { return m_tiInstance.GetTransform().localPosition.y; }
			set { m_tiInstance.GetTransform().localPosition = new Vector3(this.x, value, this.z); }
		}

		public float z
		{
			get { return m_tiInstance.GetTransform().localPosition.z; }
			set { m_tiInstance.GetTransform().localPosition = new Vector3(this.x, this.y, value); }
		}
	}
}



//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             World Rotation Interpreter
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
public partial class TransformInterpreter
{
	public class WorldRotationInterpreter
	{
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	*- Private Instance Variables
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private TransformInterpreter m_tiInstance = null;
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	** Constructor
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public WorldRotationInterpreter(TransformInterpreter tiInstance)
		{
			m_tiInstance = tiInstance;
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* Accessors
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public static implicit operator Quaternion(WorldRotationInterpreter a)
		{
			return a.Get();
		}

		public static implicit operator Vector2(WorldRotationInterpreter a)
		{
			return a.GetAsVector();
		}

		public static implicit operator Vector3(WorldRotationInterpreter a)
		{
			return a.GetAsVector();
		}

		public static implicit operator Vector4(WorldRotationInterpreter a)
		{
			return a.GetAsVector();
		}

		public Quaternion Get()
		{
			return m_tiInstance.GetTransform().rotation;
		}

		public Vector4 GetAsVector()
		{
			return new Vector4(this.x, this.y, this.z, this.w);
		}

		public void Set(Vector2 a)
		{
			m_tiInstance.GetTransform().rotation = Quaternion.Euler(a);
		}

		public void Set(Vector3 a)
		{
			m_tiInstance.GetTransform().rotation = Quaternion.Euler(a);
		}

		public void Set(Vector4 a)
		{
			m_tiInstance.GetTransform().rotation = Quaternion.Euler(a);
		}

		public void Set(Quaternion a)
		{
			m_tiInstance.GetTransform().rotation = a;
		}

		public float x
		{
			get { return m_tiInstance.GetTransform().rotation.x; }
			set { m_tiInstance.GetTransform().rotation = new Quaternion(value, this.y, this.z, this.w); }
		}

		public float y
		{
			get { return m_tiInstance.GetTransform().rotation.y; }
			set { m_tiInstance.GetTransform().rotation = new Quaternion(this.x, value, this.z, this.w); }
		}

		public float z
		{
			get { return m_tiInstance.GetTransform().rotation.z; }
			set { m_tiInstance.GetTransform().rotation = new Quaternion(this.x, this.y, value, this.w); }
		}

		public float w
		{
			get { return m_tiInstance.GetTransform().rotation.w; }
			set { m_tiInstance.GetTransform().rotation = new Quaternion(this.x, this.y, this.z, value); }
		}
	}
}



//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Local Rotation Interpreter
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
public partial class TransformInterpreter
{
	public class LocalRotationInterpreter
	{
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	*- Private Instance Variables
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private TransformInterpreter m_tiInstance = null;
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	** Constructor
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public LocalRotationInterpreter(TransformInterpreter tiInstance)
		{
			m_tiInstance = tiInstance;
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* Accessors
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public static implicit operator Quaternion(LocalRotationInterpreter a)
		{
			return a.Get();
		}

		public static implicit operator Vector2(LocalRotationInterpreter a)
		{
			return a.GetAsVector();
		}

		public static implicit operator Vector3(LocalRotationInterpreter a)
		{
			return a.GetAsVector();
		}

		public static implicit operator Vector4(LocalRotationInterpreter a)
		{
			return a.GetAsVector();
		}

		public Quaternion Get()
		{
			return m_tiInstance.GetTransform().localRotation;
		}

		public Vector4 GetAsVector()
		{
			return new Vector4(this.x, this.y, this.z, this.w);
		}

		public void Set(Vector2 a)
		{
			m_tiInstance.GetTransform().localRotation = new Quaternion(a.x, a.y, this.z, this.w);
		}

		public void Set(Vector3 a)
		{
			m_tiInstance.GetTransform().localRotation = new Quaternion(a.x, a.y, a.z, this.w);
		}

		public void Set(Vector4 a)
		{
			m_tiInstance.GetTransform().localRotation = new Quaternion(a.x, a.y, a.z, a.w);
		}

		public void Set(Quaternion a)
		{
			m_tiInstance.GetTransform().localRotation = a;
		}

		public float x
		{
			get { return m_tiInstance.GetTransform().localRotation.x; }
			set { m_tiInstance.GetTransform().localRotation = new Quaternion(value, this.y, this.z, this.w); }
		}

		public float y
		{
			get { return m_tiInstance.GetTransform().localRotation.y; }
			set { m_tiInstance.GetTransform().localRotation = new Quaternion(this.x, value, this.z, this.w); }
		}

		public float z
		{
			get { return m_tiInstance.GetTransform().localRotation.z; }
			set { m_tiInstance.GetTransform().localRotation = new Quaternion(this.x, this.y, value, this.w); }
		}

		public float w
		{
			get { return m_tiInstance.GetTransform().localRotation.w; }
			set { m_tiInstance.GetTransform().localRotation = new Quaternion(this.x, this.y, this.z, value); }
		}
	}
}



//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Scale Interpreter
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
public partial class TransformInterpreter
{
	public class ScaleInterpreter
	{
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	*- Private Instance Variables
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private TransformInterpreter m_tiInstance = null;
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	** Constructor
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public ScaleInterpreter(TransformInterpreter tiInstance)
		{
			m_tiInstance = tiInstance;
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* Accessors
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public static implicit operator Vector2(ScaleInterpreter a)
		{
			return a.Get();
		}

		public static implicit operator Vector3(ScaleInterpreter a)
		{
			return a.Get();
		}

		public Vector3 Get()
		{
			return m_tiInstance.GetTransform().localScale;
		}

		public void Set(Vector2 a)
		{
			m_tiInstance.GetTransform().localScale = a;
		}

		public void Set(Vector3 a)
		{
			m_tiInstance.GetTransform().localScale = a;
		}

		public float x
		{
			get { return m_tiInstance.GetTransform().localScale.x; }
			set { m_tiInstance.GetTransform().localScale = new Vector3(value, this.y, this.z); }
		}

		public float y
		{
			get { return m_tiInstance.GetTransform().localScale.y; }
			set { m_tiInstance.GetTransform().localScale = new Vector3(this.x, value, this.z); }
		}

		public float z
		{
			get { return m_tiInstance.GetTransform().localScale.z; }
			set { m_tiInstance.GetTransform().localScale = new Vector3(this.x, this.y, value); }
		}
	}
}