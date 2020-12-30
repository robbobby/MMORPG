using UnityEngine;

namespace Server.Monster {
    public class MonsterFlags : MonoBehaviour {
        public bool isAlive;
        public bool isAttacking;
        public void Awake() {
            isAlive = true;
            isAttacking = false;
        }
    }
}