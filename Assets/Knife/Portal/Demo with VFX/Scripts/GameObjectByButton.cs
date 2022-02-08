using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectByButton : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    [SerializeField] private GameObject target;

	private void Update ()
    {
        if (Input.GetKeyDown(key))
        {
            var state = target.activeSelf;
            state = !state;
            target.SetActive(state);
        }
	}
}
