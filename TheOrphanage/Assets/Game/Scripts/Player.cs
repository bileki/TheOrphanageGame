using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game
{
	//Player herda de MovingObject.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//Tempo de delay em segundos para reiniciar o nível.
		public int pointsPerNote = 15;				//Número de pontos a adicionar aos pontos de sanidade do player quando coletar uma nota.
		public int pointsPerTreasure = 30;				//Número de pontos a adicionar aos pontos de sanidade do player quando coletar um tesouro.
		public int objDamage = 2;					//Quanto dano um player recebe ao destruir um obstáculo.
		public Text sanitText;						//Texto para exibir o total de sanidade do player.
		public AudioClip moveSound1;				//1 de 2 áudios para reproduzir quando o player se move.
		public AudioClip moveSound2;				//2 de 2 áudios para reproduzir quando o player se move.
		public AudioClip collectSound1;					//1 de 2 áudios para reproduzir quando o player coleta uma nota.
		public AudioClip collectSound2;					//2 de 2 áudios para reproduzir quando o player coleta uma nota.
		public AudioClip collectSound3;				//1 de 2 áudios para reproduzir quando o player coleta um tesouro.
		public AudioClip collectSound4;				//2 de 2 áudios para reproduzir quando o player coleta um tesouro.
		public AudioClip gameOverSound;				//Áudio de quando o player morre.

		private Animator animator;					//Usado para armazenar uma referência ao componente do animador do Player.
		private int sanit;							//Usado para armazenar pontos de sanidade do player durante o nível.
		private bool fPause = false;				//Usado para armazenar o estado de pause.

		
		//Override do Start para definir animação e pontos de sanidade.
		protected override void Start ()
		{
			//Pega a referência do animador.
			animator = GetComponent<Animator>();
			
			//Pega o valor de sanidade até o momento.
			sanit = GameManager.instance.playerSanitPoints;
			
			//Seta o sanitText com o valor atual da sanidade.
			sanitText.text = "Sanidade: " + sanit;
			
			//Chama o Start da classe pai.
			base.Start ();
		}
		
		
		//Qaundo o player está desabilitado.
		private void OnDisable ()
		{
			//Atualiza valor de sanidade para o próximo nível.
			GameManager.instance.playerSanitPoints = sanit;
		}
		
		//Atualiza a cada frame.
		private void Update ()
		{
			//Se o player apertar espaço, pausa o jogo.
			if (Input.GetKeyDown (KeyCode.P)) 
			{
				//Cada vez que o player aperta P, nega-se a flag de pause.
				fPause = !fPause;

				//Envia para a função de pause a flag.
				GameManager.instance.Pause (fPause);
			}
			
			//Se não é a vez do player, sai da função.
			if(!GameManager.instance.playersTurn) return;
			
			int horizontal = 0;  	//Usado para guardar se o movimento é horizontal.
			int vertical = 0;		//Usado para guardar se o movimento é vertical.

			//Recebe a entrada do movimento horizontal.
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Recebe a entrada do movimento vertical.
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Verifica qual a dirção'para carregar a animação certa.
			if (vertical > 0)
			{
				animator.SetInteger("direction", 3);
			}
			else if (vertical < 0)
			{
				animator.SetInteger("direction", 1);
			}
			else if (horizontal < 0)
			{
				animator.SetInteger("direction", 4);
			}
			else if (horizontal > 0)
			{
				animator.SetInteger("direction", 2);
			}
			
			//Se o movimento é horizontal, define a vertical com 0.
			if(horizontal != 0)
			{
				vertical = 0;
			}

			//Verifica se horizontal ou vertical não são 0:
			if(horizontal != 0 || vertical != 0)
			{
				//Passa para o AttempMove uma parede, que pode ser o tipo de objeto que o player vai interagir.
				AttemptMove<Obj> (horizontal, vertical);
			}
		}
		
		//Override de AttemptMove para diminuir a sanidade.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Cada movimento diminui a sanidade.
			sanit--;
			
			//Atualiza o sanitText.
			sanitText.text = "Sanidade: " + sanit;
			
			//Chama o AttempMove da classe pai.
			base.AttemptMove <T> (xDir, yDir);
			
			//Linecast atingido quando Move é chamado.
			RaycastHit2D hit;
			
			//Se o movimento é possível:
			if (Move (xDir, yDir, out hit)) 
			{
				//Chama a sequência de sons do movimento.
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			}
			
			//Verifica se é fim de jogo.
			CheckIfGameOver ();
			
			//Define que não é mais a vez do player.
			GameManager.instance.playersTurn = false;
		}
		
		
		//Override para gerar dano quando ataca a parede.
		protected override void OnCantMove <T> (T component)
		{
			//Define a parede atacada como Wall.
			Obj hitObj = component as Obj;
			
			//Chama a função de dano ao atacar a parede.
			hitObj.DamageObj (objDamage);
			
			//Chama a animação de ataque.
			animator.SetTrigger ("playerChop");
		}
		
		
		//Quando o player choca-se em um objeto.
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Verifica se o objeto é a saída.
			if(other.tag == "Exit")
			{
				//Se for, vai para o nível.
				Invoke ("Restart", restartLevelDelay);
				
				//Desabilita o player.
				enabled = false;
			}
			
			//Verifica se o objeto é a nota.
			else if(other.tag == "Note")
			{
				//Adiciona pontos de sanidade.
				sanit += pointsPerNote;
				
				//Atualiza o sanitText.
				sanitText.text = "+" + pointsPerNote + " Sanidade: " + sanit;
				
				//Chama a sequência de sons de encontrar a nota.
				SoundManager.instance.RandomizeSfx (collectSound1, collectSound2);
				
				//Desativa a nota.
				other.gameObject.SetActive (false);
			}
			
			//Verifica se o objeto é o tesouro.
			else if(other.tag == "Treasure")
			{
				//Adiciona pontos de sanidade.
				sanit += pointsPerTreasure;

				//Atualliza o sanitText.
				sanitText.text = "+" + pointsPerTreasure + " Sanidade: " + sanit;
				
				//Chama a sequência de sons de encontrar o tesouro.
				SoundManager.instance.RandomizeSfx (collectSound3, collectSound4);
				
				//Desativa o tesouro.
				other.gameObject.SetActive (false);
			}
		}
		
		
		//Restart carrega outro nível.
		private void Restart ()
		{
			//Carrega o próximo nível.
			Application.LoadLevel (Application.loadedLevel);
		}
		
		
		//Quando o player é atacado pelo inimigo perde sanidade.
		public void LoseSanit (int loss)
		{
			//Define a animação de atacado.
			animator.SetTrigger ("playerHit");
			
			//Subtrai sanidade.
			sanit -= loss;
			
			//Atualiza o sanitText.
			sanitText.text = "-"+ loss + " Sanidade: " + sanit;
			
			//Verifica fm de jogo.
			CheckIfGameOver ();
		}
		
		
		//Verifica se o jogo acabou.
		private void CheckIfGameOver ()
		{
			//Se a sanidade for menos ou igual a 0:
			if (sanit <= 0) 
			{
				//Chama o som de gameover.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Para o som.
				SoundManager.instance.musicSource.Stop();
				
				//Avisa o GameManager que é fim de jogo.
				GameManager.instance.GameOver ();
			}
		}
	}
}

