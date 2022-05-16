using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private List<float> _lanePositionsX;
    public int _currentLane = 1;
    public float _speed;
    public float _gravity;
    public float _rollGravity;
    public float _jumpGravity;
    public float _maxSpeed;
    public float _maxVelocity;
    public Transform _feetPos;
    private bool _moveLeft;
    private bool _moveRight;
    public float _moveSpeed;

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] CapsuleCollider _collider;

    private ScrollAnimation _scrollAnimation;

    private Vector2 _cursorStartPos;
    private bool _processInput;
    private Vector2 _cursorMovementVector;

    private void Start()
    {
        //callback na zvetseni collideru po zkonceni rollu
        _scrollAnimation = _animator.GetBehaviour<ScrollAnimation>();
        _scrollAnimation.OnStateExitEvent += RollEnd;
    }


    void Update()
    {
        #region input
#if UNITY_STANDALONE //todo: mozna tohle cele presunout do samostatneho souboru, jestli toho bude moc u android verze
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _cursorStartPos = (Vector2)Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _processInput = true;
            _cursorMovementVector = (Vector2)Input.mousePosition - _cursorStartPos;
        }

#elif UNITY_ANDROID
        // Touch touchInfo = Input.GetTouch(X) // x - který prst
        // touchInfo.phase // stav v kterem prst je: zaèal, drží, pouští
        if (Input.touchCount > 0)
        {
            Touch touchInfo = Input.GetTouch(0);
            if (touchInfo.phase == TouchPhase.Began)
            {
                _cursorStartPos = touchInfo.position;
                //print($"{_mousePosition} of ({Screen.width},{Screen.height})");
            }
            else if (touchInfo.phase == TouchPhase.Ended || touchInfo.phase == TouchPhase.Canceled)
            {
                _processInput = true;
                _cursorMovementVector = touchInfo.position - _cursorStartPos;
                print("ended");
            }
        }

#endif

        if (_processInput)
        {
            _processInput = false;

            //spocitani
            Vector2 diffNormalized = new Vector2(_cursorMovementVector.x / Screen.width, _cursorMovementVector.y / Screen.height);
            //print($"{_cursorMovementVector} | {_cursorMovementVector.x}, {Screen.width}, {_cursorMovementVector.x / Screen.width} | {_cursorMovementVector.y}, {Screen.height}, {_cursorMovementVector.y / Screen.height}");

            //0.2f je nastrel hodnoty, bude potreba jeste poupravit az otestuju na fyz mobilu
            if (diffNormalized.x > 0.2f)
            {
                GoRight();
            }
            else if (diffNormalized.x < -0.2f)
            {
                GoLeft();
            }
            else if (diffNormalized.y > 0.1f)
            {
                Jump();
            }
            else if (diffNormalized.y < -0.1f)
            {
                Roll();
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + _speed * Time.fixedDeltaTime * new Vector3(0, 0, 1));
        if (!IsGrounded())
        {
            _rigidbody.AddForce(Vector3.down * _gravity, ForceMode.VelocityChange);
        }
    }

    private bool IsGrounded() { return (Physics.Raycast(_feetPos.position, Vector3.down, 1f)); }


    private bool CanSwitchLane(int direction) { return !Physics.Raycast(_feetPos.position, Vector3.right * direction, 3); }


    private void Jump()
    {
        if (!IsGrounded()) return;

        _rigidbody.AddForce(Vector3.up * _jumpGravity, ForceMode.Impulse);
        _animator.SetTrigger("Jump");
        AudioManager.Instance.Play("SwipeUp");
    }

    private void Roll()
    {
        _collider.height = 0.4f;
        _collider.center = new Vector3(0, 0.2f, 0);


        _rigidbody.AddForce(Vector3.down * _rollGravity, ForceMode.Impulse);
        _animator.SetTrigger("Scroll");
        AudioManager.Instance.Play("SwipeDown");
    }

    private void RollEnd()
    {
        _collider.height = 1f;
        _collider.center = new Vector3(0, 0.5f, 0);
    }

    private void GoLeft()
    {
        if (CanSwitchLane(-1))
        {
            AudioManager.Instance.Play("SwipeMove");
            _animator.SetTrigger("Left");
            transform.position = new Vector3(_lanePositionsX[--_currentLane], transform.position.y, transform.position.z);
        }
    }

    private void GoRight()
    {
        if (CanSwitchLane(1))
        {
            AudioManager.Instance.Play("SwipeMove");
            _animator.SetTrigger("Right");
            transform.position = new Vector3(_lanePositionsX[++_currentLane], transform.position.y, transform.position.z);
        }
    }
}
