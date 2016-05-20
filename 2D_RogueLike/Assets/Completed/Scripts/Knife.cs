using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Knife : MonoBehaviour {

	private float speed = 7.5f;				//the speed of the knife
    
    public Sprite blood;
    

	void Update () 
	{
		//makes the knife go in an up direction
		transform.Translate (Vector2.up * speed * Time.deltaTime);
	}

    void EnemyToBlood(GameObject other)
    {
        other.gameObject.GetComponent<Completed.Enemy>().enemyDied = true;
        //Changes the Sprite to the bloody one.
        other.gameObject.GetComponent<SpriteRenderer>().sprite = blood;
        //Disables all the not-needed enemy components from the ex-enemy.
        other.gameObject.GetComponent<Animator>().enabled = false;
        other.gameObject.GetComponent<Completed.Enemy>().enabled = false;
        other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		//checks if the knife hits an enemy or wall, and destroys the knife itself and the enemy
		if (other.gameObject.tag == "Enemy") 
		{
			Debug.Log ("You hit an enemy");
            EnemyToBlood(other.gameObject);
            //Destroys the bullet
			Destroy (gameObject);
		}

		if (other.gameObject.tag == "Wall") 
		{
			Debug.Log ("You hit the wall");
			Destroy (gameObject);
		}
	}
}
