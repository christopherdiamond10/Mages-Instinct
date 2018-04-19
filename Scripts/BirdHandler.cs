using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdHandler : MonoBehaviour
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*= CONSTANTS
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private const int	TOTAL_ITERATIONS						= 20;
    private const float VERTICAL_POS_BOUNDSTART_MULTIPLIER		= 0.3f;
    private const float VERTICAL_POS_BOUNDEND_MULTIPLIER		= 0.7f;
	private const float END_VERTICAL_POS_MULTIPLIER				= 1.3f;

	private const float MIDDLE_POS_TARGET_MULTIPLIER			= 0.7f;
	private const float HORIZONTAL_POS_RANGE_START_MULTIPLIER	= 0.45f;
	private const float HORIZONTAL_POS_RANGE_END_MULTIPLIER		= 0.9f;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public float m_fMinTimerRange = 2.0f;
    public float m_fMaxTimerRange = 6.0f;

    public float m_fLeftScreenPosition	= -1.0f;
    public float m_fRightScreenPosition = 1.0f;
    public float m_fTopScreenPosition	= 1.0f;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*- Private Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private TimeTracker m_ttMovementTransitionTimer = new TimeTracker(1.0f);
	private Vector3[] m_vMovementTargets = new Vector3[TOTAL_ITERATIONS];
	private int m_iCurrentElement = 0;
	private int m_iTotalElements = 0;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Start()
    {
		Reset();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Reset()
    {
		bool bLiftPath = (Random.Range(0, 2) == 0);
		if (bLiftPath)
		{
			CreateLiftPath();
		}
		else
		{
			m_ttMovementTransitionTimer.Reset();
			m_ttMovementTransitionTimer.FinishTime = Random.Range(m_fMinTimerRange, m_fMaxTimerRange);

			m_iCurrentElement = 0;
			m_iTotalElements = 2;
			CreateStraightPath();
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Lift Path
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateLiftPath()
	{
		// Reset Values
		m_ttMovementTransitionTimer.Reset();
		m_ttMovementTransitionTimer.FinishTime = (1.0f / TOTAL_ITERATIONS) * Random.Range(m_fMinTimerRange, m_fMaxTimerRange); // Amount of Bezier Curve Points * Total (Random) Time

		m_iCurrentElement = 0;
		m_iTotalElements = TOTAL_ITERATIONS;


		// Randomise Path Points
		bool bLeftScreen = (Random.Range(0, 2) == 0);
		float x, y, middley, endx;
		if (bLeftScreen)
        {
			x = m_fLeftScreenPosition;
			y = Random.Range((m_fTopScreenPosition * VERTICAL_POS_BOUNDSTART_MULTIPLIER), (m_fTopScreenPosition * VERTICAL_POS_BOUNDEND_MULTIPLIER));
			middley = Random.Range(y, y + (((m_fTopScreenPosition * END_VERTICAL_POS_MULTIPLIER) - y) * 0.5f));
			endx = Random.Range((m_fRightScreenPosition * HORIZONTAL_POS_RANGE_START_MULTIPLIER), (m_fRightScreenPosition * HORIZONTAL_POS_RANGE_END_MULTIPLIER));
        }
        else
        {
			x = m_fRightScreenPosition;
			y = Random.Range((m_fTopScreenPosition * VERTICAL_POS_BOUNDSTART_MULTIPLIER), (m_fTopScreenPosition * VERTICAL_POS_BOUNDEND_MULTIPLIER));
			middley = Random.Range(y, y + (((m_fTopScreenPosition * END_VERTICAL_POS_MULTIPLIER) - y) * 0.5f));
			endx = (m_fRightScreenPosition - Random.Range((m_fRightScreenPosition * HORIZONTAL_POS_RANGE_START_MULTIPLIER), (m_fRightScreenPosition * HORIZONTAL_POS_RANGE_END_MULTIPLIER)));
        }

		// Setup Bezier Curve
		transform.position = new Vector3(x, y, transform.position.z);
		m_vMovementTargets[0] = transform.position;
		m_vMovementTargets[1] = new Vector3( (endx * MIDDLE_POS_TARGET_MULTIPLIER), middley, transform.position.z);
		m_vMovementTargets[2] = new Vector3(endx, (m_fTopScreenPosition * END_VERTICAL_POS_MULTIPLIER), transform.position.z);
		
		CreateBezierCurve(TOTAL_ITERATIONS);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Straight Path
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateStraightPath()
	{
		bool bLeftScreen = (Random.Range(0, 2) == 0);
		float x, y, middley, endx;
		if (bLeftScreen)
		{
			x = m_fLeftScreenPosition;
			y = Random.Range((m_fTopScreenPosition * VERTICAL_POS_BOUNDSTART_MULTIPLIER), (m_fTopScreenPosition * VERTICAL_POS_BOUNDEND_MULTIPLIER));
			middley = Random.Range(y, y + (((m_fTopScreenPosition * END_VERTICAL_POS_MULTIPLIER) - y) * 0.5f));
			endx = Random.Range((m_fRightScreenPosition * HORIZONTAL_POS_RANGE_START_MULTIPLIER), (m_fRightScreenPosition * HORIZONTAL_POS_RANGE_END_MULTIPLIER));
		}
		else
		{
			x = m_fRightScreenPosition;
			y = Random.Range((m_fTopScreenPosition * VERTICAL_POS_BOUNDSTART_MULTIPLIER), (m_fTopScreenPosition * VERTICAL_POS_BOUNDEND_MULTIPLIER));
			middley = Random.Range(y, y + (((m_fTopScreenPosition * END_VERTICAL_POS_MULTIPLIER) - y) * 0.5f));
			endx = (m_fRightScreenPosition - Random.Range((m_fRightScreenPosition * HORIZONTAL_POS_RANGE_START_MULTIPLIER), (m_fRightScreenPosition * HORIZONTAL_POS_RANGE_END_MULTIPLIER)));
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Update()
    {
		if (m_iCurrentElement < (m_iTotalElements - 1))
		{
			m_ttMovementTransitionTimer.Update();
			if (m_ttMovementTransitionTimer.TimeUp())
			{
				m_ttMovementTransitionTimer.Reset();
				transform.position = m_vMovementTargets[m_iCurrentElement + 1];

				m_iCurrentElement += 1;
			}
			else
			{
				transform.position = Vector3.Lerp(m_vMovementTargets[m_iCurrentElement], m_vMovementTargets[m_iCurrentElement + 1], m_ttMovementTransitionTimer.GetCompletionPercentage());
			}
		}
		else
		{
			Reset();
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Bezier Curve
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void CreateBezierCurve(int iterations)
	{
		int i = 0;
		float stepping = 1.0f / iterations;
		Vector3 p1 = new Vector3(m_vMovementTargets[0].x, m_vMovementTargets[0].y, m_vMovementTargets[0].z);
		Vector3 p2 = new Vector3(m_vMovementTargets[1].x, m_vMovementTargets[1].y, m_vMovementTargets[1].z);
		Vector3 p3 = new Vector3(m_vMovementTargets[2].x, m_vMovementTargets[2].y, m_vMovementTargets[2].z);

		for (float x = 0.0f; x <= 1.0f; x += stepping)
		{
			Vector3 ap1 = Vector3.Lerp(p1, p2, x);
			Vector3 ap2 = Vector3.Lerp(p2, p3, x);
			//Vector3 ap3 = Vector3.Lerp(p3, p4, x);
       
			Vector3 bp1 = Vector3.Lerp(ap1, ap2, x);
			Vector3 bp2 = Vector3.Lerp(ap2, ap2, x);//ap3, x);

			m_vMovementTargets[i] = Vector3.Lerp(bp1, bp2, x);
			i += 1;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Draw Gizmos
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmos()
	{
		//CreateBezierCurve(20);
		for (int i = 0; i < (m_vMovementTargets.Length - 1); ++i)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(m_vMovementTargets[i], 0.03f);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(m_vMovementTargets[i], m_vMovementTargets[i + 1]);
		}
	}
}
