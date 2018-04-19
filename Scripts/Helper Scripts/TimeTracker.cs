//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Time Tracker
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: August 1, 2013
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script contains a time tracking class which you can use as a timer.
//	  The idea is that you can this script as a way to shorten any code that 
//	  requires a wait timer in actual seconds.
//
//-------------------------------------------------------------------------------
//	Instructions:
//
//	~	Simply create a new instance of this class and call the update function 
//		as needed.
//
//  ~   If you allow this class to add itself to the DynamicUpdateList, you do not 
//      need to call 'Update()', because it will be done automatically.
//
//	~	Check whether or not Time is Up with the 'TimeUp()' function.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TimeTracker
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float m_fCurrentTime	= 0.0f;
	private float m_fWaitTimer		= 5.0f;
	private bool m_bTimeUp			= false;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+- Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float CurrentTime
	{
		get 
		{ 
			return m_fCurrentTime; 
		}
		set 
		{ 
			m_fCurrentTime = value;
			if (CheckIfTimeIsUpExact()) ForceTimerFinish();
		}
	}

	public float FinishTime
	{
		get
		{
			return m_fWaitTimer;
		}
		set
		{
			m_fWaitTimer = value;
			if (CheckIfTimeIsUpExact()) ForceTimerFinish();
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Constructor
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TimeTracker(float TotalWaitTime)
	{
		m_fWaitTimer = TotalWaitTime;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*~ Deconstructor
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~TimeTracker()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Update Timer
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Update()
	{
		if (!TimeUp())
		{
			m_fCurrentTime += Time.deltaTime;

			if (CheckIfTimeIsUp())
			{
				ForceTimerFinish();
			}
		}
	}

	public void Update(float TimeMultiplier)
	{
		if (!TimeUp())
		{
			m_fCurrentTime += Time.deltaTime * TimeMultiplier;

			if (CheckIfTimeIsUp())
			{
				ForceTimerFinish();
			}
		}
	}

	public void UpdateWithRealtime()
	{
		if (!TimeUp())
		{
			m_fCurrentTime += GameManager.DeltaTime;

			if (CheckIfTimeIsUp())
			{
				ForceTimerFinish();
			}
		}
	}

	public void UpdateWithRealtime(float TimeMultiplier)
	{
		if (!TimeUp())
		{
			m_fCurrentTime += GameManager.DeltaTime * TimeMultiplier;

			if (CheckIfTimeIsUp())
			{
				ForceTimerFinish();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check if Time Is Up
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool CheckIfTimeIsUp()
	{
		return (m_fCurrentTime > m_fWaitTimer);
	}

	private bool CheckIfTimeIsUpExact()
	{
		return (m_fCurrentTime >= m_fWaitTimer);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Time Up?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool TimeUp()
	{
		return m_bTimeUp;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Completion Percentage
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float GetCompletionPercentage()
	{
		return (m_fCurrentTime / m_fWaitTimer);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Completion Percentage
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetTimerCompletionPercentage(float Percent)
	{
		float f = (m_fWaitTimer * Percent);
		m_fCurrentTime = Mathf.Max( 0.0f,  Mathf.Min(f, m_fWaitTimer)  );
		m_bTimeUp = CheckIfTimeIsUpExact();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Force The Timer to Finish
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ForceTimerFinish()
	{
		m_fCurrentTime = m_fWaitTimer;
		m_bTimeUp = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Timer
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Reset()
	{
		m_fCurrentTime = 0.0f;
		m_bTimeUp = false;
	}
}
