using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private bool _isFollowing;
    private Vector3 _lastNpcPosition;
    private Vector3 _offset;
    private bool _shouldRotate;
    private Camera _camera;

    public void select(bool shouldRotate, Camera cam)
    {
        _shouldRotate = shouldRotate;
        _isFollowing = true;
        _lastNpcPosition = transform.position;
        _camera = cam;

        if (!_shouldRotate) _offset = _camera.transform.position - transform.position;
        else _offset = new Vector3(0f, 3.25f, 0f);
    }

    public void unSelect()
    {
        _isFollowing = false;
    }

    private void Update()
    {
        Vector3 newNpcPosition = transform.position;
        if (_isFollowing)
        {
            _camera.transform.position = gameObject.transform.position + _offset;

            if (_shouldRotate) _camera.transform.rotation = gameObject.transform.rotation;
        }
        _lastNpcPosition = newNpcPosition;
    }
}