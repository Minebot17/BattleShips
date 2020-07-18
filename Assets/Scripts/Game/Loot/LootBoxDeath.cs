using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LootBoxDeath : MonoBehaviour, IDeath {
    private bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead() || !NetworkManagerCustom.singleton.IsServer)
            return;

        isDead = true;
        Destroy(gameObject);
        NetworkServer.Destroy(transform.parent.gameObject);

        string editorModuleName = transform.parent.name.Split('_')[1];
        GameObject lootItem = Instantiate(Map.lootItemPrefab);
        lootItem.name = "LootItem_" + editorModuleName;
        lootItem.transform.position = transform.parent.position;
        NetworkServer.Spawn(lootItem);
    }

    public bool IsDead() {
        return isDead;
    }
}
