using Client.Player.Control;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using Server.Monster.Combat;
using UnityEngine;

namespace Client.Player.Combat {
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerStateScheduler))]
    [RequireComponent(typeof(PlayerControllerServerCall))]
    public class PlayerCombatHandler : MonoBehaviour, IPlayerAction {
        private static readonly int Attack = Animator.StringToHash("attack");
        private float m_timeSinceLastAttack = Mathf.Infinity;
        private PlayerStats m_playerStats;
        private int m_targetHealth;
        private PlayerStateScheduler m_playerStateScheduler;
        private PlayerControllerServerCall m_serverCall;
        private Target m_targetObject;
        private float DelayMovementAmount { get; set; }
        private void Start() {
            m_playerStats = GetComponent<PlayerStats>();
            m_playerStateScheduler = GetComponent<PlayerStateScheduler>();
            m_serverCall = GetComponent<PlayerControllerServerCall>();
            DelayMovementAmount = 0.8f;
        }
        private void Update() {
            m_playerStats.attackSpeed = 1;
            m_playerStats.attackRange = 2f;
            m_timeSinceLastAttack += Time.deltaTime;
        }
        public bool CanMove() {
            return m_timeSinceLastAttack > DelayMovementAmount;
        }
        public bool ContinueAttack() {
            if (!IsInRange()) {
                m_serverCall.CmdValidateAttackMonster(m_targetObject.transform.position);
                m_playerStateScheduler.isMoving = true;
                return true;
            }
            if (m_playerStateScheduler.isMovingToAttack) {
                m_playerStateScheduler.StartAction(this);
                m_playerStateScheduler.isMovingToAttack = false;
            }
            if (m_targetObject.GetComponent<SMonsterStats>().currentHp == 0) {
                m_playerStateScheduler.StopAction(this);
                return false;
            }
            if ((!(m_timeSinceLastAttack > m_playerStats.attackSpeed))) return true;
            // if (!m_targetObject.transform.position) return true;
            m_serverCall.CmdValidateAttack(m_timeSinceLastAttack, m_playerStats.attackSpeed, m_targetObject.transform);
            m_playerStateScheduler.isAttacking = true;
            m_timeSinceLastAttack = 0;
            return true;
        }
        private bool IsInRange() =>
            Vector3.Distance(this.transform.position, m_targetObject.transform.position) < m_playerStats.attackRange;
        public void SetTarget(Transform hitTransform, Target targetObject) {
            m_targetObject = targetObject;
        }
        // IPlayerAction Methods
        public void StopAction() {
            m_playerStateScheduler.isAttacking = false;
            m_playerStateScheduler.isMovingToAttack = false;
            m_targetObject = null;
        }
        public void StartAction() {
            m_playerStateScheduler.StartAction(this);
        }
        // Animation methods
        private void AE_Hit() { // Hit on animation call to server to say hit where transform is
            if(!m_targetObject) return;
            m_serverCall.CmdHitTarget(m_targetObject);
        }
    }
}
