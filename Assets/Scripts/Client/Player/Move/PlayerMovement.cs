using Client.Player.Control;
using Client.Player.Core;
using UnityEngine;

namespace Client.Player.Move {
    public class PlayerMovement : MonoBehaviour, IPlayerAction {
        private PlayerControllerServerCall m_serverCall;
        private PlayerStateScheduler m_playerStateScheduler;
        private void Start() {
            m_playerStateScheduler = GetComponent<PlayerStateScheduler>();
            m_serverCall = GetComponent<PlayerControllerServerCall>();
        }
        public void StopAction() {
            m_playerStateScheduler.isMoving = false;
            m_serverCall.CmdStopMovement();
        }
        public void StartAction() {
            m_playerStateScheduler.StartAction(this);
        }
    }
}