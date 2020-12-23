using Mirror;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Camera;

namespace Player {
    public class Movement : NetworkBehaviour {
        [SerializeField] Transform target;
        private Camera m_camera;

        private void Start() {
            m_camera = Camera.main;
        }

        public override void OnStartLocalPlayer() {
            if (!main) return;
        }
        private void Update() {
            UpdateCameraPosition();

            if(!isLocalPlayer) { // Exit if not client does not own object
                return;
            }
            if (Input.GetMouseButton(0)) {
                MoveToCursor();
            }
            if (Input.GetMouseButtonUp(0)) {
                StopMovement();
            }
            UpdateAnimator();
        }

        private void UpdateCameraPosition() {
            Vector3 positionTranslate = transform.localPosition;
            if (!(m_camera is null)) {
                m_camera.transform.localPosition = new Vector3(positionTranslate.x,
                    positionTranslate.y + 5f,
                    positionTranslate.z - 8f);
                m_camera.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
            }
        }

        private void StopMovement() { // Needs work, stops too abruptly
            GetComponent<NavMeshAgent>().destination = GetComponent<NavMeshAgent>().nextPosition;
        }

        private void UpdateAnimator() {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
        private void MoveToCursor() {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            bool hasHit = Physics.Raycast(ray, out var positionToMoveTo);
            if (hasHit) {
                GetComponent<NavMeshAgent>().destination = positionToMoveTo.point;
            }
        }
    }
}
