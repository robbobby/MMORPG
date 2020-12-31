using Client.Shared;
using Mirror;
using UnityEngine;

namespace Server.Monster.Combat {
    public class SMonsterStats : NetworkBehaviour {
        public int currentMp;
        public int maxMp;
        [SyncVar] public int currentHp = 100;
        public int maxHp;
        public float attackRange = 11f;
        public float sightDistance = 10f;
        public int minDc;
        public int maxDc;
        public int minMc;
        public int maxMc;
        public int minAc;
        public int maxAc;
        public int minAmc;
        public int maxAmc;
        public int attackSpeed = 2;
        public float sightRange = 10f;

        private void Start() {
        }
        
        public bool TakeDamage(int damage) {
            return (currentHp = Mathf.Max(currentHp -damage, 0)) == 0;
        }
        public void OnDrawGizmosSelected() {
            Vector3 position = transform.position;
            Gizmos.color = Color.blue;
            // Gizmos.DrawWireSphere(position, sightRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, attackRange);
        }
    }
}