using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Wall : MonoBehaviour
	{
		public AudioClip chopSound1;				//1 de 2 de áudios quando a parede é atacada pelo player.
		public AudioClip chopSound2;				//2 de 2 de áudios quando a parede é atacada pelo player.
		public Sprite dmgSprite;					//Sprite alternativo para exibir depois que a parede foi atacado pelo jogador.
		public int hp = 3;							//Pontos de sanidade perdidos.
		
		
		private SpriteRenderer spriteRenderer;		//Armazena uma referência de componente para o SpriteRenderer anexado.
		
		
		void Awake ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}
		
		
		//Quando o jogador ataca uma parede.
		public void DamageWall (int loss)
		{
			SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			spriteRenderer.sprite = dmgSprite;
			hp -= loss;
			
			if(hp <= 0)
				gameObject.SetActive (false);
		}
	}
}
