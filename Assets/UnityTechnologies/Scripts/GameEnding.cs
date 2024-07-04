using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameEnding : MonoBehaviour
{

    public float displayImgDuration = 1f;
    public float fadeDuration = 1f;
    public GameObject player;
    public CanvasGroup endingCanvas;
    public CanvasGroup catchCanvas;
    public AudioSource exitAudio;
    public AudioSource caughtAudio;

    bool m_HasAudioPlayed;
    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;

    float m_Timer;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
        }
    }

    void Update()
    {
        if (m_IsPlayerAtExit)
        {
            EndLevel(endingCanvas, false, exitAudio);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel(catchCanvas, true, caughtAudio);
        }
    }

    public void CaughtPlayer()
    {
        m_IsPlayerCaught = true;
    }

    void EndLevel(CanvasGroup canvas, bool needRestart, AudioSource audioSource)
    {
        m_Timer += Time.deltaTime;
        canvas.alpha = m_Timer / fadeDuration;
        if (m_Timer > displayImgDuration + fadeDuration)
        {
            if (needRestart)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                Application.Quit();
            }
        }

        if (!m_HasAudioPlayed)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }
    }
}
