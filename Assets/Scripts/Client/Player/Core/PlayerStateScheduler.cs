using Client.Player.Control;
using Mirror;
using UnityEngine;

namespace Client.Player.Core {
    public class StateScheduler : NetworkBehaviour{
        private IAction m_currentAction;
        [SerializeField] private bool m_isCurrentActionNull;
        [SerializeField] public bool IsMoving;
        [SerializeField] public bool IsMovingToAttack;
        [SerializeField] public bool IsAttacking;

        private void Start() {
            m_isCurrentActionNull = true;
        }
        public void StartAction(IAction action) {
            if ((m_currentAction == action) || m_isCurrentActionNull) {
                m_currentAction = action;
                m_isCurrentActionNull = false;
                return;
            }
            print("Reached here");
            m_currentAction.StopAction();
            m_currentAction = action;
        }

        public void StopAction(IAction action) {
            // if (IsMovingToAttack) return;
            print("Stopping action without new action"); 
            action.StopAction();
            m_isCurrentActionNull = true;
        }

        public void StopCurrentAction() {
            m_currentAction.StopAction();
            m_isCurrentActionNull = true;
        }
    }
}