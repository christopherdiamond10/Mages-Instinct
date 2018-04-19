using UnityEngine;
using System.Collections;

public class LacelingTrailObject : MonoBehaviour 
{
	private TimeTracker m_ttActiveTimer;

	void Update()
	{
		if (m_ttActiveTimer != null)
		{
			m_ttActiveTimer.Update();
			if (m_ttActiveTimer.TimeUp())
			{
				Destroy(gameObject);
			}
		}
	}

	public void Setup(Vector3 Position, Color Colour, float ActiveTime )
	{
		m_ttActiveTimer = new TimeTracker(ActiveTime);
		transform.position = Position;
		GetComponent<SpriteRenderer>().color = Colour;
	}
}
