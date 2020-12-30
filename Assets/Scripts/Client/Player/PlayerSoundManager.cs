using Mirror;
using UnityEngine;

namespace Client.Player {
    public class SoundManager : NetworkBehaviour {
        [SerializeField] AudioClip footStep;
        [SerializeField] private AudioSource stepFoot;
        public void PlayFootstep() {
            stepFoot.PlayOneShot(footStep);
        }
    }
}