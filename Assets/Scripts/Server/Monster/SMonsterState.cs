using Client.Shared;
using Mirror;
using UnityEngine;

namespace Server.Monster {
    public class SMonsterState : NetworkBehaviour {
        private static readonly int Death = Animator.StringToHash("Death");
        private MonsterFlags m_flags;
        private void Start() {
            m_flags = GetComponent<MonsterFlags>();
        }
        [ClientRpc]
        public void RpcTriggerDeath() {
            Animator animator = this.GetComponent<Animator>();
            animator.SetTrigger(Death);
            this.GetComponent<Target>().enabled = false;
            this.GetComponent<BoxCollider>().enabled = false;
            m_flags.isAlive = false;
        }
    }
}