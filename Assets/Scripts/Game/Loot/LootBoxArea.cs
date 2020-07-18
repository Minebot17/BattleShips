using System.Collections.Generic;
using UnityEngine;

public class LootBoxArea : MonoBehaviour {
    
    [Range(0f, 1f)]
    public float chanceForModule = 1f;
    public int maxModulesCount = 3;
    public Vector2 size = Vector2.one;
    public LootSpawnRules rule = LootSpawnRules.ALL;
    public EditorModule[] specificList = new EditorModule[0];

    private void Start() {
        for (int i = 0; i < maxModulesCount; i++)
            if (Utils.rnd.NextDouble() < chanceForModule) {
                List<EditorModule> modulesInBox = rule.FindTheseModules(specificList);
                Vector3 spawnPosition = (transform.position.ToVector2() - size / 2
                                         + new Vector2(
                                         (float) Utils.rnd.NextDouble() * size.x,
                                         (float) Utils.rnd.NextDouble() * size.y
                                         )).ToVector3(-0.05f);
                
                Map.SpawnLootBox(spawnPosition, modulesInBox[Utils.rnd.Next(modulesInBox.Count)].name);
            }
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + new Vector3(-size.x/2f, -size.y/2f, 0), transform.position + new Vector3(size.x/2f, -size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(size.x/2f, -size.y/2f, 0), transform.position + new Vector3(size.x/2f, size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(size.x/2f, size.y/2f, 0), transform.position + new Vector3(-size.x/2f, size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(-size.x/2f, size.y/2f, 0), transform.position + new Vector3(-size.x/2f, -size.y/2f, 0));
    }
}
