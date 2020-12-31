using System.Collections;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using Server.Player;
using Server.PlayerMonster;
using Shared;
using UnityEngine;
using UnityEngine.AI;

namespace Server.Monster.Combat {
    [RequireComponent(typeof(SMonsterFlags))]
    public class SMonsterCombatHandlerCopu : NetworkBehaviour {
        private Vector3 m_guardingPosition;
        private bool m_guardAI;
        private NavMeshAgent m_navMeshAgent;
        private SMovement m_movement;
        private SMonsterStats m_stats;
        private SMonsterFlags m_flags;
        private float m_timer;
        private GameObject[] m_players;
        private GameObject m_target;
        private bool m_startAttackCoroutine;
        private MonsterState MonsterState;
        private object m_lastKnowLocation;
        private bool m_isAttackingAnimation;

        private void Start() {
            m_guardingPosition = transform.position;
            m_guardAI = true;
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_movement = GetComponent<SMovement>();
            m_flags = GetComponent<SMonsterFlags>();
            m_stats = GetComponent<SMonsterStats>();
            m_timer = 0f;
            StartCoroutine(SearchForTargetCoroutine());
        }
        private IEnumerator SearchForTargetCoroutine() {
            while(m_flags.IsAlive) {
                SearchTarget();
                while(MonsterState == MonsterState.MovingToTarget) { // Move to target loop
                    m_lastKnowLocation = m_target.transform.position;
                    // Here... Need to set move to targets position
                    // Check is target is in range
                    // if not in range change to moving to last known location
                    // if in range set to attack
                }
                yield return new WaitForSeconds(2);
            }
        }
        private void SearchTarget() {
            m_players = GameObject.FindGameObjectsWithTag("Player");
            if (m_players.Length == 0) return;
            CheckIfShouldAttack();
        }
        private void CheckIfShouldAttack() {
            foreach (GameObject player in m_players) {
                var targetState = player.GetComponent<PlayerStateScheduler>();
                if (!targetState.IsAlive || !IsInSightDistance(player.transform.position)) continue;
                // Collecting targets here, returns after 1 target
                // Get all targets later and make new function to decide which to attack
                m_target = player;
                MonsterState = MonsterState.MovingToTarget;
                m_startAttackCoroutine = true;
                return;
            }
        }
        private bool IsInSightDistance(Vector3 playerPosition) =>
            Vector3.Distance(playerPosition, transform.position) < m_stats.sightDistance;
        [Server] private void AE_EndAnimation() {
            m_isAttackingAnimation = false;
        }
        [Server] private void AE_Hit() {
            SPlayerCombatHandler combatHandler = GetComponent<SPlayerCombatHandler>();
            Target target = m_target.GetComponent<Target>();
            combatHandler.RpcTakeDamage(target , 5);
        }
    }
}