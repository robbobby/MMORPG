using System.Runtime.CompilerServices;
using Mirror;
using Server.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Player.Control {
    public class PlayerController : NetworkBehaviour {
        private Camera m_camera;
        private Animator  m_animator;
        private NavMeshAgent m_navMeshAgent;
        private ControllerServerCall m_serverCall;
        private void Start() {
            m_serverCall = GetComponent<ControllerServerCall>();
            m_animator = GetComponent<Animator>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_camera = Camera.main;
        }
        private void Update() {
            HandleMouseClick();
            if (!isLocalPlayer) return;
            UpdateAnimator();
        }
        [Client] private void HandleMouseClick() {
            if (!isLocalPlayer) return;
            if(Input.GetMouseButton(0)) {
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                bool hasHit = Physics.Raycast(ray, out RaycastHit positionToMoveTo);
                if (hasHit) {
                    m_serverCall.CmdValidateMouseButtonDown(ray, positionToMoveTo.point);
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                m_serverCall.CmdValidateMouseButtonUp();
            }
        }
        [Client] private void UpdateAnimator() {
            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            m_animator.SetFloat("forwardSpeed", speed);
        }
    }
}