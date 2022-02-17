using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollideFood : MonoBehaviour
{
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        // Check if the collider is of tag "FoodMushroom"
        if (collider.gameObject.CompareTag("FoodMushroom"))
        {
            //mushroomCounter -= 1;
            Destroy(collider.gameObject);
            count = count + 1;
            Debug.Log("Mushroom/EXP Count is: " + count);
        }
    }
}
