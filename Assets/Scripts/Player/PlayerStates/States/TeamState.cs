public class TeamState : PlayerState {

    public IntStateValue TeamIndex;

    public TeamState(Player parent, bool isTest) : base(parent, isTest) {
        TeamIndex = new IntStateValue(this, "TeamIndex", -1, true, true);
    }
}
