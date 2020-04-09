using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
	
	[SerializeField]
	private Transform target;

	[SerializeField]
	private float speed;

	public Transform Target {
		set => target = value;
	}
	
	private void LateUpdate() {
		if (!target)
			return;

		transform.position = Vector3.Lerp(transform.position, target.transform.position - new Vector3(0, 0, 10), speed);
	}
}
