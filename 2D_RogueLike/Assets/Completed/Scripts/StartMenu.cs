using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Completed
{


    public class StartMenu : MonoBehaviour
    {
        public GameObject fader;
        public GameObject soundManager;
        // Use this for initialization
        void Start()
        {
        }

        public void StartRogueLike()
        {
            StartCoroutine(FadeTo(1, 5f));
        }

        public void Settings()
        {
            Debug.Log("settings");
        }

        public void Exit()
        {
            Application.Quit();
        }

        IEnumerator FadeTo(float aValue, float aTime)
        {
            fader.SetActive(true);
            float alpha = fader.GetComponent<Image>().color.a;
            float volume = soundManager.GetComponent<AudioSource>().volume;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                soundManager.GetComponent<AudioSource>().volume = Mathf.Lerp(volume, 0, t);
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                fader.GetComponent<Image>().color = newColor;
                yield return null;
            }
            Application.LoadLevel(1);
        }
    }
}
