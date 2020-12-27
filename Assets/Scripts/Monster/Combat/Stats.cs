using Mirror;
using UnityEngine;

namespace Monster.Combat {
    public class Stats : NetworkBehaviour {
        [SerializeField] private int health = 100;
        public void TakeDamage(int damage) {
            if((health = Mathf.Max(health -damage, 0)) == 0 ) {
                // Death and drop
            }
        }
    }
}