public class CommonState : PlayerState {

    public NetworkIdentityStateValue ShipIdentity;
    public IntStateValue Score;
    public IntStateValue Kills;
    public StringStateValue ShipJson;
    public StringStateValue Nick;
    public BoolStateValue Alive;
    public BoolStateValue IsShoot;

    public CommonState(Player parent, bool isTest) : base(parent, isTest) {
        ShipIdentity = new NetworkIdentityStateValue(this, "ShipIdentity", null, true);
        Score = new IntStateValue(this, "Score", 0, true);
        Kills = new IntStateValue(this, "Kills", 0, true);
        ShipJson = new StringStateValue(this, "ShipJson", Utils.CreateEmptyShip(), true, true);
        Nick = new StringStateValue(this, "Nick", "ip", true, true);
        Alive = new BoolStateValue(this, "Alive", true, true);
        IsShoot = new BoolStateValue(this, "IsShoot", false, false, true);
    }
}
