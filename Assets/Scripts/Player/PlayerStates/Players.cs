using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

/// <summary>
/// Система состояний игроков. Позволяет присваивать параметры игрокам.
/// Каждый игрок имеет свой id, по который выдается сервером при заходе, и создается новый экземпляр Player под этим id.
/// Зная id можно получить любого игрока на сервере или на клиенте. Сервер может так же получить игрока по его NetworkConnection
/// Клиент может получить свой экземпляр напрямую
/// Параметры могут синхронизироваться между всеми клиентами при их изменении на сервере (sync у параметра)
/// Так же клиенты могут изменять параметры, которые им разрешено менять, чтобы они сихронизировались между сервером и клиентами (modification у параметра)
/// Не все параметры могут синхронизироваться и отправлять значение серверу! Надо смотреть на параметры sync и modification при их определении в конструкторе в классе состояния
/// Параметры разделены на состояния (или группы) (PlayerState). Это сделано для оптимизации хранения множества параметром, т.к. некоторые из них не всегда нужны
/// При первом доступе к состоянию, оно создается (без синхронизации создания). При удалении, оно удаляется на всех клиентах и сервере (даже если удаление было вызвано на клиенте)
/// Id игрока или его connection хранится в Player, а не в каком-либо конкретном состоянии
/// Чтобы создать свое состояние, надо унаследовать класс PlayerState, и в ней объявить нужные параметры (регистрировать ничего не надо, все на рефлексии)
/// Так же можно создавать свой тип параметров наследуя StateValue
/// Кешировать полученные состояния не обязательно, так как всё делается через хеш таблицы (сложность O(1))
/// Пример получения своего ника: Players.GetClient().GetState<GameState>().Nick.Value
/// Так же параметры имеют эвент хандлер на изменения их значения, можно подписаться на него
/// Имена всех параметров должны быть уникальны при их создании
/// </summary>
public static class Players {

    /// <summary>
    /// Эвент вызывается при запросе игрока состояний всех других игроков на сервере
    /// </summary>
    public static readonly EventHandler<ConnectionEvent> playerRequestPlayersEvent = new EventHandler<ConnectionEvent>();
    public static readonly EventHandler<PlayerEvent> playerAddedEvent = new EventHandler<PlayerEvent>();
    public static readonly EventHandler<PlayerEvent> playerRemovedEvent = new EventHandler<PlayerEvent>();
    private static List<Type> loadedStates;
    private static readonly List<Player> players = new List<Player>();
    private static readonly Dictionary<int, Player> playerFromId = new Dictionary<int, Player>();
    private static readonly Dictionary<NetworkConnection, Player> playerFromConn = new Dictionary<NetworkConnection, Player>();
    private static readonly Dictionary<NetworkIdentity, Player> playerFromIdentity = new Dictionary<NetworkIdentity, Player>();

    /// <summary>
    /// Список всех игроков
    /// </summary>
    public static List<Player> All => players;
    
    /// <summary>
    /// Id этого клиента (не использовать для получения PlayerStates клиента, смотри метод GetClient)
    /// </summary>
    public static int ClientId { set; get; }

    /// <summary>
    /// Id игрока хоста
    /// </summary>
    public static int HostId { set; get; }
    public static NetworkConnection HostConn { set; get; }

    public static IEnumerable<int> Ids => playerFromId.Keys;
    public static IEnumerable<NetworkConnection> Conns => playerFromConn.Keys;

    public static void Initialize() {
        if (loadedStates != null)
            return;
        
        loadedStates = Utils.FindChildesOfType(typeof(PlayerState)).ToList();
    }

    public static Player AddPlayer(NetworkConnection conn) {
        if (playerFromConn.ContainsKey(conn))
            return playerFromConn[conn];
        
        Player player = new Player(conn, Utils.rnd.Next());
        if (players.Count == 0) {
            HostId = player.Id;
            HostConn = conn;
        }

        players.Add(player);
        playerFromId.Add(player.Id, player);
        playerFromConn.Add(conn, player);
        
        new AddPlayerClientMessage(player.Id).SendToAllClient(conn);
        playerAddedEvent.CallListners(new PlayerEvent(player));
        return player;
    }
    
    public static Player AddPlayer(int id) {
        if (playerFromId.ContainsKey(id))
            return playerFromId[id];
        
        Player player = new Player(null, id);
        players.Add(player);
        playerFromId.Add(id, player);
        playerAddedEvent.CallListners(new PlayerEvent(player));
        return player;
    }
    
    public static void RemovePlayer(NetworkConnection conn) {
        Player toRemove = GetPlayer(conn);
        players.Remove(toRemove);
        playerFromId.Remove(toRemove.Id);
        playerFromConn.Remove(conn);
        
        playerRemovedEvent.CallListners(new PlayerEvent(toRemove));
        
        foreach (GeneralStateValue stateValue in toRemove.allValues.Values)
            stateValue.OnRemoveState();
        
        new RemovePlayerClientMessage(toRemove.Id).SendToAllClient(conn);
    }
    
    public static void RemovePlayer(int id) {
        Player toRemove = GetPlayer(id);
        players.Remove(toRemove);
        playerFromId.Remove(id);
        
        playerRemovedEvent.CallListners(new PlayerEvent(toRemove));
    }
    
    public static Player GetPlayer(int id) {
        return playerFromId.Get(id);
    }
    
    public static Player GetPlayer(NetworkConnection conn) {
        return playerFromConn.Get(conn);
    }
    
    /// <param name="identity">NetworkIdentity корабля игрока</param>
    public static Player GetPlayer(NetworkIdentity identity) {
        return playerFromIdentity.Get(identity);
    }

    public static List<Type> GetLoadedStates() {
        return loadedStates;
    }

    /// <summary>
    /// Возвращает состояния текущего клиента
    /// </summary>
    public static Player GetClient() {
        return playerFromId[ClientId];
    }

    /// <summary>
    /// Возвращает глобальное состояние
    /// </summary>
    public static GlobalState GetGlobal() {
        return playerFromId[HostId].GetState<GlobalState>();
    }

    /// <summary>
    /// Используется для получения указанного состояния всех игроков 
    /// </summary>
    public static IEnumerable<T> GetStates<T>() where T : PlayerState {
        return All.Select(s => s.GetState<T>());
    }

    /// <summary>
    /// Удаляет указанное состояние у всех игроков
    /// </summary>
    public static void RemoveStates(Type stateType, bool sync = true) {
        foreach (Player player in All) 
            player.RemoveState(stateType, false);

        if (!sync) 
            return;
        
        RemoveStatesMessage message = new RemoveStatesMessage(stateType.ToString());
        if (NetworkManagerCustom.singleton.IsServer)
            message.SendToAllClient();
        else
            message.SendToServer();
    }

    public static void BindIdentityToPlayer(Player player, NetworkIdentity identity) {
        try {
            NetworkIdentity toRemove = playerFromIdentity.First(pair => pair.Value.Id == player.Id).Key;
            playerFromIdentity.Remove(toRemove);
        }
        catch (InvalidOperationException e) { }
        
        playerFromIdentity[identity] = player;
    }

    public class ConnectionEvent : EventBase {

        public NetworkConnection Conn;

        public ConnectionEvent(NetworkConnection conn) : base(null, false) {
            Conn = conn;
        }
    }
    
    public class PlayerEvent : EventBase {

        public Player Player;

        public PlayerEvent(Player player) : base(null, false) {
            Player = player;
        }
    }
}
