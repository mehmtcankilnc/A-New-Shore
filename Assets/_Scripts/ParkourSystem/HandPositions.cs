using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPositions : MonoBehaviour
{
    [SerializeField] private Transform box;
    [SerializeField] private Vector3 leftHandOffset;
    [SerializeField] private Vector3 rightHandOffset;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    void Start()
    {
        leftHandTarget.position = box.position + box.transform.TransformDirection(leftHandOffset);
        rightHandTarget.position = box.position + box.transform.TransformDirection(rightHandOffset);
    }

}
