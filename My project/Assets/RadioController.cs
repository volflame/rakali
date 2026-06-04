using UnityEngine;

public class RadioController : MonoBehaviour
{
    public AudioClip[] tracks;
    public KeyCode switchKey = KeyCode.Q;
    public float interactDistance = 5f;  // How close the player needs to be

    private AudioSource _audioSource;
    private int _currentIndex = 0;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        Debug.Log("AudioSource found: " + _audioSource);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey) && PlayerIsLookingAtRadio())
            CycleTrack();
    }

    bool PlayerIsLookingAtRadio()
    {
        Transform cam = Camera.main.transform;

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, interactDistance))
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name);
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    void CycleTrack()
    {
        _currentIndex = (_currentIndex + 1) % 5;
        Debug.Log(_currentIndex);

        _audioSource.clip = tracks[_currentIndex];
        _audioSource.Play();
        Debug.Log($"Radio: Track {_currentIndex + 1} — {tracks[_currentIndex].name}");
    }
}