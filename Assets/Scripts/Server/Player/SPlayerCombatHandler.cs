using System.Diagnostics;
using Client;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using Server.Monster.Combat;
using Server.Monster;
using UnityEngine;
using UnityEngine.AI;

namespace Server.Player {
    internal class SPlayerCombatHandler : NetworkBehaviour {
        private IStateScheduler m_stateScheduler;
        public Animator animator;
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Death = Animator.StringToHash("Death");
        
        private void Start() {
            animator = GetComponent<Animator>();
            m_stateScheduler = animator.GetComponent<PlayerStateScheduler>();
        }
        [ClientRpc] public void RpcSetAttack(Transform target) {
            animator.SetTrigger(Attack);
            this.transform.LookAt(target);
        }
        public void TakeDamage(Target target) {
        }
        [Server] public void RpcTakeDamage(Target target, int i) { // TODO: This is going to be very heavy, maybe refactor
            SMonsterStats targetStats = target.GetComponent<SMonsterStats>();
            if (!targetStats.TakeDamage(i)) return;
            RpcTriggerDeath(target);
        }
        [ClientRpc] private void RpcTriggerDeath(Target target) {
            var targetAnimator = target.GetComponent<Animator>();
            targetAnimator.SetTrigger(Death);
            target.enabled = false;
            m_stateScheduler = target.GetComponent<IStateScheduler>();
            m_stateScheduler.IsAlive = false;
        }
    }
}

