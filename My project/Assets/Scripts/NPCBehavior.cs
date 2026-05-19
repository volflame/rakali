using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.Callbacks;
using UnityEngine;
using Yarn.Unity;
using System.Linq;

public class NPCBehavior : MonoBehaviour
{
    [System.Serializable]
    public class PotionResponseEntry
    {
        public RecipeSO recipeSO;
        public PotionQuality quality;
    }

    public enum PotionQuality { Good, Neutral, Bad }
    public  DialogueRunner dialogueRunner;
    public List<PotionResponseEntry> potionResponses;
    public string endingNodeName = "BandLeader_Ending"; // set per NPC in Inspector

        void OnCollisionEnter(Collision collision)
    {
        
        Tags tags = collision.gameObject.GetComponent<Tags>();
        if (tags != null)
        {
            if (tags.HasTag("potion")) // doing a rly stupid tag workaround b/c unity doesn't do this natively bruh
            {
                // TextMeshPro text = GetComponentInChildren<TextMeshPro>();
                // text.text = "Wow, thanks dude, that really hits the spot!";
                PotionInstance potionInstance = collision.gameObject.GetComponent<PotionInstance>();
                string quality = GetQuality(potionInstance.recipeSO);
                dialogueRunner.VariableStorage.SetValue("$potion_quality", quality);
                Destroy(collision.gameObject);
                dialogueRunner.StartDialogue(endingNodeName);
                // Rigidbody rb = GetComponent<Rigidbody>();
                // rb.isKinematic = false;
                // rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
                // StartCoroutine(Dance());
            }
        }
    }

    private string GetQuality(RecipeSO so)
    {
        var entry = potionResponses.FirstOrDefault(p => p.recipeSO == so);
        if (entry == null) return "neutral";
        return entry.quality.ToString().ToLower();
    }

    /// <summary>
    /// Figure this one out. Supposed to make the sprite dance relative to player.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Dance()
    {
        for (int i = 0; i < 5; i++)
        {
            transform.Rotate(180f, 0f, 0f);
            yield return new WaitForSeconds(1.5f);
        }
        yield return null;
    }
}
