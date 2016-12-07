using UnityEngine;
using System.Collections;

namespace Game
{
	public class Obj : MonoBehaviour
	{
		public AudioClip chopSound1;				//1 de 2 de áudios quando a parede é atacada pelo player.
		public AudioClip chopSound2;				//2 de 2 de áudios quando a parede é atacada pelo player.
		public Sprite dmgSprite;					//Sprite alternativo para exibir depois que a parede foi atacado pelo jogador.
		public int hp = 3;							//Pontos de sanidade perdidos.
		
		
		private SpriteRenderer spriteRenderer;		//Guarda a referência do SpriteRenderer.
		
		//Antes de executar o Start.
		void Awake ()
		{
			//Pega a referência do SpriteRenderer.
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}
		
		
		//Quando o player ataca o objeto.
		public void DamageObj (int loss)
		{
			//Chama a sequência de sons do ataque.
			SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			
			//Define spriteRenderer para o sprite de dano..
			spriteRenderer.sprite = dmgSprite;
			
			//Subtrai de hp a sanidade.
			hp -= loss;
			
			//Se os pontos perdidos forem menores que 0:
			if(hp <= 0)
				//Desabillita o gameObject.
				gameObject.SetActive (false);
		}
	}
}
