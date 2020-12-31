using Mirror;
using UnityEngine;

namespace Client.Player {
    public class PlayerSoundManager : MonoBehaviour {
        [SerializeField] AudioClip footStep;
        [SerializeField] private AudioSource stepFoot;
        public void PlayFootstep() {
            stepFoot.PlayOneShot(footStep);
        }
    }
}