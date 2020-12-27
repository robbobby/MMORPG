using Client.Player.Control;
using Mirror;
using UnityEngine;

namespace Client.Player.Combat {
    public class CombatHandler : NetworkBehaviour {
        [SerializeField] private Transform m_target;
        private float m_attackRange = 2f;
        private ControllerServerCall m_serverCall;
        private float m_attackSpeed;
        private float m_timeSinceLastAttack;
        private void Start() {
        m_attackSpeed = 1f;
        m_timeSinceLastAttack = 5f;
            m_serverCall = GetComponent<ControllerServerCall>();
        }
        [Client] public void SetTarget(Transform target) {
            print("Hello from SetTarget");
            m_target = target.transform;
        }
        [Client] public bool ContinueAttack() {
            print("Hello from ContinueAttack");
            if (!IsInRange()) {
                m_serverCall.CmdValidateAttackMonster(m_target.position);
                return true;
            }
            m_timeSinceLastAttack = 0;
            m_serverCall.CmdStopMovement();
            if (m_timeSinceLastAttack < m_attackSpeed) {
                return true;
            }
            return true;
        }
        [Client] private bool IsInRange() =>
            Vector3.Distance(this.transform.position, m_target.transform.position) < m_attackRange;
    }
}