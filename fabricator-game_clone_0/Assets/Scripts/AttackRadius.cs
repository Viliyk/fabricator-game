using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadius : MonoBehaviour
{
    [SerializeField]
    private SphereCollider radius;

    private void OnTriggerEnter(Collider other)
    {
        print("1");
    }
}
