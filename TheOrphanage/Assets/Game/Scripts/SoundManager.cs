using UnityEngine;
using System.Collections;

namespace Game
{
	public class SoundManager : MonoBehaviour 
	{
		public AudioSource efxSource;					//Referência de efeitos de som.
		public AudioSource musicSource;					//Referência de sons.
		public static SoundManager instance = null;		//Habilita outros scripts instanciarem o SoundManager.
		public float lowPitchRange = .95f;				//O efeito de som mais baixo será aleatoriamente lançado.
		public float highPitchRange = 1.05f;			//O efeito de som mais alto será aleatoriamente lançado.
		
		//Antes de executar o Start.
		void Awake ()
		{
			//Verifica se a instância existe.
			if (instance == null)

				//Se não, cria.
				instance = this;
			
			//Se existir, mas não for esta:
			else if (instance != this)
				//Destrói, pois só pode ter uma instância de SoundManager.
				Destroy (gameObject);
			
			//Define que gameObject não seja destruído ao recarregar o cenário.
			DontDestroyOnLoad (gameObject);
		}
		
		
		//Usado para tocar um som.
		public void PlaySingle(AudioClip clip)
		{
			//Define qual efeito de som.
			efxSource.clip = clip;
			
			//Toca o som.
			efxSource.Play ();
		}
		
		
		//Escolhe aleatoriamente sons de um vetor.
		public void RandomizeSfx (params AudioClip[] clips)
		{
			//Gera um número aleatório.
			int randomIndex = Random.Range(0, clips.Length);
			
			//Escolhe um pitch aleatório..
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			
			//Define o pitch.
			efxSource.pitch = randomPitch;
			
			//Carrega o som para tocar.
			efxSource.clip = clips[randomIndex];
			
			//Toca o som
			efxSource.Play();
		}
	}
}
