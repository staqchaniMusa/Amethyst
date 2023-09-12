using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputModule : BaseInputModule
{
    //singleton
    public static VRInputModule instance;

    [Header("Camera to use in order to create the input data")]
    public Camera cam;

    [Header("Object and position selected (readable)")]
    public GameObject currentObject;
    public Vector3 currentObjectRaycast;

    [Header("The data from the pointer (readable)")]
    public PointerEventData data;

    [Header("Debug the object that was hit")]
    public RaycastHit hit;
    public bool debugRaycast;

    [Header("Used to display line and pointer")]
    public bool showRenders = false;

    //the event system (set to current in script)
    EventSystem evsys;


    // Start is called before the first frame update
    //we are overriding the base input module
    protected override void Awake()
    {
        base.Awake();
        instance = this;

        //current event system
        evsys = EventSystem.current;

        //line renderer
        GetComponent<LineRenderer>().enabled = false;

        //create new pointer event data
        data = new PointerEventData(evsys);
   
    }

    // Update is called once per frame
    public override void Process()
    {
        //reset the data
        data.Reset();

        //get the middle of the camera pixel position
        data.position = new Vector2(cam.pixelWidth/2,cam.pixelHeight/2);

        //raycast UI
        evsys.RaycastAll(data,m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        currentObject = data.pointerCurrentRaycast.gameObject;

        //if raycast to UI is null, generate the custom raycast with physics
        if (currentObject==null)
        {
            //mesh/box/other type of collider (not UI)
            hit = CreateRayCast();
            if (hit.collider != null)
            {
                //select current object in the data
                currentObject = hit.collider.gameObject;
                evsys.SetSelectedGameObject(currentObject);

                data.pointerCurrentRaycast = new RaycastResult()
                {
                    gameObject = currentObject,
                    worldPosition = hit.point

                };

                currentObjectRaycast = hit.point;
                //Debug.Log(eventsys.currentSelectedGameObject);
            }
        }

        //show the hit gameobject if selected
        if(debugRaycast)
        {
            //Debug.Log(data.pointerCurrentRaycast.gameObject);
        }
        
        // add additional parameters (if needed)
        data.pointerPressRaycast = data.pointerCurrentRaycast;
        data.pressPosition = data.position;
        data.clickCount = 1;
        data.button = PointerEventData.InputButton.Left;


        //clear the chache for the raycast
        m_RaycastResultCache.Clear();


        //this line allows to generate the pointer enter and exit in UI and other elements
        HandlePointerExitAndEnter(data,currentObject);

        // if pointer down
        if(InputManager.instance.T_R_DW)
        {
            ProcessPress();
        }
        //if pointer up
        if (InputManager.instance.T_R_UP)
        {
            ProcessRelease();
        }


    }

    public void ProcessPress()
    {
                      
        //execute with hierarchy
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(currentObject, data, ExecuteEvents.pointerDownHandler);

        if(newPointerPress==null)
        {
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentObject);
        }

        //additional parameters
        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = newPointerPress;
        data.clickCount = 1;
        data.button = PointerEventData.InputButton.Left;


    }

    public void ProcessRelease()
    {

       //similar to pointerdown, but with up
        ExecuteEvents.Execute(data.pointerPress,data, ExecuteEvents.pointerUpHandler);        

        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerUpHandler>(currentObject);

        if (data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        if (EventSystem.current!=null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        //additional parameters
        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;
        data.clickCount = 1;
        data.button = PointerEventData.InputButton.Left;
    }

    public PointerEventData GetData()
    {
        return data;
    }


    private RaycastHit CreateRayCast()
    {
        RaycastHit hit;


        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, 200);

        return hit;
    }
}
