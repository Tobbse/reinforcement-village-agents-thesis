// original: http://www.unifycommunity.com/wiki/index.php?title=MouseOrbitZoom
//
// --01-18-2010

using UnityEngine;

public class OrbitCameraBehaviour : MonoBehaviour
{
    public GameObject agentPrefab;

    public Transform target;
    public Camera playerCamera;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public float zoomRate = 1.0f;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private NpcAgent _clickedNpc;
    private bool _isFollowing;
    private Camera _activeCamera;
    private float _lastMouseX;
    private float _lastMouseY;

    void Awake() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        setActiveCamera(Camera.main);
    }

    public void setActiveCamera(Camera cam)
    {
        _activeCamera = cam;
        _updateTarget();
    }

    private void _updateTarget()
    {
        if (target == null)
        {
            GameObject go = new GameObject("Cam Target");
            target = go.transform;
        }
        if (_activeCamera == null) return;
        target.transform.position = _activeCamera.transform.position + (transform.forward * distance);

        xDeg = Vector3.Angle(Vector3.right, _activeCamera.transform.right);
        yDeg = Vector3.Angle(Vector3.up, _activeCamera.transform.up);

        distance = 0.01f;
        currentDistance = distance;
        desiredDistance = distance;

        rotation = _activeCamera.transform.rotation;
        currentRotation = _activeCamera.transform.rotation;
        desiredRotation = _activeCamera.transform.rotation;
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        if (_activeCamera == playerCamera || MouseUiDetector.BlockedByUI || GameAcademy.IS_TRAINING) return;

        // Checking clicks.
        bool mouse1 = Input.GetMouseButtonDown(0);
        bool mouse2 = Input.GetMouseButtonDown(1);
        bool leftAlt = Input.GetKey(KeyCode.LeftAlt);

        if (mouse1 || mouse2)
        {
            _lastMouseX = Input.mousePosition.x;
            _lastMouseY = Input.mousePosition.y;
        }

        if ((mouse1 || mouse2) && !leftAlt)
        {
            _isFollowing = false;
            if (_clickedNpc != null)
            {
                _clickedNpc.InteractionHandler.unSelect();
                _clickedNpc.ShouldMonitorNpc = false;
                _clickedNpc = null;
            }

            Ray ray = _activeCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.name.Contains("Agent"))
                {
                    _clickedNpc = hitObject.GetComponent<NpcAgent>();
                    if (_clickedNpc == null) _clickedNpc = hitObject.GetComponentInParent<NpcAgent>();
                    if (_clickedNpc == null) return;
                    if (_clickedNpc.InteractionHandler == null) return;
                    _clickedNpc.InteractionHandler.select(mouse1, _activeCamera);
                    _clickedNpc.ShouldMonitorNpc = true;
                    _isFollowing = true;
                }
            }
        }

        if (_isFollowing) return;

        if (Input.GetMouseButton(1))
        {
            xDeg += (Input.mousePosition.x - _lastMouseX) * xSpeed * 0.0005f;
            yDeg -= (Input.mousePosition.y - _lastMouseY) * ySpeed * 0.0005f;

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = _activeCamera.transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            _activeCamera.transform.rotation = rotation;
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(2))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = _activeCamera.transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed * 3f);
            target.Translate(_activeCamera.transform.up * -Input.GetAxis("Mouse Y") * panSpeed * 3f, Space.World);
        }
        // otherwise move the camera.

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            float speed = panSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speed *= 5f;
            target.Translate(_activeCamera.transform.forward * speed);
        } else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            float speed = panSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speed *= 5f;
            target.Translate(_activeCamera.transform.forward * speed * -1f);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            float speed = panSpeed * 2f;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speed *= 5f;
            target.Translate(_activeCamera.transform.right * speed * -1f);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            float speed = panSpeed * 2f;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speed *= 5f;
            target.Translate(_activeCamera.transform.right * speed);
        }

        _lastMouseX = Input.mousePosition.x;
        _lastMouseY = Input.mousePosition.y;

        // affect the desired Zoom distance if we roll the scrollwheel
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            desiredDistance -= wheel * zoomRate * Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
        }

        // calculate position based on the new currentDistance 
        _activeCamera.transform.position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}