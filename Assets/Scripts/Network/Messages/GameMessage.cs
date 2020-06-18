using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// children must have empty constructor
public abstract class GameMessage {
    
    public static short StartIndex = 99;
    public static short LastIndex = StartIndex;
    public static readonly List<GameMessage> RegisteredMessages = new List<GameMessage>();
    public static Dictionary<Type, short> MessageIndexes;
    
    public static bool Inited;
    public static bool RegisteredOnServer;
    public static bool RegisteredOnClient;

    public static void Initialize() {
        if (!Inited) {
            IEnumerable<Type> messageTypes = Utils.FindChildesOfType(typeof(GameMessage));
            foreach (Type type in messageTypes) {
                try {
                    RegisteredMessages.Add((GameMessage) type.GetConstructor(new Type[0]).Invoke(new object[0]));
                }
                catch (NullReferenceException e) {
                    Debug.LogError(type + " dont have empty constructor");
                    throw;
                }
            }

            MessageIndexes = new Dictionary<Type, short>(); // Не переносить в класс! Сломаешь инициализацию всех пакетов
            foreach (GameMessage message in RegisteredMessages) {
                MessageIndexes[message.GetType()] = LastIndex;
                LastIndex++;
            }

            Inited = true;
        }

        if (NetworkManagerCustom.singleton.IsServer && !RegisteredOnServer) {
            foreach (GameMessage message in RegisteredMessages) {
                short index = MessageIndexes[message.GetType()];
                NetworkServer.RegisterHandler(index, msg => RegisteredMessages[msg.msgType - StartIndex].OnServer(msg.reader, msg.conn));
            }

            RegisteredOnServer = true;
        }

        if (NetworkManager.singleton.client != null && !RegisteredOnClient) {
            foreach (GameMessage message in RegisteredMessages) {
                short index = MessageIndexes[message.GetType()];
                NetworkManager.singleton.client.RegisterHandler(index, msg => {
                    if (!NetworkManagerCustom.singleton.IsServer || message.WithServersClient())
                        RegisteredMessages[msg.msgType - StartIndex].OnClient(msg.reader);
                });
            }
            RegisteredOnClient = true;
        }
    }
    
    public NetworkWriter Writer = new NetworkWriter();

    protected GameMessage() {
        if (MessageIndexes == null)
            return;
            
        short messageId = MessageIndexes[GetType()];
        Writer.StartMessage(messageId);
    }

    public abstract void OnServer(NetworkReader reader, NetworkConnection conn);
    public abstract void OnClient(NetworkReader reader);

    public virtual int GetChannel() {
        return Channels.DefaultReliable;
    }

    public virtual bool WithServersClient() {
        return true;
    }

    public void SendToServer() {
        Writer.FinishMessage();
        SendWriter(() => NetworkManager.singleton.client.SendWriter(Writer, GetChannel()));
    }
    
    public void SendToClient(NetworkConnection conn) {
        Writer.FinishMessage();
        SendWriter(() => conn?.SendWriter(Writer, GetChannel()));
    }
    
    public void SendToAllClient() {
        Writer.FinishMessage();
        foreach (NetworkConnection conn in NetworkServer.connections)
            SendWriter(() => conn?.SendWriter(Writer, GetChannel()));
    }
    
    public void SendToAllClient(params NetworkConnection[] excepted) {
        Writer.FinishMessage();
        foreach (NetworkConnection conn in NetworkServer.connections)
            if (!excepted.Contains(conn))
                SendWriter(() => conn?.SendWriter(Writer, GetChannel()));
    }

    public void SendToAllClientExceptHost() {
        SendToAllClient(Players.HostConn);
    }

    public void SendWriter(Action sendFunc)
    {
        try
        {
            sendFunc();
        }
        catch
        {
            Task.Delay(5000).ContinueWith(t => sendFunc());
        }
    }
}
