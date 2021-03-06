﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using System.Collections.Generic;
using UnityEngine.Analytics;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1;		//Delay time in seconds to restart level.
		public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 20;				//Number of points to add to player food points when picking up a soda object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text foodText;						//UI Text to display current player food total.
		public Image foodBar;
		private float maxFood = 250.0f;

		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
        public AudioClip PickUpSound_New;           //New Pickup sound for food.
        public AudioClip LvlCompleted_New;          //New Level Completed sound.
        public AudioClip GetHitSound_New;           //New Get hit sound.
        public AudioClip HitSound_New;              //New Hit sound.
		
		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;							//Used to store player food points total during level.
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.

		//Added variables
		private bool gotHit = false;				//Used to look if the player collides with an enemy
		private float gotHitTimer = 0.025f;			//Used as timer to make the player change color for 0.025 seconds

		public Camera cam;							//Stores the camera
		private float shake = 0.0f;					//used to make the camera shake
		private float shakeAmount = 0.2f;			//used for the amount of shakiness
		private float decreaseFactor = 0.75f;		//used for decreasing the shake
		public Vector3 originPosition;				//used for the original position of the camera

		//variables for knifeThrow
		public int knifeDir = 0;					//used to look in which direction the player is facing
		public GameObject knife;					//uses the prefab knife to instantiate later on
		private int knifes = 2;						//amount of knives
		public Text knifeText;                      //used to change the knifeText

        public bool IsGameOver;
        float upwardMotion = 0.002f;

        public GameObject fader;

        //Start overrides the Start function of MovingObject
        protected override void Start ()
		{
            fader = GameObject.Find("Fader");
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
			
			//Get the current food point total stored in GameManager.instance between levels.
			food = GameManager.instance.playerFoodPoints;
			
			//Set the foodText to reflect the current player food total.
			//foodText.text = "Food: " + food;
			foodBar.fillAmount = food / maxFood;

            

			//Call the Start function of the MovingObject base class.
			base.Start ();
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerFoodPoints = food;
		}

		private void OnEnable ()
		{
			//looks at the start position of the camera
			originPosition = cam.transform.localPosition;
		}
		
		private void Update ()
        {
            if (IsGameOver)
            {
                GameOver();
            }
            else
            {
                foodText.color = new Color(1, 1, 1, foodText.color.a - 0.01f);
                //If it's not the player's turn, exit the function.
                if (!GameManager.instance.playersTurn) return;

                int horizontal = 0;     //Used to store the horizontal move direction.
                int vertical = 0;       //Used to store the vertical move direction.

                //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

                //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
                horizontal = (int)(Input.GetAxisRaw("Horizontal"));

                //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
                vertical = (int)(Input.GetAxisRaw("Vertical"));

                //Check if moving horizontally, if so set vertical to zero.
                if (horizontal != 0)
                {
                    vertical = 0;
                }

                FlipPlayer();

                //Calling the functions
                

                CameraShaking();

                ThrowingKnifes();

                //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
                //Check if we have a non-zero value for horizontal or vertical
                if (horizontal != 0 || vertical != 0)
                {
                    //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                    //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                    AttemptMove<Wall>(horizontal, vertical);
                }
            }
            PlayerChangeColor();
        }

        private void FlipPlayer()
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }

        private void PlayerChangeColor()
		{
			//Change the color back to normal after 0.025s, after being hit by an enemy
			if (gotHit == true) 
			{
				gotHitTimer -= Time.deltaTime;
				if (gotHitTimer < 0) 
				{
					gameObject.GetComponent<SpriteRenderer>().color = Color.white;
					gotHitTimer = 0.025f;
					gotHit = false;
				}
			}
		}

		private void CameraShaking()
		{
			//CameraShake
			if(shake > 0)
			{
				cam.transform.localPosition = originPosition + Random.insideUnitSphere * shakeAmount;
				shake -= Time.deltaTime * decreaseFactor;
			}
			else
			{
				shake = 0.0f;
				cam.transform.localPosition = originPosition;
			}
		}

		private void ThrowingKnifes()
		{
			//Gets the knife directions
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				knifeDir = 0;
			}

			else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				knifeDir = 1;
			}

			else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				knifeDir = 2;
			}

			else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				knifeDir = 3;
			}

			//Throw knife if e or space has been pressed
			if(Input.GetKeyDown(KeyCode.E)  && knifes > 0 || Input.GetKeyDown(KeyCode.Space) && knifes > 0)
			{
				if(knifeDir == 0)
				{
					Instantiate(knife, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
				}

				if(knifeDir == 1)
				{
					Instantiate(knife, gameObject.transform.position, Quaternion.Euler(0, 0, 90));
				}
				if(knifeDir == 2)
				{
					Instantiate(knife, gameObject.transform.position, Quaternion.Euler(0, 0, 180));
				}

				if(knifeDir == 3)
				{
					Instantiate(knife, gameObject.transform.position, Quaternion.Euler(0, 0, -90));
				}

				knifes -= 1;
				knifeText.text = "Knife: " + knifes;
                //New knife hit sound.
                SoundManager.instance.PlaySingle(HitSound_New);
            }
		}
		
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Every time player moves, subtract from food points total.
			food--;
			
			//Update food text display to reflect current score.
			//foodText.text = "Food: " + food;

			foodBar.fillAmount = food / maxFood;
			
			//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
			base.AttemptMove <T> (xDir, yDir);
			
			//Hit allows us to reference the result of the Linecast done in Move.
			RaycastHit2D hit;
			
			//If Move returns true, meaning Player was able to move into an empty space.
			if (Move (xDir, yDir, out hit)) 
			{
				//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			}
			
			//Since the player has moved and lost food points, check if the game has ended.
			CheckIfGameOver ();
			
			//Set the playersTurn boolean of GameManager to false now that players turn is over.
			GameManager.instance.playersTurn = false;
		}
		
		
		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{
			//Set hitWall to equal the component passed in as a parameter.
			Wall hitWall = component as Wall;
			
			//Call the DamageWall function of the Wall we are hitting.
			hitWall.DamageWall (wallDamage);
			
			//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
			animator.SetTrigger ("playerChop");
		}
		
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "Exit")
            {
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                Invoke("Restart", restartLevelDelay);

                //New level completed sound.
                SoundManager.instance.PlaySingle(LvlCompleted_New);

                //Disable the player object since level is over.
                enabled = false;
            }

            //Check if the tag of the trigger collided with is Food.
            else if (other.tag == "Food")
            {
                //Add pointsPerFood to the players current food total.
                food += pointsPerFood;
                //Update foodText to represent current total and notify player that they gained points
                foodText.color = Color.white;
                foodText.text = "+" + pointsPerFood;
                foodBar.fillAmount = food / maxFood;

                //New food pickup sound.
                SoundManager.instance.PlaySingle(PickUpSound_New);
                //Disable the food object the player collided with.
                other.gameObject.SetActive(false);
            }

            //Check if the tag of the trigger collided with is Soda.
            else if (other.tag == "Soda")
            {
                //Add pointsPerSoda to players food points total
                food += pointsPerSoda;
                //Update foodText to represent current total and notify player that they gained points
                foodText.color = Color.white;
                foodText.text = "+" + pointsPerSoda;
                foodBar.fillAmount = food / maxFood;

                //New food pickup sound.
                SoundManager.instance.PlaySingle(PickUpSound_New);
                //Disable the soda object the player collided with.
                other.gameObject.SetActive(false);
            }
		}
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game.
			Application.LoadLevel (Application.loadedLevel);
		}

        

        //LoseFood is called when an enemy attacks the player.
        //It takes a parameter loss which specifies how many points to lose.
        public void LoseFood (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");

			//Show small second of color changing, because getting hit hurts a lot
			gotHit = true;
			gameObject.GetComponent<SpriteRenderer> ().color = Color.red;

            //New getting hit sound.
            SoundManager.instance.PlaySingle(GetHitSound_New);

            //Subtract lost food points from the players total.
            food -= loss;
            foodText.color = Color.white;
            foodText.text = "-" + loss;
            //Update the food display with the new total.
            //foodText.text = "-"+ loss + " Food: " + food;
            foodBar.fillAmount = food / maxFood;

			//Make the camera vibrate when you get hit
			if (shake <= 0.0f) {
				shake = 0.1f;
			}
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (food <= 0) 
			{
                animator.SetTrigger("playerDead");
                IsGameOver = true;
                //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
                SoundManager.instance.PlaySingle (gameOverSound);
                GetComponent<BoxCollider2D>().enabled = false;
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				//Call the GameOver function of GameManager.
				//GameManager.instance.GameOver ();
			}
		}
        private void GameOver()
        {
            Debug.Log(upwardMotion);
            upwardMotion = upwardMotion * 1.05f;
            upwardMotion = Mathf.Clamp(upwardMotion, 0, 0.1f);
            transform.Translate(new Vector3(0, upwardMotion, 0));
            StartCoroutine(FadeTo(1, 3));
        }
        IEnumerator FadeTo(float aValue, float aTime)
        {
            fader.SetActive(true);
            float alpha = fader.GetComponent<Image>().color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                fader.GetComponent<Image>().color = newColor;
                yield return null;
            }
            Destroy(GameManager.instance);
            Destroy(SoundManager.instance);
            Application.LoadLevel(0);
        }
    }
}

