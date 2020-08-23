using UnityEngine;

public class ModuleGraphics : MonoBehaviour {
    [SerializeField] private Sprite[] moduleSprites;
    [SerializeField] private Sprite[] bloomSprites;
    [SerializeField] private GameObject bloom;
    [SerializeField] private string textureIndex = "0";
    [SerializeField] private ShipColor currentColor = ShipColor.PINK;

    public void SetColor(ShipColor color) {
        if (!bloom) {
            GameObject bloomInstance = new GameObject("Bloom");
            bloomInstance.transform.parent = transform;
            bloomInstance.transform.localPosition = new Vector3(0, 0, 0.01f);
            bloomInstance.AddComponent<SpriteRenderer>();
            bloom = bloomInstance;
        }
        
        bloom.GetComponent<SpriteRenderer>().sprite = bloomSprites[(int) color];
        GetComponent<SpriteRenderer>().sprite = moduleSprites[(int) color];
        currentColor = color;
    }
}