using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ForceOnStart : MonoBehaviour
{
    [SerializeField] private Vector3 force;
    [SerializeField] private Vector3 relativeForce;

    private void Start ()
    {
        var body = GetComponent<Rigidbody>();

        body.AddForce(force);
        body.AddRelativeForce(relativeForce);
	}
}
