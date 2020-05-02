using UnityEngine.Networking;

public class PlayerServerData {
    public static EventHandler<ChangeNickEvent> changeNickEvent = new EventHandler<ChangeNickEvent>();
    
    private NetworkConnection conn;
    private NetworkIdentity shipIdentity;
    private int score;
    private int kills;
    private int id;
    private string shipJson;
    private string nick;
    private bool alive;
    private bool isShoot;

    public NetworkConnection Conn {
        get => conn;
        set => conn = value;
    }

    public NetworkIdentity ShipIdentity {
        get => shipIdentity;
        set => shipIdentity = value;
    }

    public int Score {
        get => score;
        set => score = value;
    }

    public int Kills {
        get => kills;
        set => kills = value;
    }

    public int Id {
        get => id;
        set => id = value;
    }

    public string ShipJson {
        get => shipJson;
        set => shipJson = value;
    }

    public string Nick {
        get => nick;
        set {
            nick = value;
            changeNickEvent.CallListners(new ChangeNickEvent(this));
        }
    }

    public bool Alive {
        get => alive;
        set => alive = value;
    }

    public bool IsShoot {
        get => isShoot;
        set => isShoot = value;
    }

    public PlayerServerData(string nick) {
        this.nick = nick;
    }

    public void Reset() {
        Score = 0;
        Kills = 0;
        ShipJson = Utils.CreateEmptyShip();
        Alive = true;
        IsShoot = false;
    }

    public override bool Equals(object obj) {
        return obj is PlayerServerData data && data.conn == conn;
    }

    public override int GetHashCode() {
        return conn.GetHashCode();
    }
    
    public class ChangeNickEvent : EventBase {
        public ChangeNickEvent(object sender) : base(sender, false) { }
    }
}
