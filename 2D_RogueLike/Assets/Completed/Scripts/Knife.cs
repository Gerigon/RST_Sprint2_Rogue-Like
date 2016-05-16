using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Knife : MonoBehaviour {

	private float speed = 7.5f;				//the speed of the knife

	// Update is called once per frame
	void Update () 
	{
		//makes the knife go in an up direction
		transform.Translate (Vector2.up * speed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//checks if the knife hits an enemy or wall, and destroys the knife itself and the enemy
		if (other.gameObject.tag == "Enemy") 
		{
			Debug.Log ("You hit an enemy");
			other.gameObject.GetComponent<Completed.Enemy>().enemyDied = true;
			Destroy (other.gameObject);
			Destroy (gameObject);
		}

		if (other.gameObject.tag == "Wall") 
		{
			Debug.Log ("You hit the wall");
			Destroy (gameObject);
		}
	}
}
