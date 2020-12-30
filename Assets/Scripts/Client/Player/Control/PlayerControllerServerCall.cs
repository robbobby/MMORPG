using Client.Shared;
using Mirror;
using Server.Player;
using UnityEngine;

namespace Client.Player.Control {
    [RequireComponent(typeof(SMovement))]
    [RequireComponent(typeof(SCombatHandler))]
    public class ControllerServerCall : NetworkBehaviour {
        private Server.Player.SMovement m_sSMovement;
        private Server.Player.SCombatHandler m_sCombat;
        private void Start() {
            m_sSMovement = GetComponent<SMovement>();
            m_sCombat = GetComponent<SCombatHandler>();
        }
        [Command] private void CmdUpdateAnimator() {
            m_sSMovement.RpcUpdateAnimator();
        }
        [Command] public void CmdValidateMouseButtonDown(Ray ray, Vector3 hitInfoPoint) {
            m_sSMovement.RpcSetMove(hitInfoPoint);
        }
        [Command] public void CmdValidateAttackMonster(Vector3 monsterPosition) {
            m_sSMovement.RpcSetMove(monsterPosition);
        }
        [Command] public void CmdStopMovement() {
            m_sSMovement.RpcStopMovement();
        }
        [Command] public void CmdValidateAttack(float timeSinceLastAttack, float attackSpeed, Transform target) {
            if (timeSinceLastAttack > attackSpeed) {
                m_sCombat.RpcSetAttack(target);
            }
        }
        [Command] public void CmdHitTarget(Target target) {
            if (!target) return;
            // Calculate damage here, pass that damage into the m_sCombat.TakeDamage(damage)
            m_sCombat.RpcTakeDamage(target, 5);
        }
    }
}