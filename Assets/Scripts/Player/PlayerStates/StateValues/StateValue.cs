using System;
using System.Collections.Generic;
using UnityEngine.Networking;

public abstract class StateValue<T> : GeneralStateValue {

    private PlayerState parent;
    private string name;
    private T defaultValue;
    private T value;
    private bool sync;
    private bool modification;
    private int playerRequestPlayersEventId;
    
    /// <summary>
    /// Эвент, срабатывающий при изменении значения параметра
    /// </summary>
    public readonly EventHandler<OnChangeValueEvent> onChangeValueEvent = new EventHandler<OnChangeValueEvent>();
    
    public T Value {
        get => value;
        set {
            OnChangeValueEvent result = onChangeValueEvent.CallListners(new OnChangeValueEvent(this.value, value));
            if (result.IsCancel)
                return;
            
            this.value = result.NewValue;

            if (sync && NetworkManagerCustom.singleton.IsServer) {
                SyncStateValueClientMessage message = new SyncStateValueClientMessage(GetOwnerId(), GetName());
                Write(message.Writer);
                message.SendToAllClient();
            }

            if (modification && !NetworkManagerCustom.singleton.IsServer) {
                ModificationStateValueServerMessage message = new ModificationStateValueServerMessage(GetOwnerId(), GetName());
                Write(message.Writer);
                message.SendToAllClient();
            }
        }
    }
    
    protected StateValue(PlayerState parent, string name, T defaultValue, bool isTest, bool sync, bool modification) {
        this.parent = parent;
        this.name = name;
        this.defaultValue = defaultValue;
        this.sync = sync;
        this.modification = modification;
        value = defaultValue;

        if (isTest) 
            parent.GetParent().testValues.Add(name, this);
        else {
            parent.GetParent().allValues.Add(name, this);
            playerRequestPlayersEventId = Players.playerRequestPlayersEvent.SubcribeEvent(e => {
                if (this.defaultValue.Equals(value)) 
                    return;
                
                SyncStateValueClientMessage message = new SyncStateValueClientMessage(GetOwnerId(), GetName());
                Write(message.Writer);
                message.SendToClient(e.Conn);
            });
        }
    }

    protected int GetOwnerId() {
        return parent.GetParent().Id;
    }

    protected string GetName() {
        return name;
    }
    
    public void Reset() {
        value = defaultValue;
    }

    public override void Read(NetworkReader reader) {
        Value = ReadValue(reader);
    }

    public override void OnRemoveState() {
        Players.playerRequestPlayersEvent.UnSubcribeEvent(playerRequestPlayersEventId);
    }

    protected abstract T ReadValue(NetworkReader reader);

    public class OnChangeValueEvent : EventBase {

        public T OldValue;
        public T NewValue;
        
        public OnChangeValueEvent(T oldValue, T newValue) : base(null, true) {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
