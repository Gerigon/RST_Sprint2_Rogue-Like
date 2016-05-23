using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(FadeTo(0, 2));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = GetComponent<Image>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            GetComponent<Image>().color = newColor;
            yield return null;
        }
    }
}
