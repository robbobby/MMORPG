using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Server.Player {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class SMovement : NetworkBehaviour {
        private NavMeshAgent m_navMeshAgent;
        private Animator m_animator;
        private void Start() {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_animator = GetComponent<Animator>();
        }
        [ClientRpc] public void RpcSetMove(Vector3 destination) {
            m_navMeshAgent.SetDestination(destination);
        }
        [ClientRpc] public void RpcStopMovement() {
            m_navMeshAgent.ResetPath();
            print("Stopping movement");
        }
        [ClientRpc] public void RpcUpdateAnimator() {
            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            m_animator.SetFloat("forwardSpeed", speed);
        }
    }
}
