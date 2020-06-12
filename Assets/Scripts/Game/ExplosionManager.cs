using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Чтобы создать взрыв на карте, надо вызывать метод Explode у класса Explosion на стороне сервера
/// Пресеты готовых взрывов надо создавать тут в классе в качестве статических полей
/// Особенные взрывы можно создавать в месте вызова создав объект класса Explosion
/// </summary>
public class ExplosionManager : MonoBehaviour {
    public static ExplosionManager singleton;
    public static Explosion moduleSmallExplosion = new Explosion(0, 5, 1, 1.5f, 5);

    [SerializeField]
    private GameObject[] explosionPrefabs;

    public void Awake() {
        singleton = this;
    }

    public void SpawnExplosionPrefab(int type, float lifeTime, Vector2 position) {
        GameObject explosion = Instantiate(explosionPrefabs[type], position.ToVector3(-0.5f), new Quaternion());
        Destroy(explosion, lifeTime);
    }

    public class Explosion {
        private int prefabType;
        private int damage;
        private float lifeTime;
        private float radius;
        private float kickForce;

        /// <param name="prefabType">Указывает тип префаба по порядку из массива explosionPrefabs</param>
        /// <param name="damage">Урон от взрыва блокам. Линейно уменьшается от центра до 0</param>
        /// <param name="lifeTime">Время жизни префаба в секундах</param>
        /// <param name="radius">Радиус взрыва. Измеряется в метрах</param>
        /// <param name="kickForce">Сила отталкивания взрывом. Итоговый вектор расчитывается с учетом всех задевшихся блоков</param>
        public Explosion(int prefabType, int damage, float lifeTime, float radius, float kickForce) {
            this.prefabType = prefabType;
            this.damage = damage;
            this.lifeTime = lifeTime;
            this.radius = radius;
            this.kickForce = kickForce;
        }

        /// <summary>
        /// Вызывает взрыв. Вызывать только на сервере
        /// </summary>
        public void Explode(Vector2 position) {
            if (!NetworkManagerCustom.singleton.IsServer)
                return;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, Utils.shipCellsMask);
            Dictionary<NetworkIdentity, Vector2> kickVectors = new Dictionary<NetworkIdentity, Vector2>();
            foreach (Collider2D col in colliders) {
                NetworkIdentity parent = col.gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>();

                if (parent && !kickVectors.ContainsKey(parent) && parent.gameObject.tag.Equals("Player"))
                    kickVectors[parent] = new Vector2();

                Vector2 toAdd = col.gameObject.transform.position.ToVector2() - position;
                kickVectors[parent] = kickVectors[parent] + toAdd;

                ModuleHp hp = col.gameObject.GetComponentInChildren<ModuleHp>();
                if (hp)
                    hp.Damage(new DamageSource((int) Math.Ceiling(damage * (toAdd.magnitude/radius))));
            }

            foreach (KeyValuePair<NetworkIdentity, Vector2> pair in kickVectors) {
                Rigidbody2D rigidbody = pair.Key.gameObject.GetComponent<Rigidbody2D>();
                rigidbody.AddForce(pair.Value.normalized * kickForce, ForceMode2D.Impulse);
                rigidbody.MarkServerChange();
            }

            new CreateExplosionClientMessage(prefabType, lifeTime, position).SendToAllClient();
        }
    }
}
