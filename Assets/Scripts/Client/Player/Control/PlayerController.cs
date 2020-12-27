using System.Runtime.CompilerServices;
using Client.Player.Combat;
using Client.Shared;
using Mirror;
using Server.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Player.Control {
    public class PlayerController : NetworkBehaviour {
        private bool m_hasTarget;
        private Camera m_camera;
        private Animator m_animator;
        private NavMeshAgent m_navMeshAgent;
        private ControllerServerCall m_serverCall;
        private CombatHandler m_combatHandler;
        private void Start() {
            m_combatHandler = GetComponent<CombatHandler>();
            m_serverCall = GetComponent<ControllerServerCall>();
            m_animator = GetComponent<Animator>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_camera = Camera.main;
            m_hasTarget = false;
        }
        private void Update() {
            HandleMouseClick();
            if (!isLocalPlayer) return;
            if (m_hasTarget) {
                m_hasTarget = m_combatHandler.ContinueAttack();
            }
            UpdateAnimator();
        }
        [Client] private void HandleMouseClick() {
            if (!isLocalPlayer) return;
            if (Input.GetMouseButton(0)) {
                CheckWhatCursorClicked();
            }
            if (Input.GetMouseButtonUp(0)) {
                m_serverCall.CmdValidateMouseButtonUp();
            }
        }
        [Client] private void CheckWhatCursorClicked() {
            SetHasTarget(false);
            Ray ray = GetMouseRay();
            if (HasClickedMonster()) return;
            SetMovePosition(ray);
        }
        [Client] private void SetMovePosition(Ray ray) {
            bool hasHit = Physics.Raycast(ray, out RaycastHit positionToMoveTo);
            if (hasHit) {
                m_serverCall.CmdValidateMouseButtonDown(ray, positionToMoveTo.point);
            }
        }
        [Client] private bool HasClickedMonster() {
            var hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits) {
                Target target = hit.transform.GetComponent<Target>();
                if (target == null) continue;
                print("Hello From If Its A Monster");
                SetHasTarget();
                m_combatHandler.SetTarget(hit.transform);
                return true;
            }
            return false;
        }
        [Client] private Ray GetMouseRay() => m_camera.ScreenPointToRay(Input.mousePosition);
        [Client] private void UpdateAnimator() {
            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            m_animator.SetFloat("forwardSpeed", speed);
        }
        [Client] private void SetHasTarget(bool isTargetting = true) {
            m_hasTarget = isTargetting;
        }

    }
}