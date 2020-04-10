using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
	
	[SerializeField]
	private GameObject target;
	[SerializeField]
	private Camera camera;
	[SerializeField]
	private BoxCollider2D leftBorder;
	[SerializeField]
	private BoxCollider2D rightBorder;
	[SerializeField]
	private BoxCollider2D topBorder;
	[SerializeField]
	private BoxCollider2D bottomBorder;

	public float speed;

	public GameObject Target {
		set { target = value; }
	}

	private void LateUpdate() {
		if (!target)
			return;

		Vector2 newPosition = Vector2.Lerp(transform.position, target.transform.position, speed * Time.deltaTime);

		if (transform.position != new Vector3(newPosition.x, newPosition.y, transform.position.z))
			transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

		float widthOfScreen = (camera.ViewportToWorldPoint(new Vector3(1, 0.5f, camera.farClipPlane)).x - transform.position.x) * 2;
		float heightOfScreen = (camera.ViewportToWorldPoint(new Vector3(0.5f, 1, camera.farClipPlane)).y - transform.position.y) * 2;

		if (camera.ViewportToWorldPoint(new Vector3(0, 0.5f, camera.farClipPlane)).x < leftBorder.transform.position.x)
			transform.position = new Vector3(leftBorder.transform.position.x + widthOfScreen / 2, transform.position.y, transform.position.z);
		
		if (camera.ViewportToWorldPoint(new Vector3(1, 0.5f, camera.farClipPlane)).x > rightBorder.transform.position.x)
			transform.position = new Vector3(rightBorder.transform.position.x - widthOfScreen / 2, transform.position.y, transform.position.z);
		
		if (camera.ViewportToWorldPoint(new Vector3(0.5f, 1, camera.farClipPlane)).y > topBorder.transform.position.y)
			transform.position = new Vector3(transform.position.x, topBorder.transform.position.y - heightOfScreen / 2, transform.position.z);
		
		if (camera.ViewportToWorldPoint(new Vector3(0.5f, 0, camera.farClipPlane)).y < bottomBorder.transform.position.y)
			transform.position = new Vector3(transform.position.x, bottomBorder.transform.position.y + heightOfScreen / 2, transform.position.z);
	}

}
