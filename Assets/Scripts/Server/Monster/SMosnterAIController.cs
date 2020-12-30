using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Mirror;
using Server.Monster.Combat;
using UnityEngine;

namespace Server.Monster {
    [RequireComponent(typeof(MonsterFlags))]
    public class AIController : NetworkBehaviour {
        [SerializeField] float sightDistance = 5f;
        private GameObject[] m_players;
        private IEnumerator m_checkIfShouldAttack;
        private MonsterFlags m_flags;
        private bool m_isChecking;
        private float m_timer;
        private const float ATTACKING_CHECK_TIME = 5f;
        private const float IDLE_CHECK_TIME = 2f;
        [Server] private void Start() {
            m_flags = GetComponent<MonsterFlags>();
            StartCoroutine(CheckIfHasTargets());
            m_timer = 0f;
        }
        [Server] private void Update() {
            if (m_flags.isAttacking || m_isChecking) return;
            m_timer += Time.deltaTime;
            if (!(m_timer > IDLE_CHECK_TIME)) return;
            CheckIfHasTargetsNew();
            m_timer = 0f;
        }
        [Server] private void CheckIfHasTargetsNew() {
            m_players = GameObject.FindGameObjectsWithTag("Player");
            if (m_players.Length == 0) return;
            CheckIfShouldAttack();
        }
        [Server] private IEnumerator CheckIfHasTargets() {
            while (m_flags.isAlive) {
                CheckIfHasTargetsNew();
                if (m_flags.isAttacking) {
                    m_isChecking = false;
                    yield return new WaitForSeconds(ATTACKING_CHECK_TIME);
                }
                m_isChecking = true;
                yield return new WaitForSeconds(IDLE_CHECK_TIME);
            }
        }
        [Server] private void CheckIfShouldAttack() {
            foreach (GameObject player in m_players) {
                if (Vector3.Distance(player.transform.position, this.transform.position) < sightDistance) {
                    print(gameObject + " Should Attack");
                }
            }
        }
    }
}