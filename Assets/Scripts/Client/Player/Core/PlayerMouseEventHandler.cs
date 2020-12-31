using Client.Player.Combat;
using Client.Player.Control;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Player.Move {
    public class PlayerMouseEventHandler : MonoBehaviour {
        private Camera m_camera;
        private PlayerControllerServerCall m_serverCall;
        private PlayerCombatHandler m_combatHandler;
        private PlayerStateScheduler m_stateScheduler;
        private PlayerMovement m_movement;
        [Client] private void Start() {
            m_stateScheduler = GetComponent<PlayerStateScheduler>();
            m_combatHandler = GetComponent<PlayerCombatHandler>();
            m_serverCall = GetComponent<PlayerControllerServerCall>();
            m_camera = Camera.main;
            m_movement = GetComponent<PlayerMovement>();
        }
        [Client] private void CheckWhatCursorClicked() { // Step 2 - Mouse Click
            m_stateScheduler.hasTarget = false;
            Ray ray = GetMouseRay();
            if (HasClickedMonster()) return;
            SetMovePosition(ray);
        }
        [Client] private bool HasClickedMonster() {
            var hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits) {
                Target target = hit.transform.GetComponent<Target>();
                if (!target) continue; // Checks if object has Target script
                m_stateScheduler.isMovingToAttack = true;
                m_movement.StartAction();
                m_stateScheduler.hasTarget = true;
                m_combatHandler.SetTarget(hit.transform, target);
                return true;
            }
            m_stateScheduler.isMovingToAttack = false;
            return false;
        }
        [Client] private Ray GetMouseRay() => m_camera.ScreenPointToRay(Input.mousePosition);
        [Client] public void CheckInput() {      // Step 1 - Mouse Click
            if (Input.GetMouseButton(0)) {
                // Set IsAttacking false when player can move
                if ((m_stateScheduler.isAttacking = !m_combatHandler.CanMove())) return;
                CheckWhatCursorClicked();
            }
            if (!Input.GetMouseButtonUp(0)) return;
            if (m_stateScheduler.isMovingToAttack || m_stateScheduler.isAttacking) return;
            m_stateScheduler.StopCurrentAction();
            m_stateScheduler.hasTarget = false;
        }
        [Client] private void SetMovePosition(Ray ray) { // Step 3 - Mouse Click
            bool hasHit = Physics.Raycast(ray, out RaycastHit positionToMoveTo);
            if (!hasHit) return;
            m_movement.StartAction();
            m_stateScheduler.isMoving = true;
            m_serverCall.CmdValidateMouseButtonDown(ray, positionToMoveTo.point);
        }
    }
}