using UnityEngine;
using System.Collections;

public class DestroyZone : MonoBehaviour 
{
	void OnCollisionEnter2D(Collision2D collision)
	{
		Destroy(collision.gameObject);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.156f, 0.607f, 0.5f);
		Gizmos.DrawCube(transform.position, GetComponent<BoxCollider2D>().size);
	}
}
