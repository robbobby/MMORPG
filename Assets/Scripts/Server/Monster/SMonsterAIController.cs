using System;
using System.Collections;
using Client.Player.Core;
using Client.Shared;
using Mirror;
using Server.Monster.Combat;
using Server.Player;
using Server.PlayerMonster;
using UnityEngine;
using UnityEngine.AI;

namespace Server.Monster {
    [RequireComponent(typeof(SMonsterFlags))]
    public class SMonsterAIController : NetworkBehaviour {
        private static readonly int AttackAnimation = Animator.StringToHash("AttackAnimation");
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        private static readonly int Attack = Animator.StringToHash("attack");
        private IEnumerator m_checkIfShouldAttack;
        private NavMeshAgent m_navMeshAgent;
        private GameObject[] m_players;
        private SMonsterFlags m_flags;
        private SMovement m_movement;
        private Animator m_animator;
        private GameObject m_target;
        private const float RESET_MOVE_TIME = 0.2f;
        private const float ATTACKING_CHECK_TIME = 5f;
        private const float IDLE_CHECK_TIME = 2f;
        private bool m_isChecking;

        private void Start() {
            m_animator = GetComponent<Animator>();
            ServerStart();
        }
        [ServerCallback] private void ServerStart() {
            m_flags = GetComponent<SMonsterFlags>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }
        [ServerCallback] private void Update() {
            if (!m_flags.IsAlive) { return; }
            UpdateAnimator();
        }
        private void UpdateAnimator() {
            Vector3 localVelocity = gameObject.transform.InverseTransformDirection(m_navMeshAgent.velocity);
            m_animator.SetFloat(ForwardSpeed, localVelocity.z);
        }
    }
}