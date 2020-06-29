using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[MapElement]
public class MapLaser : NetworkBehaviour {

    [SerializeField] private Material lineMaterial;
    [SerializeField] private Transform forwardPointer;
    [MapElement] [SerializeField] private float lineWidth;
    [MapElement] [SerializeField] private int startAngle;
    [MapElement] [SerializeField] private int startPhase;
    [MapElement] [SerializeField] private bool startWithOff;
    [MapElement] [SerializeField] private int damage;
    [MapElement] [SerializeField] private int perTicks;
    [MapElement] [SerializeField] private int turnOnTime;
    [MapElement] [SerializeField] private int turnOffTime;
    
    private DamageInfo damageInfo;
    private GameObject lineObject;
    private LineRenderer lineRenderer;
    private PolygonCollider2D lineTrigger;
    private LayerMask rayCastMask;
    private LayerMask overlapMask;

    private int currentTimer;
    [SyncVar(hook = nameof(TurnLaser))] private bool isOn;
    private long currentTicks;

    protected void Start() {
        rayCastMask = LayerMask.GetMask("Default");
        overlapMask = LayerMask.GetMask("Modules");
        damageInfo = new DamageInfo(damage, GetComponent<NetworkIdentity>()) {
            effects = GetComponents<IEffectFabric>().ToList()
        };
        
        lineObject = new GameObject("LineObject");
        lineObject.layer = LayerMask.NameToLayer("Ammo");
        lineObject.transform.parent = transform;
        lineObject.transform.localPosition = new Vector3(0, 0, 0.1f);
        
        lineTrigger = lineObject.AddComponent<PolygonCollider2D>();
        lineTrigger.isTrigger = true;

        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPositions(new []{ 
            new Vector3(transform.position.x, transform.position.y, 0), 
            new Vector3(transform.position.x, transform.position.y, 0) 
        });
        isOn = !startWithOff;
        currentTimer = (isOn ? turnOnTime : turnOffTime) - startPhase;
        TurnLaser(isOn);
    }
    
    public void FixedUpdate() {
        currentTimer--;
        if (currentTimer < 0 && NetworkManagerCustom.singleton.IsServer && turnOffTime != 0) {
            isOn = !isOn;
            TurnLaser(isOn);
            currentTimer = isOn ? turnOnTime : turnOffTime;
        }

        if (isOn) {
            Vector2 direction = Quaternion.Euler(0, 0, transform.localEulerAngles.z + startAngle) * forwardPointer.localPosition.ToVector2();
            bool buffer = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100, rayCastMask);
            Physics2D.queriesHitTriggers = buffer;
            Vector2 endPoint = direction * 0.32f + hit.point;
            lineRenderer.SetPosition(1, endPoint);

            if (NetworkManagerCustom.singleton.IsServer) {
                lineTrigger.points = new[] {
                    new Vector2(-lineWidth/2f, 0),
                    new Vector2(lineWidth/2f, 0), 
                    new Vector2(lineWidth/2f, hit.distance),
                    new Vector2(-lineWidth/2f, hit.distance)
                };
                
                if (currentTicks % perTicks == 0) {
                    List<Collider2D> colliders = new List<Collider2D>();
                    Physics2D.OverlapCollider(lineTrigger, new ContactFilter2D { useLayerMask = true, layerMask = overlapMask, useTriggers = true }, colliders);
                    foreach (Collider2D col in colliders) {
                        if (col.gameObject.GetComponent<MapElementDeath>())
                            continue;

                        ModuleHp hp = col.gameObject.GetComponent<ModuleHp>();
                        if (hp)
                            hp.Damage(damageInfo);
                    }
                }

                currentTicks++;
            }
        }
    }

    private void TurnLaser(bool toOn) {
        lineRenderer.enabled = toOn;
        lineTrigger.enabled = toOn;
    }
}
