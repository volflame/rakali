using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private int health = 5;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit by: " + collision.gameObject.name);
        if (health > 0)
        {
            health--;
            if (health == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
