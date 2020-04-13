﻿using System;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

	public static CameraFollower singleton;
	[SerializeField] private Transform target;
	[SerializeField] private Camera camera;
	[SerializeField] private BoxCollider2D leftBorder;
	[SerializeField] private BoxCollider2D rightBorder;
	[SerializeField] private BoxCollider2D topBorder;
	[SerializeField] private BoxCollider2D bottomBorder;

	[SerializeField]
	private float speed;

	public Transform Target {
		set => target = value;
	}

	private void Awake() {
		singleton = this;
	}

	private void LateUpdate() {
		if (!target)
			return;
		
		Vector2 newPosition = Vector2.Lerp(transform.position, target.position, speed * Time.deltaTime);

		if (transform.position != new Vector3(newPosition.x, newPosition.y, transform.position.z))
			transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

		float widthOfScreen = (camera.ViewportToWorldPoint(new Vector3(1, 0.5f, camera.farClipPlane)).x - transform.position.x) * 2;
		float heightOfScreen = (camera.ViewportToWorldPoint(new Vector3(0.5f, 1, camera.farClipPlane)).y - transform.position.y) * 2;
		bool leftBorderAchived = false;
		bool rightBorderAchived = false;
		bool topBorderAchived = false;
		bool bottomBorderAchived = false;

		if (camera.ViewportToWorldPoint(new Vector3(0, 0.5f, camera.farClipPlane)).x < leftBorder.transform.position.x)
		{
			leftBorderAchived = true;
			transform.position = new Vector3(leftBorder.transform.position.x + widthOfScreen / 2, transform.position.y, transform.position.z);
		}

		if (camera.ViewportToWorldPoint(new Vector3(1, 0.5f, camera.farClipPlane)).x > rightBorder.transform.position.x)
		{
			rightBorderAchived = true;
			transform.position = new Vector3(rightBorder.transform.position.x - widthOfScreen / 2, transform.position.y, transform.position.z);
		}

		if (camera.ViewportToWorldPoint(new Vector3(0.5f, 1, camera.farClipPlane)).y > topBorder.transform.position.y)
		{
			topBorderAchived = true;
			transform.position = new Vector3(transform.position.x, topBorder.transform.position.y - heightOfScreen / 2, transform.position.z);
		}

		if (camera.ViewportToWorldPoint(new Vector3(0.5f, 0, camera.farClipPlane)).y < bottomBorder.transform.position.y)
		{
			bottomBorderAchived = true;
			transform.position = new Vector3(transform.position.x, bottomBorder.transform.position.y + heightOfScreen / 2, transform.position.z);
		}

		if (leftBorderAchived && rightBorderAchived) transform.position = new Vector3(0, transform.position.y, transform.position.z);
		if (topBorderAchived && bottomBorderAchived) transform.position = new Vector3(transform.position.x, 0, transform.position.z);
	}
}
