using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    //if you copy from below this point, you are legally required to like the video
    public float throwForce = 500f; //force at which the object is thrown at
    public float pickUpRange = 5f; //how far the player can pickup the object from
    private float rotationSensitivity = 3.0f; //how fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private int LayerNumber; //layer index
    private int playerLayer;
    public CinemachineBrain brain;
    private MoneyManager moneyManager;
    public Animator animator;
    private float radius = 0.2f;
    public AudioSource whoosh;
    private RaycastHit hit;
    private Transform activeTooltip;
    public GameObject ingredientCard;
    //Reference to script which includes mouse movement of player (looking around)
    //we want to disable the player looking around when rotating the object
    //example below 
    //MouseLookScript mouseLookScript;
    void Start()
    {
        playerLayer = ~LayerMask.GetMask("Player", "Cauldron"); // ~ means ignore this layer

        LayerNumber = LayerMask.NameToLayer("holdLayer"); //if your holdLayer is named differently make sure to change this ""
        moneyManager = FindObjectOfType<MoneyManager>(); // anywhere in scene
        // animator = GetComponent<Animator>();
        //mouseLookScript = player.GetComponent<MouseLookScript>();
    }
    void Update()
    {
        // locking cursor on click into the game; TODO: Ryan make sure this doesn't bug out the build
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState == CursorLockMode.None && !moneyManager.shop.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        bool isHitting = Physics.SphereCast(
            transform.position,
            radius,
            transform.TransformDirection(Vector3.forward),
            out hit,
            pickUpRange,
            playerLayer
        );
        // if (Input.GetKey(KeyCode.E)) //change E to whichever key you want to press to pick up
        if (isHitting && Input.GetKeyDown(KeyCode.Mouse0) && heldObj == null)
        {
            if (hit.transform.gameObject.tag == "canPickUp")
            {
                //pass in object hit into the PickUpObject function
                PickUpObject(hit.transform.gameObject);
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && heldObj != null)
        {
            if (canDrop == true)
            {
                StopClipping(); //prevents object from clipping through walls
                DropObject();
            }
        }

        if (heldObj != null) //if player is holding object
        {
            MoveObject(); //keep object position at holdPos
            RotateObject();
            if (Input.GetKeyDown(KeyCode.E) && canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
            {
                StopClipping();
                ThrowObject();
            }

        }

        bool isTracking = Physics.Raycast(
            transform.position,
            transform.TransformDirection(Vector3.forward),
            out hit,
            pickUpRange,
            playerLayer
        );
        // Tooltip logic
        if (isTracking && hit.transform.CompareTag("canPickUp"))
        {
            Debug.Log("Hitting: " + hit.transform.gameObject.name);
            Transform tooltip = FindChildWithTag(hit.transform, "tooltip");
            Debug.Log("Tooltip found: " + (tooltip != null ? tooltip.name : "NULL"));
            if (tooltip != null)
            {
                activeTooltip = tooltip;
                activeTooltip.gameObject.SetActive(true);
                // if (Input.GetKeyDown(KeyCode.Q)) {

                // }
            }
        }
        else
        {
            if (activeTooltip != null)
            {
                activeTooltip.gameObject.SetActive(false);
                activeTooltip = null;
            }
        }

        if (isTracking && hit.transform.CompareTag("canPickUp"))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                IngredientStats so = hit.transform.GetComponent<IngredientPickup>().ingredient;
                if (so != null)
                {
                    Transform nameText = FindChildWithTag(ingredientCard.transform, "name");
                    Transform descText = FindChildWithTag(ingredientCard.transform, "description");
                    nameText.GetComponent<TextMeshProUGUI>().text = so.ingredientName;
                    descText.GetComponent<TextMeshProUGUI>().text = so.flavorText;
                    ingredientCard.SetActive(!ingredientCard.activeSelf);
                }
            }
        }
        else
        {
            ingredientCard.SetActive(false);
        }
        // else
        // {
        //     StartCoroutine(WaitAndRest());
        // }
    }
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            whoosh.Play();
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            // TO DO: RYAN THERE IS A BUG WHERE IF YOU SPAM CLICK IT'LL SPAM THE GRAB RELEASE EITHER YOU FIX THIS YOURSELF OR ASK SOMEONE/SOMETHING
            animator.ResetTrigger("Grab");
            animator.ResetTrigger("Release"); // same here
            animator.SetTrigger("Grab");
            animator.SetBool("isHolding", true); // ADD THIS
            animator.SetBool("isResting", false);
        }
    }
    void DropObject()
    {
        //re-enable collision with player
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0; //object assigned back to default layer
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //unparent object
        heldObj = null; //undefine game object
        animator.ResetTrigger("Grab");
        animator.ResetTrigger("Release");

        animator.SetBool("isHolding", false);
        animator.SetBool("isResting", false);

        animator.SetTrigger("Release");

        StartCoroutine(SetRestingAfterRelease());
    }
    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.localPosition = Vector3.zero;
        // heldObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
    }
    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))//hold R key to rotate, change this to whatever key you want
        {
            canDrop = false; //make sure throwing can't occur during rotating

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
            brain.enabled = false;
        }
        else
        {
            //re-enable player being able to look around
            canDrop = true;
            brain.enabled = true;
        }
    }
    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
        animator.ResetTrigger("Grab");
        animator.ResetTrigger("Release");

        animator.SetBool("isHolding", false);
        animator.SetBool("isResting", false);

        animator.SetTrigger("Release");

        StartCoroutine(SetRestingAfterRelease());
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange, playerLayer);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

    private IEnumerator WaitAndRest()
    {
        yield return new WaitForSeconds(1f);
        animator.ResetTrigger("Release");
        animator.Play("Arm Resting");
        yield return null;
    }

    IEnumerator SetRestingAfterRelease()
    {
        yield return new WaitForSeconds(0.5f); // adjust to animation length
        animator.SetBool("isResting", true);
    }

    Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag(tag))
                return child;
        }
        return null;
    }
}
