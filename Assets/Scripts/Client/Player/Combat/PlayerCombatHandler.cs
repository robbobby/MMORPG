using Client.Player.Control;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using Server.Monster.Combat;
using UnityEngine;

namespace Client.Player.Combat {
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(StateScheduler))]
    [RequireComponent(typeof(ControllerServerCall))]
    public class CombatHandler : NetworkBehaviour, IAction {
        [SerializeField] private Transform m_target;
        private float m_attackRange = 2f;
        private ControllerServerCall m_serverCall;
        private float m_attackSpeed;
        private float m_timeSinceLastAttack;
        private StateScheduler m_stateScheduler;
        private Animator m_animator;
        private static readonly int Attack = Animator.StringToHash("attack");
        private Target m_targetObject;
        private bool m_isMoving;
        private bool m_isAttacking;
        private int m_targetHealth;
        private float DelayMovementAmount { get; set; }


        private void Start() {
            m_isMoving = false;
            m_animator = GetComponent<Animator>();
            m_stateScheduler = GetComponent<StateScheduler>();
            m_attackSpeed = 1f;
            m_timeSinceLastAttack = 5f;
            m_serverCall = GetComponent<ControllerServerCall>();
            DelayMovementAmount = 0.8f;
        }
        private void Update() {
            m_timeSinceLastAttack += Time.deltaTime;
        }
        public bool CanMove() {
            return m_timeSinceLastAttack > DelayMovementAmount;
        }
        [Client] public bool ContinueAttack() {
            if (!IsInRange()) {
                m_serverCall.CmdValidateAttackMonster(m_target.position);
                m_isMoving = true;
                return true;
            }
            if (m_stateScheduler.IsMovingToAttack) {
                m_stateScheduler.StartAction(this);
                m_stateScheduler.IsMovingToAttack = false;
            }
            if (m_targetObject.GetComponent<Stats>().hp == 0) {
                m_stateScheduler.StopAction(this);
                return false;
            }
            if ((!(m_timeSinceLastAttack > m_attackSpeed))) return true;
            if (!m_target) return true;
            m_serverCall.CmdValidateAttack(m_timeSinceLastAttack, m_attackSpeed, m_target);
            m_stateScheduler.IsAttacking = true;
            m_timeSinceLastAttack = 0;
            return true;
        }
        [Client] private bool IsInRange() =>
            Vector3.Distance(this.transform.position, m_target.transform.position) < m_attackRange;
        public void StopAction() {
            m_stateScheduler.IsAttacking = false;
            m_stateScheduler.IsMovingToAttack = false;
            m_target = null;
        }
        public void SetTarget(Transform hitTransform, Target targetObject) {
            m_targetObject = targetObject;
            m_target = targetObject.transform;
        }
        [Client] private void AE_Hit() { // Hit on animation call to server to say hit where transform is
            if(!isLocalPlayer) return;
            if(!m_targetObject) return;
            m_serverCall.CmdHitTarget(m_targetObject);
        }
    }
}
