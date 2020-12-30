using Client.Player.Combat;
using Client.Player.Control;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Player.Control {
[RequireComponent(typeof(StateScheduler))]
[RequireComponent(typeof(CombatHandler))]
[RequireComponent(typeof(ControllerServerCall))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : NetworkBehaviour, IAction {
        private bool m_hasTarget;
        private Camera m_camera;
        private Animator m_animator;
        private NavMeshAgent m_navMeshAgent;
        private ControllerServerCall m_serverCall;
        private CombatHandler m_combatHandler;
        private StateScheduler m_stateScheduler;

        private void Start() {
            m_stateScheduler = GetComponent<StateScheduler>();
            m_combatHandler = GetComponent<CombatHandler>();
            m_serverCall = GetComponent<ControllerServerCall>();
            m_animator = GetComponent<Animator>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_camera = Camera.main;
        }
        private void Update() {
            UpdateAnimator();
            HandleMouseClick();
            if (!isLocalPlayer) return;
            if (m_hasTarget) { // Target is set on HandleMouseClick()
                m_hasTarget = m_combatHandler.ContinueAttack();
            }
        }
        [Client] private void HandleMouseClick() {      // Step 1 - Mouse Click
            if (!isLocalPlayer) return;
            if (Input.GetMouseButton(0)) {
                // Set IsAttacking false when player can move
                if ((m_stateScheduler.IsAttacking = !m_combatHandler.CanMove())) return;
                CheckWhatCursorClicked();
            }
            if (!Input.GetMouseButtonUp(0)) return;
            if (m_stateScheduler.IsMovingToAttack || m_stateScheduler.IsAttacking) return;
            m_stateScheduler.StopCurrentAction();
            m_hasTarget = false;
        }
        [Client] private void CheckWhatCursorClicked() { // Step 2 - Mouse Click
            SetHasTarget(false);
            Ray ray = GetMouseRay();
            if (HasClickedMonster()) return;
            SetMovePosition(ray);
        }
        [Client] private void SetMovePosition(Ray ray) { // Step 3 - Mouse Click
            bool hasHit = Physics.Raycast(ray, out RaycastHit positionToMoveTo);
            if (!hasHit) return;
            m_stateScheduler.StartAction(this);
            m_stateScheduler.IsMoving = true;
            m_serverCall.CmdValidateMouseButtonDown(ray, positionToMoveTo.point);
        }
        [Client] private bool HasClickedMonster() {
            var hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits) {
                Target target = hit.transform.GetComponent<Target>();
                if (!target) continue; // Checks if object has Target script
                m_stateScheduler.IsMovingToAttack = true;
                m_stateScheduler.StartAction(this);
                SetHasTarget();
                m_combatHandler.SetTarget(hit.transform, target);
                return true;
            }
            m_stateScheduler.IsMovingToAttack = false;
            return false;
        }
        [Client] private Ray GetMouseRay() => m_camera.ScreenPointToRay(Input.mousePosition);
        [Client] private void UpdateAnimator() {
            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            m_animator.SetFloat("forwardSpeed", speed);
        }
        [Client] private void SetHasTarget(bool isTargeting = true) {
            m_hasTarget = isTargeting;
        }

        public void StopAction() {
            m_stateScheduler.IsMoving = false;
            m_serverCall.CmdStopMovement();
        }
    }
}
