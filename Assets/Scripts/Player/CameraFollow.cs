using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    private void LateUpdate() {
        this.transform.position = target.position;
    }
}
