using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private List<float> _lanePositionsX;
    public int _currentLane = 1;
    public float _speed;
    public float _gravity;

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;

    private Vector2 _cursorStartPos;
    private bool _processInput;
    private Vector2 _cursorMovementVector;


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
            print($"{_cursorMovementVector} | {_cursorMovementVector.x}, {Screen.width}, {_cursorMovementVector.x / Screen.width} | {_cursorMovementVector.y}, {Screen.height}, {_cursorMovementVector.y / Screen.height}");
            print(diffNormalized);
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

        //transform.position += transform.forward * _speed * Time.deltaTime;

        //_rigidbody.AddForce(new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, _speed));
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, _speed);
        //print(_rigidbody.velocity);



        //todo: jen debug, odstranit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Random.Range(0, 2) == 1)
                AudioManager.Instance.ChangeMusic("Max");
            else
                AudioManager.Instance.ChangeMusic("Kiki");

        }
    }

    private void Jump()
    {
        _animator.SetTrigger("Jump");
        AudioManager.Instance.Play("SwipeUp");
    }

    private void Roll()
    {
        _animator.SetTrigger("Scroll");
        AudioManager.Instance.Play("SwipeDown");
    }

    private void GoLeft()
    {
        if (_currentLane != 0)
        {
            AudioManager.Instance.Play("SwipeMove");
            _animator.SetTrigger("Left");
            Vector3 a = new Vector3(_lanePositionsX[--_currentLane], transform.position.y, transform.position.z);
            //print(a);

            transform.position = a;
        }
    }

    private void GoRight()
    {
        if (_currentLane != 2)
        {
            AudioManager.Instance.Play("SwipeMove");
            _animator.SetTrigger("Right");
            Vector3 a = new Vector3(_lanePositionsX[++_currentLane], transform.position.y, transform.position.z);
            //print(a);
            //_rigidbody.MovePosition(a);
            transform.position = a;
        }
    }
}
