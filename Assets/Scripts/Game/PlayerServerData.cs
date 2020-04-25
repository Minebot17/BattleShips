using UnityEngine.Networking;

public class PlayerServerData {
    public NetworkConnection conn;
    public NetworkIdentity shipIdentity;
    public int score;
    public int kills;
    public string shipJson;
    public bool alive;
    public bool isShoot;

    public override bool Equals(object obj) {
        return obj is PlayerServerData && ((PlayerServerData) obj).conn == conn;
    }

    public override int GetHashCode() {
        return conn.GetHashCode();
    }
}
