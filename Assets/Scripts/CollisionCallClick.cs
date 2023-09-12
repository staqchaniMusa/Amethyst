using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollisionCallClick : MonoBehaviour
{
    //pointer event
    public PointerEventData pointer;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.tag == "hand" )
        {
            //create pointer event
            pointer = new PointerEventData(EventSystem.current);


            ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }

    }
}
