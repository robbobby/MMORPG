using Mirror;
using UnityEngine;

namespace Client.Player.Control {
    public class ControllerServerCall : NetworkBehaviour {
        private Server.Player.Movement m_sMovement;
        private void Start() {
            m_sMovement = GetComponent<Server.Player.Movement>();
        }
        [Command] private void CmdUpdateAnimator() {
            m_sMovement.RpcUpdateAnimator();
        }
        [Command] public void CmdValidateMouseButtonDown(Ray ray, Vector3 hitInfoPoint) {
            m_sMovement.RpcMoveToCursor(hitInfoPoint);
        }

        [Command] public void CmdValidateAttackMonster(Vector3 monsterPosition) {
            m_sMovement.RpcMoveToCursor(monsterPosition);
        }
        [Command] public void CmdValidateMouseButtonUp() {
            m_sMovement.RpcStopMovement();
        }
        [Command] public void CmdStopMovement() {
            m_sMovement.RpcStopMovement();
        }
    }
}