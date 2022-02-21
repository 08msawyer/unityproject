using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueSpeedBuff : MonoBehaviour
{
    private bool _activated;
    // If the statue collides with something it will be given a speed boost
    private void OnTriggerEnter(Collider other)
    {
        var animal = other.gameObject.GetComponent<AnimalMovementController>();
        if (animal != null)
        {
            if (_activated) return;
            animal.playerSpeed *= 1.5f;
            _activated = true;
        }
    }
}
