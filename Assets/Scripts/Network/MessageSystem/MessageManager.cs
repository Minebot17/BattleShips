
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class MessageManager {
    public static short StartIndex = 200;
    public static short LastIndex = StartIndex;
    public static readonly List<GameMessage> RegisteredMessages = new List<GameMessage>();
    public static Dictionary<Type, short> MessageIndexes;
    public static bool Inited;

    public static void Initialize() {
        if (Inited)
            return;

        IEnumerable<Type> messageTypes = Utils.FindChildesOfType(typeof(GameMessage));
        foreach (Type type in messageTypes) {
            try {
                RegisteredMessages.Add((GameMessage)type.GetConstructor(new Type[0]).Invoke(new object[0]));
            }
            catch (NullReferenceException e) {
                Debug.LogError(type + " dont have empty constructor");
                throw;
            }
        }

        MessageIndexes = new Dictionary<Type, short>(); // Не переносить в класс! Сломаешь инициализацию всех пакетов
        for (int i = 0; i < RegisteredMessages.Count; i++) {
            if (NetworkManager.singleton.client != null)
                NetworkManager.singleton.client.RegisterHandler(LastIndex, msg => RegisteredMessages[msg.msgType-StartIndex].OnClient(msg.reader));
            NetworkServer.RegisterHandler(LastIndex, msg => RegisteredMessages[msg.msgType-StartIndex].OnServer(msg.reader, msg.conn));
            MessageIndexes[RegisteredMessages[i].GetType()] = LastIndex;
            LastIndex++;
        }

        Inited = true;
    }

    public static void SendToServer(GameMessage message) {
        message.Writer.FinishMessage();
        NetworkManager.singleton.client.SendWriter(message.Writer, message.GetChannel());
    }
    
    public static void SendToClient(NetworkConnection conn, GameMessage message) {
        message.Writer.FinishMessage();
        conn.SendWriter(message.Writer, message.GetChannel());
    }
    
    public static void SendToAllClient(GameMessage message) {
        message.Writer.FinishMessage();
        foreach (NetworkConnection conn in NetworkServer.connections)
            conn?.SendWriter(message.Writer, message.GetChannel());
    }
}
