using Mirror;
using UnityEngine;

namespace Client.Player.PlayerCamera {
    public class CameraPosition : NetworkBehaviour {
        [SerializeField] private Transform target;
        private Camera m_camera;

        private void Start() {
            m_camera = Camera.main;
        }
        private void Update() {
            if (!isLocalPlayer) return;
            UpdateCameraPosition();
        }
        private void UpdateCameraPosition() {
            Vector3 positionTranslate = transform.localPosition;
            if (m_camera is null) return;
            m_camera.transform.localPosition = new Vector3(positionTranslate.x,
                positionTranslate.y + 5f,
                positionTranslate.z - 8f);
            m_camera.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
        }
    }
}

