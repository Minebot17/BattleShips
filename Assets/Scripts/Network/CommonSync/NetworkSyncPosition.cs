using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("NetworkCustom/NetworkSyncPosition")]
public class NetworkSyncPosition : NetworkVectors {
	[SerializeField] float posLerpRate = 15;
	[SerializeField] float posThreshold = 0.1f;
	[SerializeField] bool fromLocalPlayer;
	[SyncVar] Vector3 lastPosition;

	void Start() {
		if (isServer)
			lastPosition = transform.position;
	}

	void Update() {
		if (fromLocalPlayer) {
			if (hasAuthority)
				return;
		}
		else if (isServer)
			return;

		InterpolatePosition();
	}

	void FixedUpdate() {
		if (fromLocalPlayer) {
			if (!hasAuthority)
				return;
		}
		else if (!isServer)
			return;

		if (IsPositionChanged()) {
			if (!isServer)
				CmdSendPosition(transform.position);
			lastPosition = transform.position;
		}
	}

	bool IsPositionChanged() {
		Vector3 position = new Vector3(X ? transform.position.x : lastPosition.x, Y ? transform.position.y : lastPosition.y, Z ? transform.position.z : lastPosition.z);
		return Vector3.Distance(position, lastPosition) > posThreshold;
	}

	void InterpolatePosition() {
		Vector3 pos = transform.position;
		Vector3 newPos = Vector3.Lerp(transform.position, lastPosition, Time.deltaTime * posLerpRate);
		transform.position = new Vector3(X ? newPos.x : pos.x, Y ? newPos.y : pos.y, Z ? newPos.z : pos.z);
	}

	[Command(channel = Channels.DefaultUnreliable)]
	void CmdSendPosition(Vector3 pos) {
		lastPosition = pos;
	}

	public override int GetNetworkChannel() {
		return Channels.DefaultUnreliable;
	}

	public override float GetNetworkSendInterval() {
		return 0.02f;
	}
}
