using UnityEngine;
using System.Collections;

namespace Game
{	
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;
		public GameObject soundManager;
		
		
		void Awake ()
		{
			//Verifica se a instância do GameManager já existe, se não a cria.
			if (GameManager.instance == null)
				Instantiate(gameManager);
			
			//Verifica se a instância do SoundManager já existe, se não a cria.
			if (SoundManager.instance == null)
				Instantiate(soundManager);
		}
	}
}