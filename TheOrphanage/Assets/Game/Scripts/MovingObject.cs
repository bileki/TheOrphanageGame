using UnityEngine;
using System.Collections;

namespace Game
{
	//Classe abstrata de movimento.
	public abstract class MovingObject : MonoBehaviour
	{
		public float moveTime = 0.1f;			//Tempo que levará o objeto a se mover, em segundos.
		public LayerMask blockingLayer;			//Camada na qual a colisão será verificada.


		private BoxCollider2D boxCollider; 		//O componente BoxCollider2D anexado ao objeto.
		private Rigidbody2D rb2D;				//O componente Rigidbody2D anexado ao objeto.
		private float inverseMoveTime;			//Usado para tornar o movimento mais eficiente.
		
		
		//Funções protected virtual podem ser sobrescrita por classes herdadas.
		protected virtual void Start ()
		{
			//Pega a referência do BoxCollider2D.
			boxCollider = GetComponent <BoxCollider2D> ();
			
			//Pega a referência do Rigidbody2D.
			rb2D = GetComponent <Rigidbody2D> ();
			
			//Define o inverso do tempo de movimento.
			inverseMoveTime = 1f / moveTime;
		}
		
		
		//Retorna se o movimento na direção passada é possível.
		protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
		{
			//Define a posição de início.
			Vector2 start = transform.position;
			
			//Calcula a posição final.
			Vector2 end = start + new Vector2 (xDir, yDir);
			
			//Desativa o boxCollider para que o linecast não acerte o próprio colisor do objeto.
			boxCollider.enabled = false;
			
			//Cria uma linha do início ao fim para verificar colisão.
			hit = Physics2D.Linecast (start, end, blockingLayer);
			
			//Rehabilita o boxCollider.
			boxCollider.enabled = true;
			
			//Verifica se algo foi atingido:
			if(hit.transform == null)
			{
				//Se nada foi atingido inicia a co-rotina SmoothMovement até a posição final.
				StartCoroutine (SmoothMovement (end));
				
				//Retorna true se o movimento foi bem sucedido.
				return true;
			}
			
			//Se houve alguma colisão, retorna false.
			return false;
		}
		
		
		//Co-rotina para mover unidades de um espaço para o próximo.
		protected IEnumerator SmoothMovement (Vector3 end)
		{
			//Calcula a raiz da distância restante.
			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			//Enquanto a distância for maior que Epsilon:
			while(sqrRemainingDistance > float.Epsilon)
			{
				//Encontra uma nova posição proporcionalmente mais perto do final, com base no moveTime.
				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				
				//Chama o MovePosition da nova posição.
				rb2D.MovePosition (newPostion);
				
				//Recalcula.
				sqrRemainingDistance = (transform.position - end).sqrMagnitude;
				
				//Retorna ao loop até sqrRemainingDistance ser próximo o suficiente de zero para terminar a função.
				yield return null;
			}
		}
		
		
		//Especifica o tipo de componente com o qual o objeto deve interagir se bloqueado (Player para inimigos, paredes para player).
		protected virtual void AttemptMove <T> (int xDir, int yDir)
			where T : Component
		{
			//Linecast atingido quando Move é chamado.
			RaycastHit2D hit;
			
			//Define se o movimento é possível.
			bool canMove = Move (xDir, yDir, out hit);
			
			//Verifica se nada foi atingido pelo linecast.
			if(hit.transform == null)
				//Se nada foi atingido, retornar e não executar código adicional.
				return;
			
			//Pega a referência do objeto atingido.
			T hitComponent = hit.transform.GetComponent <T> ();
			
			//Se o movimento não é possível e atingiu algo, pode-se interagir com o objeto.
			if(!canMove && hitComponent != null)
				
				//Chama a função de quando não se pode mover.
				OnCantMove (hitComponent);
		}
		
		
		//OnCantMove será substituído por funções nas classes herdadas.
		protected abstract void OnCantMove <T> (T component)
			where T : Component;
	}
}
