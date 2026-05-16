using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.Callbacks;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Tags tags = collision.gameObject.GetComponent<Tags>();
        if (tags != null)
        {
            if (tags.HasTag("potion")) // doing a rly stupid tag workaround b/c unity doesn't do this natively bruh
            {
                TextMeshPro text = GetComponentInChildren<TextMeshPro>();
                text.text = "Wow, thanks dude, that really hits the spot!";
                // Rigidbody rb = GetComponent<Rigidbody>();
                // rb.isKinematic = false;
                // rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
                // StartCoroutine(Dance());
            }
        }
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
