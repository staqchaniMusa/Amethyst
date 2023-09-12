using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  called when the index_2_end enters the collider
/// </summary>
public class ContactButton : MonoBehaviour
{
    Coroutine corr;

    [Header("The callback of the button")]
    public MyEvent eventCall;

    public float buttonTime=0.25f;
    public Transform casing, origin;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(corr==null)
        {
            transform.position = origin.transform.position;
            transform.rotation = origin.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "index_2_end")
        {
            //Debug.Log("Trigger button");
            if (corr == null)
            {
                corr = StartCoroutine(TriggerButton_Co());
            }
        }
        
    }

    IEnumerator TriggerButton_Co()
    {
        float elapsed = 0;

        while (elapsed<buttonTime)
        {
            elapsed += Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(transform.position, casing.transform.position, elapsed / buttonTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, casing.transform.rotation, elapsed / buttonTime);

            yield return new WaitForFixedUpdate();
        }
        
        
        elapsed = 0;

        while (elapsed < buttonTime)
        {
            elapsed += Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(transform.position, origin.transform.position, elapsed / buttonTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, origin.transform.rotation, elapsed / buttonTime);

            yield return new WaitForFixedUpdate();
        }

        eventCall.Invoke();

        corr = null;
    }
}
