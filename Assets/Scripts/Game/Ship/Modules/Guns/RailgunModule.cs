using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
internal class RailgunModule : AbstractGunModule
{
    [SerializeField] private float lineTime = 1;
    [SerializeField] private int blocksThrough = 5;
    private LineRenderer lineRenderer;
    
    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    protected override void Shoot(Vector2 vec)
    {
        List<RaycastHit2D> hits = Physics2D.RaycastAll(transform.position, vec, 100).ToList();
        hits.RemoveAll(h => h.collider.transform.parent == null 
                            || h.collider.transform.parent.gameObject == damageInfo.OwnerShip.gameObject 
                            || (h.collider.gameObject.TryGetComponent(out ModuleHp moduleHp)
                                && moduleHp.transform.parent.parent.gameObject == damageInfo.OwnerShip.gameObject));
        
        if (blocksThrough != 0)
            hits = hits.Take(blocksThrough).ToList();
        
        hits.ForEach(h =>
        {
            if (h.collider.gameObject.TryGetComponent(out ModuleHp moduleHp)
                && NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, damageInfo))
            {
                if (h.collider.gameObject.TryGetComponent(out EffectModule effectModule))
                    effectModule.AddEffects(damageInfo.effects.Select(e => e.Create()));
                moduleHp.Damage(damageInfo);
            }
        });
        new RailgunClientMessage(damageInfo.OwnerShip, transform.parent.parent == null ? -1 : transform.parent.GetSiblingIndex(), hits.Last().point).SendToAllClient();
        StartCoroutine(RenderLine(hits.Last().point));
    }

    public IEnumerator RenderLine(Vector3 lastHit)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new [] { transform.position, lastHit});
        yield return new WaitForSeconds(lineTime);
        lineRenderer.positionCount = 0;
    }
        
}

