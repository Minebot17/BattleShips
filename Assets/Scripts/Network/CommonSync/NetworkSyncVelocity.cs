using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("NetworkCustom/NetworkSyncVelocity")]
public class NetworkSyncVelocity : NetworkVectors {
	[SerializeField] float velocityLerpRate = 15;
	[SerializeField] float velocityThreshold = 0.1f;
	[SerializeField] bool fromLocalPlayer;
	[SyncVar] Vector3 lastVelocity;
	Rigidbody2D rigidbody2D;

	public Vector3 LastVelocity {
		set => lastVelocity = value;
	}

	void Start() {
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {
		if (fromLocalPlayer) {
			if (!hasAuthority) {
				InterpolateVelocity();
				return;
			}
		}
		else if (!isServer) {
			InterpolateVelocity();
			return;
		}

		if (IsVelocityChanged()) {
			if (!isServer)
				CmdSendVelocity(rigidbody2D.velocity);
			lastVelocity = rigidbody2D.velocity;
		}
	}

	bool IsVelocityChanged() {
		Vector2 velocity = new Vector2(X ? rigidbody2D.velocity.x : lastVelocity.x, Y ? rigidbody2D.velocity.y : lastVelocity.y);
		return Vector2.Distance(velocity, lastVelocity) > velocityThreshold;
	}

	void InterpolateVelocity() {
		Vector3 velocity = rigidbody2D.velocity;
		Vector3 newVelocity = Vector3.Lerp(rigidbody2D.velocity, lastVelocity, Time.deltaTime * velocityLerpRate);
		rigidbody2D.velocity = new Vector3(X ? newVelocity.x : velocity.x, Y ? newVelocity.y : velocity.y, Z ? newVelocity.z : velocity.z);
	}

	[Command(channel = Channels.DefaultUnreliable)]
	void CmdSendVelocity(Vector3 velocity) {
		lastVelocity = velocity;
	}

	[TargetRpc]
	public void TargetMarkChangeVelocity(NetworkConnection target, Vector3 velocity) {
		rigidbody2D.velocity = velocity;
		lastVelocity = velocity;
	}

	public override int GetNetworkChannel() {
		return Channels.DefaultUnreliable;
	}

	public override float GetNetworkSendInterval() {
		return 0.02f;
	}
}
