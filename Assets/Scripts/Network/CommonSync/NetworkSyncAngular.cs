
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class NetworkSyncAngular : NetworkBehaviour {
    [FormerlySerializedAs("velocityLerpRate")] [SerializeField]
    float angularLerpRate = 15;
    [FormerlySerializedAs("velocityThreshold")] [SerializeField]
    float angularThreshold = 0.1f;
    [SerializeField] bool fromLocalPlayer;
    [SyncVar] float lastAngular;
    Rigidbody2D rigidbody2D;

    void Start() {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
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

    bool IsAngularChanged() {
        return Mathf.Abs(rigidbody2D.angularVelocity - lastAngular) > angularThreshold;
    }

    void InterpolateAngular() {
        rigidbody2D.angularVelocity = Mathf.Lerp(
        rigidbody2D.angularVelocity, lastAngular, Time.deltaTime * angularLerpRate);
    }

    [Command(channel = Channels.DefaultUnreliable)]
    void CmdSendAngular(float angular) {
        lastAngular = angular;
    }

    public override int GetNetworkChannel() {
        return Channels.DefaultUnreliable;
    }

    public override float GetNetworkSendInterval() {
        return 0.02f;
    }
}