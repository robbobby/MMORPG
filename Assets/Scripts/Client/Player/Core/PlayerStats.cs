using Mirror;
using UnityEngine;

namespace Client.Player.Core {
    public class PlayerStats : NetworkBehaviour {
        [SerializeField] public int attackSpeed;
        [SerializeField] public float attackRange;
        [SerializeField] public int maxHp;
        [SerializeField] public int maxMp;
        [SyncVar] public int currentHp;
        [SerializeField] public int currentMp;
        [SerializeField] public int minDc;
        [SerializeField] public int maxDc;
        [SerializeField] public int minMc;
        [SerializeField] public int maxMc;
        [SerializeField] public int minSc;
        [SerializeField] public int maxSc;
        [SerializeField] public int minAc;
        [SerializeField] public int maxAc;
        [SerializeField] public int minAmc;
        [SerializeField] public int maxAmc;
        [SerializeField] public int luck;
        [SerializeField] public int slow;
        [SerializeField] public int poison;
        [SerializeField] public int magicResistance;
        [SerializeField] public int poisonResistance;
        private void Start() {
            currentHp = 100;
        }
    }
}
