using UnityEngine;
using System.Collections;

namespace Game
{
	//Classe inimigo herda de MovingObject
	public class Enemy : MovingObject
	{
		public int playerDamage; 							//A quantidade de sanidade que o player perde ao atacar.
		public AudioClip attackSound1;						//Primeiro áudio de quando o player é atacado.
		public AudioClip attackSound2;						//Segundo áudio de quando o player é atacado.


		private Animator animator;							//Variável do tipo Animator para armazenar uma referência ao componente Animator do inimigo.
		private Transform target;							//Transformar para tentar mover em direção ao alvo.
		private bool skipMove;								//Boolean para determinar se o inimigo deve ou não pular o movimento.
		
		
		//Override do Start para criar o inimigo e suas animações.
		protected override void Start ()
		{
			//Adiciona o inimigo no controlador do jogo, para que o controlador o trate como um objeto e controle-o.
			GameManager.instance.AddEnemyToList (this);
			
			//Salva a referência do animador do inimigo.
			animator = GetComponent<Animator> ();
			
			//Procura o player para usá-lo como referência dos movimentos.
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			
			//Chama o Start da classe pai.
			base.Start ();
		}
		
		
		//Override de AttempMove para incluir o pulo do movimento.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Verifica se skipMove é true, se for, pula o movimento.
			if(skipMove)
			{
				skipMove = false;
				return;				
			}
			
			//Chama o AttempMove da classe pai.
			base.AttemptMove <T> (xDir, yDir);
			
			//Depois de um movimento, pula o próximo, para dar um tempo ao player.
			skipMove = true;
		}
		
		
		//Move o inimigo para tentar alcaçar o player.
		public void MoveEnemy ()
		{
			//Declara as direćões para x e y, de -1 a 1.
			int xDir = 0;
			int yDir = 0;
			
			//Se a difrença entre as posições do inimigo e do player forem maior que Epsilon:
			if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				
				//Se a y da posição do player for maior que y da posição desse inimigo, define a direção y=1 (para mover para cima). Se não, define para -1 (para mover para baixo).
				yDir = target.position.y > transform.position.y ? 1 : -1;
			else
				//Se a y da posição do player for maior que y da posição desse inimigo, define a direção x=1 (para mover para direita). Se não, define para -1 (para mover para esquerda).
				xDir = target.position.x > transform.position.x ? 1 : -1;
			
			//Chama AttempMove passando o player, a fim de tentar encontrá-lo.
			AttemptMove <Player> (xDir, yDir);
		}
		
		
		//Quando o inimigo tenta ir para a posição ocupada pelo player.
		protected override void OnCantMove <T> (T component)
		{
			//Declara hitPlayer como player.
			Player hitPlayer = component as Player;
			
			//Chama a função de perder sanidade.
			hitPlayer.LoseSanit (playerDamage);
			
			//Usa a animação de ataque.
			animator.SetTrigger ("enemyAttack");
			
			//Chama a sequência de sons para o ataque.
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}
	}
}
