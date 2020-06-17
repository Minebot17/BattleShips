using UnityEngine;

public abstract class AbstractGunModule : MonoBehaviour {

    [SerializeField] int coolDown = 0;
    [SerializeField] float recoilForce = 0;

    private int timerCoolDown;

    Rigidbody2D rigidbody;
    private Transform forwardPointer;

    private void Start() {
        rigidbody = transform.GetComponentInParent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
    }

    public void TryShoot(Vector2 vec) {
        if (!NetworkManagerCustom.singleton.IsServer || timerCoolDown > 0)
            return;
        
        Shoot(vec);
        if (recoilForce != 0) {
            rigidbody.AddForce(vec * -recoilForce, ForceMode2D.Impulse);
            rigidbody.MarkServerChange();
            Vector2 a = Quaternion.Euler(0, 0, 14) * new Vector2();
        }

        timerCoolDown = coolDown;
    }

    public void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (timerCoolDown > 0)
            timerCoolDown--;
    }

    public abstract void Shoot(Vector2 vec);
}
