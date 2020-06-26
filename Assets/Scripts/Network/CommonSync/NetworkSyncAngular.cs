
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class NetworkSyncAngular : NetworkBehaviour {
    [FormerlySerializedAs("velocityLerpRate")] [SerializeField]
    private float angularLerpRate = 15;
    [FormerlySerializedAs("velocityThreshold")] [SerializeField]
    private float angularThreshold = 0.1f;
    [SerializeField] private bool fromLocalPlayer;
    [SyncVar] private float lastAngular;
    private Rigidbody2D rigidbody2D;

    private void Start() {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if (fromLocalPlayer) {
            if (!hasAuthority) {
                InterpolateAngular();
                return;
            }
        }
        else if (!isServer) {
            InterpolateAngular();
            return;
        }

        if (IsAngularChanged()) {
            if (!isServer)
                CmdSendAngular(rigidbody2D.angularVelocity);
            lastAngular = rigidbody2D.angularVelocity;
        }
    }

    private bool IsAngularChanged() {
        return Mathf.Abs(rigidbody2D.angularVelocity - lastAngular) > angularThreshold;
    }

    private void InterpolateAngular() {
        rigidbody2D.angularVelocity = Mathf.Lerp(
        rigidbody2D.angularVelocity, lastAngular, Time.deltaTime * angularLerpRate);
    }

    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSendAngular(float angular) {
        lastAngular = angular;
    }

    public override int GetNetworkChannel() {
        return Channels.DefaultUnreliable;
    }

    public override float GetNetworkSendInterval() {
        return 0.02f;
    }
}