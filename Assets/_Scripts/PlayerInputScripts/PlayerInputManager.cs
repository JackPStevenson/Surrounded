using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public Transform thing;
    public Material swipingMaterial;
    public Material baseMat;
    private Transform mainCamTransform;

    private PlayrInput playerInput;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    [SerializeField] private float minimumSwipeMagnitude = 40f;
    [SerializeField] private float maximumSwipeMagnitude = 1000f;
    [SerializeField] private float totalswipeMagnitude;

    [SerializeField] private bool swiping = false;

    private Vector2 _swipeDirection;
    private List<Vector2> SwipePoints = new List<Vector2>();
    private Vector2 lastTouch;


    private void Awake()
    {
        playerInput = new PlayrInput();

        playerInput.Enable();
        playerInput.Touch.TouchPress.canceled += ProcessTouchComplete;
        playerInput.Touch.Swipe.performed += ProcessSwipeData;

        mainCamTransform = Camera.main.transform;
        touchPositionAction = playerInput.FindAction("TouchPosition");
        touchPressAction = playerInput.FindAction("TouchPress");

        baseMat = thing.gameObject.GetComponent<Renderer>().material;
    }

    private void ProcessSwipeData(InputAction.CallbackContext context)
    {
        _swipeDirection = context.ReadValue<Vector2>();
        if (_swipeDirection.magnitude < minimumSwipeMagnitude)
        {
            totalswipeMagnitude = 0f;
            swiping = false;
            thing.gameObject.GetComponent<Renderer>().material = baseMat;
        }
        else
        {
            totalswipeMagnitude += context.ReadValue<Vector2>().magnitude;
        }

        if (totalswipeMagnitude > 50 && !swiping)
        {
            swiping = true;
            thing.gameObject.GetComponent<Renderer>().material = swipingMaterial;
        }
        else if (swiping)
        {
            Ray ray = Camera.main.ScreenPointToRay(lastTouch);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 pos = hit.point;
                pos.y = 0;
                thing.position = pos;
            }
            else
            {
                Debug.Log("Missed");
            }
        }
    }


    // track when the button is pressed down
    // track the total magnitdude of the swipe
    // track when the button is released

    private void ProcessTouchComplete(InputAction.CallbackContext context)
    {
        if (swiping)
        {
            totalswipeMagnitude = 0f;
            swiping = false;
            thing.gameObject.GetComponent<Renderer>().material = baseMat;

        }
        else if (totalswipeMagnitude < 10f)
        {
            Ray ray = Camera.main.ScreenPointToRay(lastTouch);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 pos = hit.point;
                pos.y = 0;
                thing.position = pos;
            }
            else
            {
                Debug.Log("Missed");
            }
        }
    }


    public void OnTouchPosition(InputValue context)
    {
        lastTouch = context.Get<Vector2>();
        print("Set last touch to " + lastTouch);
    }

    public void OnTap(InputAction.CallbackContext context)
    {
        // Vector2 tapLocation = value.Get<Vector2>();
        if (context.performed)
        {
            Debug.Log(context.started);
            //Touch touch = context.ReadValue<Touch>();
            Vector2 TapLocation = Touchscreen.current.position.ReadValue();
            Debug.Log("Screen Tap Location" + context.ReadValue<Vector2>());

            Ray ray = Camera.main.ScreenPointToRay(TapLocation);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                thing.position = new Vector3(hit.point.x, 0, hit.point.y);
                //Debug.Log("Hit at " + hit.point);
            }
            else
            {
                Debug.Log("Missed");
            }
        }

    }
}
