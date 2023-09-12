using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// THIS IS THE SHOP SCRIPT
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("My Coins")]
    public int coins = 1000;

    public static ShopManager instance;

    [Header("UI")]
    public Text coinText;

    [Header("UI")]
    public float rotationSpeed = 15.5f;
    public Transform head;
    GameObject shop;
    PlayerHealth playerHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        shop = transform.GetChild(0).gameObject;
        playerHealth = transform.root.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "$" + coins;

        if (playerHealth.health<0)
        {
            shop.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //adapt to the forward direction of head
        Vector3 dirHead = new Vector3(head.forward.x, 0, head.forward.z);
        Vector3 dirThis = new Vector3(transform.forward.x, 0, transform.forward.z); ;

        float angle = (Vector3.SignedAngle(dirHead, dirThis, Vector3.up));
        if (Mathf.Abs(angle) > 20)
        {
            transform.rotation *= Quaternion.Euler(0, -rotationSpeed * Time.fixedDeltaTime * Mathf.Sign(angle), 0);
        }

    }

    public void EnableDisableShop()
    {
        shop.SetActive(!shop.activeInHierarchy);
    }
}
