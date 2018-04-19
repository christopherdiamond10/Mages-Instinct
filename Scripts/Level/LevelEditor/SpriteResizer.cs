using UnityEngine;
using System.Collections;

public class SpriteResizer : MonoBehaviour
{
	public void ResizeSprite()
	{
		if (gameObject.GetComponent<SpriteRenderer>().sprite != null)
		{
			transform.localScale = new Vector3((transform.localScale.x * (LevelGrid.GetTileSize().x / gameObject.GetComponent<SpriteRenderer>().bounds.size.x)),
											   (transform.localScale.y * (LevelGrid.GetTileSize().y / gameObject.GetComponent<SpriteRenderer>().bounds.size.y)),
												transform.localScale.z);
		}
	}
}
