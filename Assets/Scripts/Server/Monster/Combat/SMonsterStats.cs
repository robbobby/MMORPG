using Client.Shared;
using Mirror;
using UnityEngine;

namespace Server.Monster.Combat {
    public class Stats : NetworkBehaviour {
        [SyncVar] public int hp = 100;
        public bool TakeDamage(int damage) {
            return (hp = Mathf.Max(hp -damage, 0)) == 0;
        
        }
    }
}