using Client.Player.Combat;
using Client.Player.Control;
using Client.Player.Core;
using Client.Player.Move;
using Client.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Player.Control {
    [RequireComponent(typeof(PlayerStateScheduler))]
    [RequireComponent(typeof(PlayerCombatHandler))]
    [RequireComponent(typeof(PlayerControllerServerCall))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerMouseEventHandler))]
    public class PlayerController : MonoBehaviour {
        private bool m_hasTarget;
        private Animator m_animator;
        private NavMeshAgent m_navMeshAgent;
        private PlayerCombatHandler m_playerCombatHandler;
        private PlayerMouseEventHandler m_mouseEventHandler;
        private PlayerStateScheduler m_stateScheduler;
        private PlayerMovement m_movement;

        private void Start() {
            m_playerCombatHandler = GetComponent<PlayerCombatHandler>();
            m_stateScheduler = GetComponent<PlayerStateScheduler>();
            m_animator = GetComponent<Animator>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_mouseEventHandler = GetComponent<PlayerMouseEventHandler>();
            m_movement = GetComponent<PlayerMovement>();
        }
        private void Update() {
            UpdateAnimator();
            if (IsDead()) return;
            m_mouseEventHandler.CheckInput();
            if (m_stateScheduler.hasTarget) { // Target is set on HandleMouseClick()
                m_stateScheduler.hasTarget = m_playerCombatHandler.ContinueAttack();
            }
        }

        private bool IsDead() {
            if (!m_stateScheduler.IsAlive) {
                m_stateScheduler.StopCurrentAction();
                return true;
            }
            return false;
        }

        [Client] private void UpdateAnimator() {
            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            m_animator.SetFloat("forwardSpeed", speed);
        }
    }
}
