using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Server.PlayerMonster {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class SMovement : NetworkBehaviour {
        private NavMeshAgent m_navMeshAgent;
        private void Start() {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }
        [ClientRpc] public void RpcSetMove(Vector3 destination) {
            m_navMeshAgent.SetDestination(destination);
        }
        [ClientRpc] public void RpcStopMovement() {
            m_navMeshAgent.ResetPath();
        }
    }
}
