using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
	
	[SerializeField]
	private GameObject target;

	public float speed;

	public GameObject Target {
		set { target = value; }
	}

	private void FixedUpdate() {
		if (!target)
			return;

		if (transform.position != target.transform.position) {
			Vector2 delta = ((target.transform.position - transform.position) * speed) + transform.position;

			if (transform.position != new Vector3(delta.x, delta.y, transform.position.z))
				transform.position = new Vector3(delta.x, delta.y, transform.position.z);
		}
	}
}
