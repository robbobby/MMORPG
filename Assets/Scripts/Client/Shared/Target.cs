using Mirror;
using Shared;
using UnityEngine;

namespace Client.Shared {
    public class Target : NetworkBehaviour {
        [SerializeField] public TargetType targetType;
        private void Start() {
            targetType = TargetType.MONSTER;
        }
    }
}