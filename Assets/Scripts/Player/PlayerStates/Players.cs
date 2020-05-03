using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

/// <summary>
/// Система состояний игроков. Позволяет присваивать параметры игрокам.
/// Каждый игрок имеет свой id, по который выдается сервером при заходе, и создается новый экземпляр PlayerStates под этим id.
/// Зная id можно получить любого игрока на сервере или на клиенте. Сервер может так же получить игрока по его NetworkConnection
/// Клиент может получить свой экземпляр напрямую
/// Параметры могут синхронизироваться между всеми клиентами при их изменении на сервере (sync у параметра)
/// Так же клиенты могут изменять параметры, которые им разрешено менять, чтобы они сихронизировались между сервером и клиентами (modification у параметра)
/// Не все параметры могут синхронизироваться и отправлять значение серверу! Надо смотреть на параметры sync и modification при их определении в конструкторе в классе состояния
/// Параметры разделены на состояния (или группы) (PlayerState). Это сделано для оптимизации хранения множества параметром, т.к. некоторые из них не всегда нужны
/// Id игрока или его connection хранится в PlayerStates, а не в каком-либо конкретном состоянии
/// Чтобы создать свое состояние, надо унаследовать класс PlayerState, и в ней объявить нужные параметры (регистрировать ничего не надо, все на рефлексии)
/// Группы будут создаваться на клиентах и сервере при их первом использовании
/// Так же можно создавать свой тип параметров наследуя StateValue
/// Кешировать полученные состояния не обязательно, так как всё делается через хеш таблицы (сложность O(1))
/// Пример получения своего ника: Players.GetClient().GetState<GameState>().Nick.Value
/// Так же параметры имеют эвент хандлер на изменения их значения, можно подписаться на него
/// </summary>
public static class Players {

    /// <summary>
    /// Эвент вызывается при запросе игрока состояний всех других игроков на сервере
    /// </summary>
    public static readonly EventHandler<PlayerRequestPlayersEvent> playerRequestPlayersEvent = new EventHandler<PlayerRequestPlayersEvent>();
    private static List<Type> loadedStates;
    private static readonly List<PlayerStates> players = new List<PlayerStates>();
    private static readonly Dictionary<int, PlayerStates> playerFromId = new Dictionary<int, PlayerStates>();
    private static readonly Dictionary<NetworkConnection, PlayerStates> playerFromConn = new Dictionary<NetworkConnection, PlayerStates>();
    private static int clientId;

    /// <summary>
    /// Список всех игроков
    /// </summary>
    public static List<PlayerStates> All => players;
    
    /// <summary>
    /// Id этого клиента (не использовать для получения PlayerStates клиента, смотри метод GetClient)
    /// </summary>
    public static int ClientId { set => clientId = value; }

    public static void Initialize() {
        if (loadedStates != null)
            return;
        
        loadedStates = Utils.FindChildesOfType(typeof(PlayerState)).ToList();
    }

    public static PlayerStates AddPlayer(NetworkConnection conn) {
        if (playerFromConn.ContainsKey(conn))
            return playerFromConn[conn];
        
        PlayerStates states = new PlayerStates(conn, Utils.rnd.Next());
        players.Add(states);
        playerFromId.Add(states.Id, states);
        playerFromConn.Add(conn, states);
        
        new AddPlayerStateClientMessage(states.Id).SendToAllClient(conn);
        return states;
    }
    
    public static PlayerStates AddPlayer(int id) {
        if (playerFromId.ContainsKey(id))
            return playerFromId[id];
        
        PlayerStates states = new PlayerStates(null, id);
        players.Add(states);
        playerFromId.Add(id, states);
        return states;
    }
    
    public static void RemovePlayer(NetworkConnection conn) {
        PlayerStates toRemove = GetPlayer(conn);
        players.Remove(toRemove);
        playerFromId.Remove(toRemove.Id);
        playerFromConn.Remove(conn);
        
        foreach (GeneralStateValue stateValue in toRemove.allValues.Values)
            stateValue.OnRemoveState();
        
        new RemovePlayerStateClientMessage(toRemove.Id).SendToAllClient(conn);
    }
    
    public static void RemovePlayer(int id) {
        PlayerStates toRemove = GetPlayer(id);
        players.Remove(toRemove);
        playerFromId.Remove(id);
    }
    
    public static PlayerStates GetPlayer(int id) {
        return playerFromId.Get(id);
    }
    
    public static PlayerStates GetPlayer(NetworkConnection conn) {
        return playerFromConn.Get(conn);
    }

    public static List<Type> GetLoadedStates() {
        return loadedStates;
    }

    /// <summary>
    /// Возвращает состояния текущего клиента
    /// </summary>
    public static PlayerStates GetClient() {
        return playerFromId[clientId];
    }

    public class PlayerRequestPlayersEvent : EventBase {

        public NetworkConnection Conn;

        public PlayerRequestPlayersEvent(NetworkConnection conn) : base(null, false) {
            Conn = conn;
        }
    }
}
