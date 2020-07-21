public class CommonState : PlayerState {

    public NetworkIdentityStateValue ShipIdentity;
    public FloatStateValue CurrentHealth;
    public IntStateValue Score;
    public IntStateValue Kills;
    public IntStateValue AdditionalBuildPoints;
    public StringStateValue ShipJson;
    public StringStateValue Nick;
    public BoolStateValue Alive;
    public BoolStateValue IsShoot;
    public BoolStateValue WithShield;
    public BoolStateValue IsInvisible;

    public CommonState(Player parent, bool isTest) : base(parent, isTest) {
        ShipIdentity = new NetworkIdentityStateValue(this, "ShipIdentity", null, SyncType.ALL_SYNC);
        CurrentHealth = new FloatStateValue(this, "CurrentHp", 0f, SyncType.ALL_SYNC);
        Score = new IntStateValue(this, "Score", 0, SyncType.ALL_SYNC);
        Kills = new IntStateValue(this, "Kills", 0, SyncType.ALL_SYNC);
        AdditionalBuildPoints = new IntStateValue(this, "AdditionalBuildPoints", 0, SyncType.OWNER_SYNC);
        ShipJson = new StringStateValue(this, "ShipJson", Utils.CreateEmptyShip(), SyncType.ALL_SYNC, true);
        Nick = new StringStateValue(this, "Nick", "ip", SyncType.ALL_SYNC, true);
        Alive = new BoolStateValue(this, "Alive", true, SyncType.ALL_SYNC);
        IsShoot = new BoolStateValue(this, "IsShoot", false, SyncType.NOT_SYNC, true);
        WithShield = new BoolStateValue(this, "WithShield", true, SyncType.ALL_SYNC, true);
        IsInvisible = new BoolStateValue(this, "IsInvisible", false, SyncType.ALL_SYNC);
    }
}
