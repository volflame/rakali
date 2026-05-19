using UnityEngine;
using Yarn.Unity;

public class CursorManager : MonoBehaviour {
    [SerializeField] DialogueRunner dialogueRunner;
    
    bool dialogueActive = false;

    void Start() {
        dialogueRunner.onDialogueStart.AddListener(OnDialogueStart);
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
    }

    void OnDialogueStart() {
        dialogueActive = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnDialogueComplete() {
        dialogueActive = false;
    }

    void Update() {
        if (dialogueActive) {
            // Force cursor on — overrides anything else trying to hide it
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        // Your existing cursor-hiding logic here
        if (Input.GetMouseButtonDown(0)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void OnDestroy() {
        dialogueRunner.onDialogueStart.RemoveListener(OnDialogueStart);
        dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
    }
}