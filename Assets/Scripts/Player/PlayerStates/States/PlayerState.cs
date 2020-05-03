public class PlayerState {

    private PlayerStates parent;
    private bool isTest;
    
    // Такой конструктор обязателен у детей!
    public PlayerState(PlayerStates parent, bool isTest) {
        this.parent = parent;
        this.isTest = isTest;
    }

    public PlayerStates GetParent() {
        return parent;
    }

    public bool IsTest() {
        return isTest;
    }
}
