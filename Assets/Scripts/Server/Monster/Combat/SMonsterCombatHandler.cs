using System.Collections;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using Server.Player;
using Server.PlayerMonster;
using UnityEngine;
using UnityEngine.AI;

namespace Server.Monster.Combat {
    [RequireComponent(typeof(SMonsterFlags))]
    public class SMonsterCombatHandler : NetworkBehaviour {
        private static readonly int AttackAnimation = Animator.StringToHash("AttackAnimation");
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        private static readonly int Attack = Animator.StringToHash("attack");
        private IEnumerator m_checkIfShouldAttack;
        private NavMeshAgent m_navMeshAgent;
        private Vector3 lastKnowPosition;
        private GameObject[] m_players;
        private SMonsterStats m_stats;
        private SMonsterFlags m_flags;
        private SMovement m_movement;
        private Animator m_animator;
        private GameObject m_target;
        private const float RESET_MOVE_TIME = 0.2f;
        private const float ATTACKING_CHECK_TIME = 5f;
        private const float IDLE_CHECK_TIME = 2f;
        private bool m_startAttackCoroutine = false;
        private bool m_isAttackingAnimation = false;
        private bool m_isChecking;
        private float m_timer;
        private Vector3 m_startPosition;
        private bool m_guardAI;

        private void Start() {
            m_animator = GetComponent<Animator>();
            ServerStart();
        }
        [ServerCallback] private void ServerStart() {
            m_startPosition = transform.position;
            m_guardAI = true;
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_movement = GetComponent<SMovement>();
            m_flags = GetComponent<SMonsterFlags>();
            m_stats = GetComponent<SMonsterStats>();
            m_timer = 0f;
            m_stats.attackSpeed = 2;
            m_stats.attackRange = 2f;
            StartCoroutine(SearchForTargetCoroutine());
        }
        [ServerCallback] private void Update() {
            if(!m_flags.IsAlive) { return; }
            if(m_startAttackCoroutine) StartCoroutine(ContinueAttackCoRoutine());
            if(!m_flags.hasTarget || m_isChecking) return;
            m_timer += Time.deltaTime;
            if (!(m_timer > IDLE_CHECK_TIME)) return;
            SearchTarget();
            m_timer = 0f;
        }

        private void MoveBackToGuardPosition() {
            print("Not getting here");
            m_movement.RpcSetMove(m_startPosition);
        }

        [Server] private void SearchTarget() {
            m_players = GameObject.FindGameObjectsWithTag("Player");
            if (m_players.Length == 0) return;
            CheckIfShouldAttack();
        }
        [Server] private IEnumerator SearchForTargetCoroutine() {
            while (m_flags.IsAlive) {
                SearchTarget();
                if (m_flags.hasTarget) {
                    m_isChecking = false;
                    yield return new WaitForSeconds(ATTACKING_CHECK_TIME);
                }
                m_isChecking = true;
                m_flags.hasTarget = false;
                if (m_guardAI) MoveBackToGuardPosition();
                yield return new WaitForSeconds(IDLE_CHECK_TIME);
            }
        }
        [Server] private void CheckIfShouldAttack() {
            foreach (GameObject player in m_players) {
                var targetState = player.GetComponent<PlayerStateScheduler>();
                if (!IsObjectInSightDistance(player.transform.position) || !targetState.IsAlive) continue;
                print("IsObjectInSightDistance " + IsObjectInSightDistance(player.transform.position));
                m_target = player;
                m_flags.hasTarget = true;
                m_startAttackCoroutine = true;
                return;
            }
        }
        [Server] private bool IsObjectInSightDistance(Vector3 playerPosition) => Vector3.Distance(playerPosition, transform.position) < m_stats.sightDistance;
        [Server] private IEnumerator ContinueAttackCoRoutine() {
            var targetState = m_target.GetComponent<PlayerStateScheduler>();
            while (m_flags.hasTarget && m_flags.IsAlive && targetState.IsAlive) {
                m_startAttackCoroutine = false;
                if (InAttackRange(m_target.transform.position)) {
                    SAttackTarget();
                    m_movement.RpcStopMovement();
                    lastKnowPosition = m_target.transform.position;
                    yield return new WaitForSeconds(2f);
                } else {
                    IsObjectInSightDistance(lastKnowPosition);
                    m_movement.RpcSetMove(m_target.transform.position);
                    if (InAttackRange(lastKnowPosition) && !IsObjectInSightDistance(m_target.transform.position)) {
                        m_flags.hasTarget = false;
                        m_players = null;
                    }
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        [Server] private void SAttackTarget() {
            if (m_isAttackingAnimation) return;
            m_isAttackingAnimation = true;
            AttackTarget(m_target.transform.position);
        }
        [ClientRpc] private void AttackTarget(Vector3 targetPosition) {
            m_animator.SetFloat(AttackAnimation, UnityEngine.Random.Range(0.01f, 0.99f));
            this.transform.LookAt(targetPosition);
            m_animator.SetTrigger(Attack);
        }
        [Server] private bool InAttackRange(Vector3 playerPosition) =>
             Vector3.Distance(playerPosition, transform.position) <= 4f;
        // Animation Event functions
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