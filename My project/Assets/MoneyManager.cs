using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Linq;

public class MoneyManager : MonoBehaviour
{
    public VisualElement ui;
    public VisualElement shop;
    public Label coinAmt;
    public Button buyIngOne;
    public Button buyIngTwo;
    public Button buyIngThree;
    public int coins = 100;
    public Transform playerLoc;
    // Start is called before the first frame update
    [System.Serializable]
    public class IngredientData
    {
        public string id;
        public string name;
        public int cost;
        public GameObject prefab;
    }

    public List<IngredientData> ingredients; // assign in Inspector
    private Dictionary<string, IngredientData> ingredientDict;
    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        ingredientDict = ingredients.ToDictionary(i => i.id);
    }

    private void OnEnable()
    {
        shop = ui.Q<VisualElement>("Shop");

        coinAmt = ui.Q<Label>("Coins");
        coinAmt.text = "Coins: " + coins;

        buyIngOne = ui.Q<Button>("Ingredient1");
        buyIngOne.clicked += () => buyIng("1");

        buyIngTwo = ui.Q<Button>("Ingredient2");
        buyIngTwo.clicked += () => buyIng("2");

        buyIngThree = ui.Q<Button>("Ingredient3");
        buyIngThree.clicked += () => buyIng("3");
    }

    private void buyIng(string id)
    {
        IngredientData ingData = ingredientDict[id];
        if (ingData != null)
        {
            if (ingData.cost <= coins)
            {
                coins -= ingData.cost;
                GameObject ingPrefab = Instantiate(ingData.prefab, playerLoc.position + Vector3.forward * 2f + Vector3.up * 10f, Quaternion.identity);
                Rigidbody ingPrefabRB = ingPrefab.GetComponent<Rigidbody>();
                StartCoroutine(SetKinematic(ingPrefabRB));
                coinAmt.text = "Coins: " + coins;
            }
            else
            {
                Debug.Log("Insufficient Funds!");
            }
        }
    }

    void Update()
    {
        // locking cursor on click into the game; TODO: Ryan make sure this doesn't bug out the build
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (shop.visible)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                shop.visible = !shop.visible; 
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                shop.visible = !shop.visible;    
            }
        }
    }
    public IEnumerator SetKinematic(Rigidbody rb)
    {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = true;
        yield return null;

    }
    }

