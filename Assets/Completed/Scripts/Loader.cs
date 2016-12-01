using UnityEngine;
using System.Collections;

namespace Completed
{	
	//Controlador do loader.
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;
		public GameObject soundManager;
		
		
		void Awake ()
		{
			if (GameManager.instance == null)
				Instantiate(gameManager);
			
			if (SoundManager.instance == null)
				Instantiate(soundManager);
		}
	}
}