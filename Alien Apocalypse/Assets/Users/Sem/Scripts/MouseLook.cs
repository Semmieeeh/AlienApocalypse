using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static MouseLook instance;

    [Header("Settings")]
    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor = true;
    [Space]
    private Vector2 sensitivity;
    [Space]
    public Vector2 smoothing = new Vector2(3, 3);
    public float sens;
    [Header("First Person")]
    public GameObject characterBody;

    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;

    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;
    Rigidbody rb;
    private Vector2 mouseDelta;

    [HideInInspector]
    public bool scoped;

    void Start()
    {
        instance = this;

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;
        sensitivity = new Vector2 (sens, sens);
        // Set target direction for the character body to its inital state.
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
        
        if (lockCursor)
            LockCursor();
        rb = characterBody.GetComponent<Rigidbody>();
    }

    public void LockCursor()
    {
        // make the cursor hidden and locked
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        _mouseAbsolute += _smoothMouse;

        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (characterBody)
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
            rb.MoveRotation(yRotation * targetCharacterOrientation);
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}
