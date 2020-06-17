using System;
using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("NetworkCustom/NetworkSyncRotation")]
public class NetworkSyncRotation : NetworkVectors{
	[SerializeField] float rotLerpRate = 15;
	[SerializeField] float rotThreshold = 0.1f;
	[SerializeField] bool fromLocalPlayer;
	[SyncVar] Vector3 lastRotation;

	void Start() {
		if (isServer)
			lastRotation = transform.localEulerAngles;
	}

	void Update() {
		if (fromLocalPlayer) {
			if (hasAuthority)
				return;
		}
		else if (isServer)
			return;

		InterpolateRotation();
	}

	void FixedUpdate() {
		if (fromLocalPlayer) {
			if (!hasAuthority)
				return;
		}
		else if (!isServer)
			return;

		if (IsRotationChanged()) {
			if (!isServer)
				CmdSendRotation(transform.localEulerAngles);
			lastRotation = transform.localEulerAngles;
		}
	}

	bool IsRotationChanged() {
		Vector3 rotation = new Vector3(X ? transform.localEulerAngles.x : lastRotation.x, Y ? transform.localEulerAngles.y : lastRotation.y, Z ? transform.localEulerAngles.z : lastRotation.z);
		return Vector3.Distance(rotation, lastRotation) > rotThreshold;
		return true;
	}

	void InterpolateRotation() {
		Vector3 rot = transform.localEulerAngles;
		Vector3 newRot = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(lastRotation), Time.deltaTime * rotLerpRate).eulerAngles;
		transform.localEulerAngles = new Vector3(X ? newRot.x : rot.x, Y ? newRot.y : rot.y, Z ? newRot.z : rot.z);
		transform.localEulerAngles = lastRotation;
	}

	[Command(channel = Channels.DefaultUnreliable)]
	void CmdSendRotation(Vector3 rot) {
		lastRotation = rot;
	}

	public override int GetNetworkChannel() {
		return Channels.DefaultUnreliable;
	}

	public override float GetNetworkSendInterval() {
		return 0.05f;
	}
}

