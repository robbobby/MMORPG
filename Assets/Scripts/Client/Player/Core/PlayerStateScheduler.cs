using Client.Player.Control;
using Mirror;
using UnityEngine;

namespace Client.Player.Core {
    public class PlayerStateScheduler : MonoBehaviour, IStateScheduler {
        [SerializeField] private bool isCurrentActionNull;
        public bool IsAlive { get; set; }
        public bool hasTarget;
        public bool isMoving;
        public bool isMovingToAttack;
        public bool isAttacking;
        private IPlayerAction m_currentPlayerAction;

        private void Start() {
            isCurrentActionNull = true;
            IsAlive = true;
        }
        public void StartAction(IPlayerAction playerAction) {
            if ((m_currentPlayerAction == playerAction) || isCurrentActionNull) {
                m_currentPlayerAction = playerAction;
                isCurrentActionNull = false;
                return;
            }
            m_currentPlayerAction.StopAction();
            m_currentPlayerAction = playerAction;
        }
        public void StopAction(IPlayerAction playerAction) {
            playerAction.StopAction();
            isCurrentActionNull = true;
        }
        public void StopCurrentAction() {
            m_currentPlayerAction.StopAction();
            isCurrentActionNull = true;
        }
    }
}