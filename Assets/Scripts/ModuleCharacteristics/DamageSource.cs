using UnityEngine.Networking;

public class DamageSource {

    private int amount;
    
    public int Amount => amount;

    public DamageSource(int amount) {
        this.amount = amount;
    }
}
