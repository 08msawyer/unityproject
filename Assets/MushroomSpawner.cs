using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class MushroomSpawner : MonoBehaviour {
 
    public int mushrooms = 30;
 
    public GameObject mushroom;
    
    public int mushroomCounter = 0;
 
    void Update()
    {
        var worldBounds = GameObject.FindWithTag("World").GetComponentInChildren<Collider>().bounds;
        
        while(mushroomCounter<mushrooms)
        {
            Vector3 position = new Vector3(Random.Range(worldBounds.min.x, worldBounds.max.x), 0, Random.Range(worldBounds.min.z, worldBounds.max.z));
            //Do a raycast along Vector3.down -> if you hit something the result will be given to you in the "hit" variable
            //This raycast will only find results between +-100 units of your original"position" (ofc you can adjust the numbers as you like)
            if (Physics.Raycast (position + new Vector3 (0, worldBounds.max.y, 0), Vector3.down, out var hit, 200.0f)) {
                Instantiate (mushroom, hit.point, Quaternion.identity);
            } else {
                Debug.Log ("there seems to be no ground at this position");
            }

            mushroomCounter++;
        }
    }
}