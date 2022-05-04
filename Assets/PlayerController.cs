using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private List<float> _lanePositionsX;
    private int _currentLane;


    [SerializeField] private Animator _animator;

    private Vector2 mousePosition;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_STANDALONE
        //Input.GetKeyDown(KeyCode.Mouse0);   // zaèal
        //Input.GetKey(KeyCode.Mouse0);   // drží
        //Input.GetKeyUp(KeyCode.Mouse0);     //pousti

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mousePosition = (Vector2)Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            //todo: tohle presunout do jinam, aby to bylo spolecne i pro touch input
            //spocitani vektoru

            Vector2 diff = (Vector2)Input.mousePosition - mousePosition;
            Vector2 diffNormalized = new Vector2(diff.x / Screen.width, diff.y / Screen.height);
            //Debug.Log($"{diff}, {Screen.width}, {diff / Screen.width} | {diff}, {Screen.height}, {diff / Screen.width}");

            if (diffNormalized.x > 0.2f)
            {
                _animator.SetTrigger("Right");
                print("right");
            }
            else if (diffNormalized.x < -0.2f)
            {
                GoLeft();
                print("left");
            }
            else if (diffNormalized.y > 0.1f)
            {
                _animator.SetTrigger("Jump");
                print("up");
            }
            else if (diffNormalized.y < -0.1f)
            {
                _animator.SetTrigger("Scroll");
                print("down");
            }
        }


#elif UNITY_ANDROID
        // Touch touchInfo = Input.GetTouch(X) // x - který prst
        // touchInfo.phase // stav v kterem prst je: zaèal, drží, pouští
#endif
    }

    private void GoLeft()
    {
        _animator.SetTrigger("Left");
        if (_currentLane != 2)
        {
            //transform.position.x = _lanePositionsX[_currentLane++];
            transform.position += new Vector3(_lanePositionsX[_currentLane--], 0, 0);
        }
    }

}
