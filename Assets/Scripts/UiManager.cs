using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private bool _paused;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Text _pausedText;

    public void TogglePause()
    {
        _paused = !_paused;

        if (_paused)
        {
            Time.timeScale = 0;
            _pauseMenu.SetActive(true);
        }
        else
        {
            StartCoroutine(CountDownUnpause());
        }
    }

    IEnumerator CountDownUnpause()
    {
        for(int i = 3; i > 0; i--)
        {
            _pausedText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1);
        }
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
