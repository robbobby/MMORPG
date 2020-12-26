using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Camera;

namespace Player {
    public class Movement : NetworkBehaviour {
        private Camera m_camera;
        private Animator m_animator;
        private NavMeshAgent m_navMeshAgent;
        private Vector3 destination;

        private void Start() {
            m_animator = GetComponent<Animator>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_camera = Camera.main;
        }
        private void Update() {
            if(!isLocalPlayer) return; // Exit if not client does not own object
            UpdateAnimator();
            UpdateCameraPosition();
            HandleMouseClick();
        }
        [Command] private void CmdValidateMouseButtonDown(Ray ray, Vector3 hitInfoPoint) {
            RpcMoveToCursor(hitInfoPoint);
        }
        [Command] private void CmdValidateMouseButtonUp() {
            RpcStopMovement();
        }
        [Client] private void HandleMouseClick() {
            if(!isLocalPlayer) return;
            if (Input.GetMouseButton(0)) {
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                bool hasHit = Physics.Raycast(ray, out RaycastHit positionToMoveTo);
                if (hasHit) {
                    CmdValidateMouseButtonDown(ray, positionToMoveTo.point);
                }
            }
            if (Input.GetMouseButtonUp(0))   CmdValidateMouseButtonUp();
        }
        [Client] private void UpdateCameraPosition() {
            Vector3 positionTranslate = transform.localPosition;
            if (m_camera is null) return;
            m_camera.transform.localPosition = new Vector3(positionTranslate.x,
                positionTranslate.y + 5f,
                positionTranslate.z - 8f);
            m_camera.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
        }
        [Client] private void UpdateAnimator() {
            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            m_animator.SetFloat("forwardSpeed", speed);
        }
        [ClientRpc] private void RpcStopMovement() { // Needs work, stops too abruptly
            m_navMeshAgent.ResetPath();
        }
        [ClientRpc]
        private void RpcMoveToCursor(Vector3 destination) {
            m_navMeshAgent.SetDestination(destination);
            }
        }
    }
