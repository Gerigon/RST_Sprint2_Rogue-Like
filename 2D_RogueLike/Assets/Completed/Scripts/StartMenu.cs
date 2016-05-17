using UnityEngine;
using System.Collections;


namespace Completed
{
    public class StartMenu : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        public void StartRogueLike()
        {
            Application.LoadLevel(1);

        }

        public void Settings()
        {
            Debug.Log("settings");
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
