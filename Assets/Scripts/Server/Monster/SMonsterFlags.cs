using Client;
using Mirror;
using UnityEngine;

namespace Server.Monster {
    public class SMonsterFlags : NetworkBehaviour, IStateScheduler {
        public bool IsAlive { get; set; }
        public bool isAttacking;
        public bool hasTarget;
        public void Awake() {
            IsAlive = true;
            isAttacking = false;
            hasTarget = false;
        }

    }
}