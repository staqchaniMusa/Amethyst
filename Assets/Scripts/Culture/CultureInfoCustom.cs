using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// This is used to prevent the problems between the "." and ","
/// </summary>
public class CultureInfoCustom : MonoBehaviour
{
    
    public static CultureInfoCustom instance;
    public CultureInfo ci;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //set culture info --> prevent problems between "," and "." for floats
        ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        ci.NumberFormat.NumberDecimalSeparator = ".";

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
