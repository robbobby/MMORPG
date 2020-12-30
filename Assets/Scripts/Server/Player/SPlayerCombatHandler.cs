using System.Diagnostics;
using Client.Shared;
using Mirror;
using Server.Monster.Combat;
using Server.Monster;
using UnityEngine;

namespace Server.Player {
    internal class SCombatHandler : NetworkBehaviour {

        public Animator animator;
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Death = Animator.StringToHash("Death");
        
        private void Start() {
            animator = GetComponent<Animator>();
        }
        [ClientRpc] public void RpcSetAttack(Transform target) {
            animator.SetTrigger(Attack);
            this.transform.LookAt(target);
        }
        public void TakeDamage(Target target) {
        }
        [ClientRpc] public void RpcTakeDamage(Target target, int i) { // TODO: This is going to be very heavy, maybe refactor
            Stats targetStats = target.GetComponent<Stats>();
            if (!targetStats.TakeDamage(i)) return;
            target.GetComponent<SMonsterState>().RpcTriggerDeath();
            
        }
    }
}

