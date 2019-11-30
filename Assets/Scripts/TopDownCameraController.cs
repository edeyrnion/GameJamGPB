using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Time it takes to interpolate camera position"), Range(1f, 20f)]
    [SerializeField] private float _positionLerpTime = 10f;

    [Tooltip("Camera angle around world x-axis."), Range(0f, 90f)]
    [SerializeField] private float _cameraAngle = 50f;

    [Tooltip("Camera height"), Range(0f, 5f)]
    [SerializeField] private float _cameraPosY = 3f;

    [Tooltip("Camera minimal distance"), Range(0f, 10f)]
    [SerializeField] private float _distanceMin = 8f;

    [Tooltip("Camera maximal distance"), Range(10f, 20f)]
    [SerializeField] private float _distanceMax = 12f;

    [SerializeField] private Transform _player;

    private Camera _camera;

    #region CameraState
    private class CameraState
    {
        private float x;
        private float y;
        private float z;

        public void SetPosition(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public void LerpTowards(Vector3 target, float positionLerpPct)
        {
            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.position = new Vector3(x, y, z);
        }
    }
    #endregion

    private readonly CameraState _interpolatingCameraState = new CameraState();

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(_cameraAngle, 0f, 0f);
        transform.position = GetCameraPosition();
        _interpolatingCameraState.SetPosition(transform.position);
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(_cameraAngle, 0f, 0f);

        Vector3 desieredCameraPosition = GetCameraPosition();

        _interpolatingCameraState.LerpTowards(desieredCameraPosition, _positionLerpTime * Time.deltaTime);

        _interpolatingCameraState.UpdateTransform(transform);
    }

    Vector3 GetCameraPosition()
    {
        var target = _player.position;

        return target + (Quaternion.Euler(_cameraAngle, 0f, 0f) * (-Vector3.forward * _distanceMax));
    }
}