using Client.Shared;
using Mirror;
using Server.Player;
using Server.PlayerMonster;
using UnityEngine;

namespace Client.Player.Control {
    [RequireComponent(typeof(SMovement))]
    [RequireComponent(typeof(SPlayerCombatHandler))]
    public class PlayerControllerServerCall : NetworkBehaviour {
        private SMovement m_sPlayerSPlayerMovement;
        private Server.Player.SPlayerCombatHandler m_sPlayerCombat;
        private void Start() {
            m_sPlayerSPlayerMovement = GetComponent<SMovement>();
            m_sPlayerCombat = GetComponent<SPlayerCombatHandler>();
        }
        [Command] public void CmdValidateMouseButtonDown(Ray ray, Vector3 hitInfoPoint) {
            m_sPlayerSPlayerMovement.RpcSetMove(hitInfoPoint);
        }
        [Command] public void CmdValidateAttackMonster(Vector3 monsterPosition) {
            m_sPlayerSPlayerMovement.RpcSetMove(monsterPosition);
        }
        [Command] public void CmdStopMovement() {
            m_sPlayerSPlayerMovement.RpcStopMovement();
        }
        [Command] public void CmdValidateAttack(float timeSinceLastAttack, float attackSpeed, Transform target) {
            if (timeSinceLastAttack > attackSpeed) m_sPlayerCombat.RpcSetAttack(target);
        }
        [Command] public void CmdHitTarget(Target target) {
            if (!target) return;
            // Calculate damage here, pass that damage into the m_sCombat.TakeDamage(damage)
            m_sPlayerCombat.RpcTakeDamage(target, 5);
        }
    }
}