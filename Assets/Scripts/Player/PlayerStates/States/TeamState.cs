public class TeamState : PlayerState {

    public IntStateValue TeamIndex;

    public TeamState(PlayerStates parent, bool isTest = false) : base(parent, isTest) {
        TeamIndex = new IntStateValue(this, "TeamIndex", -1, true, true);
    }
}
