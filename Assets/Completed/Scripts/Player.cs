using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Completed
{
	//Player herda de MovingObject.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//Tempo de delay em segundos para reiniciar o nível.
		public int pointsPerFood = 10;				//Número de pontos a adicionar aos pontos de sanidade do player quando coletar uma nota.
		public int pointsPerSoda = 20;				//Número de pontos a adicionar aos pontos de sanidade do player quando coletar um tesouro.
		public int wallDamage = 1;					//Quanto dano um player recebe ao destruir um obstáculo.
		public Text foodText;						//Texto para exibir o total de sanidade do player.
		public AudioClip moveSound1;				//1 de 2 áudios para reproduzir quando o player se move.
		public AudioClip moveSound2;				//2 de 2 áudios para reproduzir quando o player se move.
		public AudioClip eatSound1;					//1 de 2 áudios para reproduzir quando o player coleta uma nota.
		public AudioClip eatSound2;					//2 de 2 áudios para reproduzir quando o player coleta uma nota.
		public AudioClip drinkSound1;				//1 de 2 áudios para reproduzir quando o player coleta um tesouro.
		public AudioClip drinkSound2;				//2 de 2 áudios para reproduzir quando o player coleta um tesouro.
		public AudioClip gameOverSound;				//Áudio de quando o player morre
		
		private Animator animator;					//Usado para armazenar uma referência ao componente do animador do Player.
		private int food;							//Usado para armazenar pontos de sanidade do player durante o nível.
		private Vector2 touchOrigin = -Vector2.one;	//Usado para armazenar a localização da origem de toque na tela para controles móveis.
		
		

		protected override void Start ()
		{
			animator = GetComponent<Animator>();
			food = GameManager.instance.playerFoodPoints;
			foodText.text = "Sanidade: " + food;
			
			base.Start ();
		}
		
		
		private void OnDisable ()
		{
			GameManager.instance.playerFoodPoints = food;
		}
		
		
		private void Update ()
		{
			if(!GameManager.instance.playersTurn) return;
			
			int horizontal = 0;
			int vertical = 0;
			
			//Verifica se está executando no editor Unity ou em uma compilação autônoma.
			#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			if(horizontal != 0)
			{
				vertical = 0;
			}
			//Verifique se está rodando em iOS, Android, Windows Phone 8 ou Unity iPhone
			#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			if (Input.touchCount > 0)
			{
				Touch myTouch = Input.touches[0];
				
				if (myTouch.phase == TouchPhase.Began)
				{
					touchOrigin = myTouch.position;
				}
				
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					Vector2 touchEnd = myTouch.position;
					float x = touchEnd.x - touchOrigin.x;
					float y = touchEnd.y - touchOrigin.y;
					touchOrigin.x = -1;
					
					if (Mathf.Abs(x) > Mathf.Abs(y))
						horizontal = x > 0 ? 1 : -1;
					else
						vertical = y > 0 ? 1 : -1;
				}
			}
			
			#endif //Fim da seção de compilação dependente de plataforma móvel iniciada acima com #elif
			
			if(horizontal != 0 || vertical != 0)
			{
				AttemptMove<Wall> (horizontal, vertical);
			}
		}
		
		//Override de AttemptMove para incluir o skipMove.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			food--;
			foodText.text = "Sanidade: " + food;
			base.AttemptMove <T> (xDir, yDir);
			RaycastHit2D hit;
			
			if (Move (xDir, yDir, out hit)) 
			{
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			}
			
			CheckIfGameOver ();
			GameManager.instance.playersTurn = false;
		}
		
		
		//Quando o player tenta ir para a posição ocupada por um objeto, habilita o poder de destruí-lo.
		protected override void OnCantMove <T> (T component)
		{
			Wall hitWall = component as Wall;
			hitWall.DamageWall (wallDamage);
			animator.SetTrigger ("playerChop");
		}
		
		
		// OnTriggerEnter2D é enviado quando outro objeto entra em um colisor de trigger anexado ao objeto.
		private void OnTriggerEnter2D (Collider2D other)
		{
			if(other.tag == "Exit")
			{
				Invoke ("Restart", restartLevelDelay);
				enabled = false;
			}
			else if(other.tag == "Food")
			{
				food += pointsPerFood;
				foodText.text = "+" + pointsPerFood + " Sanidade: " + food;
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				other.gameObject.SetActive (false);
			}
			else if(other.tag == "Soda")
			{
				food += pointsPerSoda;
				foodText.text = "+" + pointsPerSoda + " Sanidade: " + food;
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				other.gameObject.SetActive (false);
			}
		}
		
		
		//Restarta o cenário.
		private void Restart ()
		{
			Application.LoadLevel (Application.loadedLevel);
		}
		
		
		//Perde sanidade quando atacado.
		public void LoseFood (int loss)
		{
			animator.SetTrigger ("playerHit");
			food -= loss;
			foodText.text = "-"+ loss + " Sanidade: " + food;
			CheckIfGameOver ();
		}
		
		
		//Verifica se é fim de jogo.
		private void CheckIfGameOver ()
		{
			if (food <= 0) 
			{
				SoundManager.instance.PlaySingle (gameOverSound);
				SoundManager.instance.musicSource.Stop();
				GameManager.instance.GameOver ();
			}
		}
	}
}

