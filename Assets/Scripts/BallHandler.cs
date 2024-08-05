using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Rigidbody2D _pivot;
    [SerializeField] private float _detachDelay;
    [SerializeField] private float _respawnDelay;

    private Camera _mainCamera;
    private bool _isDragging;
    private Rigidbody2D _currentBallRigidbody;
    private SpringJoint2D _currentBallSpringJoin;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;  
        SpawnNewBall();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentBallRigidbody == null)
            return;

        if(Touch.activeTouches.Count > 0)
        {
            _isDragging = true;
            _currentBallRigidbody.isKinematic = true;

            Vector2 touchPosition = new Vector2();
            foreach(Touch touch in Touch.activeTouches)
            {
                touchPosition += touch.screenPosition;
            }
            touchPosition /= Touch.activeTouches.Count;
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
            _currentBallRigidbody.position = worldPosition;
        }
        else
        {
            if(_isDragging)
            {
                LaunchBall();
            }

            _isDragging = false;
        }
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(_ballPrefab, _pivot.position, Quaternion.identity);

        _currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        _currentBallSpringJoin = ballInstance.GetComponent<SpringJoint2D>();

        _currentBallSpringJoin.connectedBody = _pivot;
    }

    private void LaunchBall()
    {
        _currentBallRigidbody.isKinematic = false;
        _currentBallRigidbody = null;

        Invoke(nameof(DetachBall), _detachDelay);
    }

    private void DetachBall()
    {
        _currentBallSpringJoin.enabled = false;
        _currentBallSpringJoin = null;

        Invoke(nameof(SpawnNewBall), _respawnDelay);
    }
}
