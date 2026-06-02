using UnityEngine;
using Yarn.Unity;

public class HookInScriptForLinePresenter : MonoBehaviour
{
    [SerializeField] LineAdvancer? lineAdvancer;

    private void Update()
    {
        if (lineAdvancer == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            lineAdvancer.RequestNextLine();
        }
    }
}