using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Wall : MonoBehaviour
	{
		public AudioClip chopSound1;				//1 of 2 audio clips that play when the wall is attacked by the player.
		public AudioClip chopSound2;				//2 of 2 audio clips that play when the wall is attacked by the player.
		public Sprite dmgSprite;					//Alternate sprite to display after Wall has been attacked by player.
        public Sprite stoneSprite;                  //Alternate sprite to be shown on this position after Wall has been destroyed.
		public int hp = 3;							//hit points for the wall.
		
		
		private SpriteRenderer spriteRenderer;		//Store a component reference to the attached SpriteRenderer.
        private BoxCollider2D box2d;
		
		
		void Awake ()
		{
			//Get a component reference to the SpriteRenderer.
			spriteRenderer = GetComponent<SpriteRenderer> ();
            box2d = GetComponent<BoxCollider2D>();
		}
		
		
		//DamageWall is called when the player attacks a wall.
		public void DamageWall (int loss)
		{
			//Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
			SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			
			//Set spriteRenderer to the damaged wall sprite.
			spriteRenderer.sprite = dmgSprite;
			
			//Subtract loss from hit point total.
			hp -= loss;
            Debug.Log(hp);

            //If hit points are less than or equal to zero:
            if (hp <= 0)
            {
                //Change the sprite of the gameObject.
                spriteRenderer.sprite = stoneSprite;
                //Disable the gameObject.
                box2d.enabled = false;
            }
		}
	}
}
