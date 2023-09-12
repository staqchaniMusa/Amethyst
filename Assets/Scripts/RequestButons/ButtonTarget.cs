using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ButtonTarget : MonoBehaviourPun
{
    // Start is called before the first frame update

    [Header("This is the object that will move")]
    public Transform target;

    [Header("This is the objective where it will move")]
    public Transform objective;
    public Transform objective2;

    float elapsed;
    [Header("Interactuable time")]
    public bool entered;
    public float timeToMove = 2.5f;


    void Start()
    {
        entered = false;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
     
                
    }

    //if the hand has entered, start counting time
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "handRight" || other.gameObject.tag == "handLeft")
        {
           
                entered = true;
        
        }
    }

    //if hand goes out, stop count
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "handRight"|| other.gameObject.tag == "handLeft")
        {
            
                entered = false;
        }
    }


    public void MoveTarget()
    {
        target.GetComponent<PhotonView>().RequestOwnership();
        
        StartCoroutine(SetNewPosition());
    }


    /// <summary>
    /// Coroutine for moving the button
    /// </summary>
    /// <returns></returns>
    IEnumerator SetNewPosition()
    {
        float elapsed2 = 0;
        Vector3 origin = target.transform.position;

        while (elapsed2 < timeToMove)
        {

            target.transform.position = Vector3.Lerp(origin,objective.position, elapsed2 / timeToMove);

            yield return new WaitForFixedUpdate();

            elapsed2 += Time.fixedDeltaTime;
        }

        yield return new WaitForSeconds(5);

        elapsed2 = 0;
        origin = target.transform.position;
        while (elapsed2 < timeToMove)
        {

            target.transform.position = Vector3.Lerp(origin, objective2.position, elapsed2 / timeToMove);

            yield return new WaitForFixedUpdate();

            elapsed2 += Time.fixedDeltaTime;
        }

    }


}
