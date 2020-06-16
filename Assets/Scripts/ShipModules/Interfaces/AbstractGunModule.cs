using UnityEngine;

public abstract class AbstractGunModule : MonoBehaviour {

    [SerializeField]
    int coolDown = 0;
    [SerializeField]
    int recoilForce  = 0;

    private int timerCoolDown;

    Rigidbody2D rigidbody;
    private Transform forwardPointer;

    private void Start()
    {
        rigidbody = transform.GetComponentInParent<Rigidbody2D>();
        forwardPointer = transform.parent.parent.Find("ForwardPointer");
    }

    public void TryShoot(Vector2 vec)
    {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (timerCoolDown <= 0)
        {
            Shoot(vec);
            rigidbody.AddForce((forwardPointer.parent.position - forwardPointer.position).ToVector2() * recoilForce, ForceMode2D.Force);
            timerCoolDown = coolDown;
        }    
    }

    public void FixedUpdate()
    {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (timerCoolDown > 0)
            timerCoolDown--;
    }

    abstract protected void Shoot(Vector2 vec);
}
