using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private int health = 3;
    public Mesh powder;

    void OnCollisionEnter(Collision collision)
    {
        Tags tags = collision.gameObject.GetComponent<Tags>();
        if (tags != null)
        {
            if (tags.HasTag("pestle")) // doing a rly stupid tag workaround b/c unity doesn't do this natively bruh
            {
                Debug.Log("Hit by: " + collision.gameObject.name);
                if (health > 0)
                {
                    health--;
                    if (health == 0)
                    {
                        MeshFilter ingMesh = gameObject.GetComponent<MeshFilter>();
                        ingMesh.mesh = powder;
                        GetComponent<Tags>().AddTag("mashed");
                    }
                }
            }
        }
    }
}
