using UnityEngine;
using System.Collections;

namespace Completed
{
	//Classe abstrata de movimento.
	public abstract class MovingObject : MonoBehaviour
	{
		public float moveTime = 0.1f;			//Tempo que levará o objeto a se mover, em segundos.
		public LayerMask blockingLayer;			//Camada na qual a colisão será verificada.
		
		
		private BoxCollider2D boxCollider; 		//O componente BoxCollider2D anexado ao objeto.
		private Rigidbody2D rb2D;				//O componente Rigidbody2D anexado ao objeto.
		private float inverseMoveTime;			//Usado para tornar o movimento mais eficiente.
		
		
		protected virtual void Start ()
		{
			boxCollider = GetComponent <BoxCollider2D> ();
			rb2D = GetComponent <Rigidbody2D> ();
			inverseMoveTime = 1f / moveTime;
		}
		
		
		//Define se o movimento é possível, se não há objetos de colisão.
		protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
		{
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2 (xDir, yDir);
			boxCollider.enabled = false;
			hit = Physics2D.Linecast (start, end, blockingLayer);
			boxCollider.enabled = true;
			
			if(hit.transform == null)
			{
				StartCoroutine (SmoothMovement (end));
				return true;
			}
			
			return false;
		}
		
		
		//Co-rotina para mover unidades de um espaço para o próximo.
		protected IEnumerator SmoothMovement (Vector3 end)
		{
			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			while(sqrRemainingDistance > float.Epsilon)
			{
				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				rb2D.MovePosition (newPostion);
				sqrRemainingDistance = (transform.position - end).sqrMagnitude;
				yield return null;
			}
		}
		
		
		//Toma um parâmetro genérico T para especificar o tipo de componente com o qual esperamos que o objeto interaja se bloqueado (Player para inimigos, paredes para player).
		protected virtual void AttemptMove <T> (int xDir, int yDir)
			where T : Component
		{
			RaycastHit2D hit;
			bool canMove = Move (xDir, yDir, out hit);
			
			if(hit.transform == null)
				return;
			
			T hitComponent = hit.transform.GetComponent <T> ();
			
			if(!canMove && hitComponent != null)
				OnCantMove (hitComponent);
		}
		
		
		//Quando o objeto tenta ir para a posição ocupada por outro objeto.
		protected abstract void OnCantMove <T> (T component)
			where T : Component;
	}
}
