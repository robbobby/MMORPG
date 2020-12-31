using Client;
using Client.Shared;
using Mirror;
using UnityEngine;

namespace Server.Monster {
    public class SMonsterState : NetworkBehaviour{
        private static readonly int Death = Animator.StringToHash("Death");
        private SMonsterFlags m_monsterFlags;
        private void Start() {
            m_monsterFlags = GetComponent<SMonsterFlags>();
        }
        [ClientRpc]
        public void RpcTriggerDeath() {
            Animator animator = this.GetComponent<Animator>();
            animator.SetTrigger(Death);
            GetComponent<Target>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            m_monsterFlags.IsAlive = false;
        }
    }
}