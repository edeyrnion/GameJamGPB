using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] [Range(1f, 10f)] private float _walkingSpeed = 8f;
    [SerializeField] [Range(0f, 1f)] private float _walkingReactionTime = 0.3f;
    [SerializeField] private LayerMask _groundLayer;

    private float axisX;
    private float axisY;

    private float movementSpeedX;
    private float movementSpeedY;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        axisX = Mathf.SmoothDamp(axisX, Input.GetAxisRaw("Horizontal"), ref movementSpeedX, _walkingReactionTime * 0.2f);
        axisY = Mathf.SmoothDamp(axisY, Input.GetAxisRaw("Vertical"), ref movementSpeedY, _walkingReactionTime * 0.2f);

        transform.Translate(axisX * _walkingSpeed * Time.deltaTime, 0f, axisY * _walkingSpeed * Time.deltaTime, Space.World);

        if (GetMousePosInWorldSpace(out Vector3 mousePos))
        {
            mousePos.y = transform.position.y;
            transform.LookAt(mousePos);
        }
    }

    private bool GetMousePosInWorldSpace(out Vector3 mousePos)
    {
        Vector3 mp = Input.mousePosition;
        var ray = _cam.ScreenPointToRay(mp);

        if (Physics.Raycast(ray, out RaycastHit hitinfo, 100f, _groundLayer))
        {
            mousePos = hitinfo.point;
            return true;
        }

        mousePos = new Vector3();
        return false;
    }
}
