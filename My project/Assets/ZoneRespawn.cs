using UnityEngine;
using System.Collections;

public class ZoneRespawn : MonoBehaviour
{
    [Header("Zone Settings")]
    public Transform zoneCenter;       // Center of the allowed zone
    public float zoneRadius = 0.1f;     // Radius of the zone
    public float timeOutsideLimit = 1f; // Seconds before respawn

    [Header("Spawn Settings")]
    public Vector3 spawnPosition;      // Where to respawn (defaults to zone center)
    public Quaternion spawnRotation;

    private Coroutine _outsideCoroutine;
    private bool _isOutside = false;


    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    void Update()
    {

        float distance = Vector3.Distance(transform.position, zoneCenter.position);
        bool currentlyOutside = distance > zoneRadius;

        if (currentlyOutside && !_isOutside)
        {
            _isOutside = true;
            _outsideCoroutine = StartCoroutine(RespawnCountdown());
        }
        else if (!currentlyOutside && _isOutside)
        {
            _isOutside = false;
            if (_outsideCoroutine != null)
                StopCoroutine(_outsideCoroutine);
        }
    }

    private IEnumerator RespawnCountdown()
    {
        yield return new WaitForSeconds(timeOutsideLimit);
        Respawn();
    }

    private void Respawn()
    {
        _isOutside = false;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.velocity = Vector3.zero;
    }

    // Optional: visualize zone in editor
    void OnDrawGizmosSelected()
    {
        if (zoneCenter == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(zoneCenter.position, zoneRadius);
    }
}