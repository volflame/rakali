using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class IngredientBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private int health = 3;
    public Mesh powder;
    private DialogueRunner dialogueRunner;
    private static bool mashTutorialDone = false;

    void Start()
	{
		dialogueRunner = FindObjectOfType<DialogueRunner>();
	}
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
                    collision.gameObject.GetComponent<AudioSource>().Play();
                    health--;
                    if (health == 0)
                    {
                        MeshFilter ingMesh = gameObject.GetComponent<MeshFilter>();
                        ingMesh.mesh = powder;
                        GetComponent<Tags>().AddTag("mashed");

                        if (!mashTutorialDone && dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
						{
							mashTutorialDone = true;
							StartCoroutine(WaitForPestleDrop());
						}
                        
                    }
                }
            }
        }
    }
    IEnumerator WaitForPestleDrop()
    {
        PickUpScript pickUp = FindObjectOfType<PickUpScript>();
        
        // Wait until player is no longer holding the pestle
        while (pickUp != null && pickUp.heldObj != null && 
            pickUp.heldObj.GetComponent<Tags>() != null && 
            pickUp.heldObj.GetComponent<Tags>().HasTag("pestle"))
        {
            yield return null; // check every frame
        }

        // Also wait for any dialogue to finish
        while (dialogueRunner.IsDialogueRunning)
            yield return null;
        // RYAN THIS IS SO HARD CODED + CHOPPED FIX THIS LATER
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            dialogueRunner.StartDialogue("TutorialManager_PostMash");    
        }
    }
}
