using UnityEngine;
using System.Collections;

namespace Completed
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
		
		
		protected override void Start ()
		{
			GameManager.instance.AddEnemyToList (this);
			animator = GetComponent<Animator> ();
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			base.Start ();
		}
		
		
		//Override de AttemptMove para incluir o skipMove.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			if(skipMove)
			{
				skipMove = false;
				return;
				
			}
			
			base.AttemptMove <T> (xDir, yDir);
			skipMove = true;
		}
		
		
		//Move o inimigo.
		public void MoveEnemy ()
		{
			int xDir = 0;
			int yDir = 0;
			
			if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				yDir = target.position.y > transform.position.y ? 1 : -1;
			else
				xDir = target.position.x > transform.position.x ? 1 : -1;
			
			AttemptMove <Player> (xDir, yDir);
		}
		
		
		//Quando o inimigo tenta ir para a posição ocupada pelo player, gera dano ao player.
		protected override void OnCantMove <T> (T component)
		{
			Player hitPlayer = component as Player;
			hitPlayer.LoseFood (playerDamage);
			animator.SetTrigger ("enemyAttack");
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}
	}
}
