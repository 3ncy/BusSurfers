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

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] CapsuleCollider _collider;

    private ScrollAnimation _scrollAnimation;

    private Vector2 _cursorStartPos;
    private bool _processInput;
    private Vector2 _cursorMovementVector;

    private void Start()
    {
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
            //print(diffNormalized);
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

        //todo: jen debug, odstranit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Random.Range(0, 2) == 1)
                AudioManager.Instance.ChangeMusic("Max");
            else
                AudioManager.Instance.ChangeMusic("Kiki");

        }
    }

    private void FixedUpdate()
    {
        //transform.position += transform.forward * _speed * Time.deltaTime;

        //_rigidbody.AddForce(new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, _speed));
        //_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, _speed);
        //_rigidbody.AddForce(new Vector3(0, 0, _speed), ForceMode.VelocityChange);
        //_rigidbody.AddForce(-transform.up * Time.deltaTime * _gravity);
        //print(_rigidbody.velocity);


        //_rigidbody.velocity = (transform.forward * _speed) + (Physics.gravity * _gravity);
        //if (_rigidbody.velocity.magnitude <= _maxSpeed)
        //    _rigidbody.AddRelativeForce(Vector3.forward * _speed);



        //_rigidbody.AddForce(_maxVelocity - transform.forward * _speed, ForceMode.VelocityChange);


        //float speedFactor = (_maxVelocity - _rigidbody.velocity.magnitude) / _maxVelocity;
        //_rigidbody.AddForce(speedFactor * _speed * Vector3.forward, ForceMode.VelocityChange);


        //_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 1) * _speed;

        //    float xMove = Input.GetAxisRaw("Horizontal"); // d key changes value to 1, a key changes value to -1
        //    float zMove = Input.GetAxisRaw("Vertical"); // w key changes value to 1, s key changes value to -1
        //    _rigidbody.velocity = new Vector3(xMove, _rigidbody.velocity.y, zMove) * _speed; // Creates velocity in direction of value equal to keypress (WASD). rb.velocity.y deals with falling + jumping by setting velocity to y. 


        _rigidbody.MovePosition(transform.position + _speed * Time.fixedDeltaTime * new Vector3(0, 0, 1));
        if (!IsGrounded())
        {
            _rigidbody.AddForce(Vector3.down * _gravity, ForceMode.VelocityChange);
        }
        //_rigidbody.MovePosition(new Vector3(transform.position.x, 0, transform.position.z) + _speed * Time.fixedDeltaTime * new Vector3(0, 0, 1));

    }

    private bool IsGrounded()
    {
        return (Physics.Raycast(_feetPos.position, Vector3.down, 1f));
    }


    private bool CanSwitchLane(int direction)
    {
        return !Physics.Raycast(_feetPos.position, Vector3.right * direction, 3);
    }


    private void Jump()
    {
        if (!IsGrounded()) return;
        print("grounded");
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
