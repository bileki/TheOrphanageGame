using UnityEngine;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;
	using UnityEngine.UI;
	
	//Controla o jogo
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//Tempo para esperar antes de iniciar o nível, em segundos.
		public float turnDelay = 0.1f;							//Delay entre os turnos do player.
		public int playerFoodPoints = 100;						//Valor inicia de sanidade do player.
		public static GameManager instance = null;				//Instância estática do GameManager que permite que ele seja acessado por qualquer outro script.
		[HideInInspector] public bool playersTurn = true;		//Boolean para verificar se os jogadores estão ativados, escondidos no inspetor, mas públicos.
		
		
		private Text levelText;									//Texto para exibir o número do nível atual.
		private GameObject levelImage;							//Imagem para bloquear o nível enquanto os níveis estão sendo configurados, background para o levelText.
		private BoardManager boardScript;						//Armazena uma referência ao BoardManager, que configurará o nível.
		private int level = 1;									//Número do nível atual, expresso no jogo como "Cômodo 1".
		private List<Enemy> enemies;							//Lista de todos os inimigos, usadas para emitir comandos de movimento.
		private bool enemiesMoving;								//Boolean para verificar se os inimigos estão se movendo.
		private bool doingSetup = true;							//Boolean para verificar se o cenário está configurado, impede que o player se mova durante a configuração.
		

		void Awake()
		{
			if (instance == null)
				instance = this;
			else if (instance != this)
				Destroy(gameObject);	
			
			DontDestroyOnLoad(gameObject);
			enemies = new List<Enemy>();
			boardScript = GetComponent<BoardManager>();
			
			InitGame();
		}
		
		//Quando nível for carregado.
		void OnLevelWasLoaded(int index)
		{
			level++;
			InitGame();
		}
		
		//Inicializa o nível.
		void InitGame()
		{
			doingSetup = true;
			levelImage = GameObject.Find("LevelImage");
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			levelText.text = "Cômodo " + level;
			levelImage.SetActive(true);
			Invoke("HideLevelImage", levelStartDelay);
			enemies.Clear();
			boardScript.SetupScene(level);
			
		}
		
		
		//Oculta a imagem preta usada entre os níveis
		void HideLevelImage()
		{
			levelImage.SetActive(false);
			doingSetup = false;
		}
		
		void Update()
		{
			if(playersTurn || enemiesMoving || doingSetup)
				return;
			
			StartCoroutine (MoveEnemies ());
		}
		
		//Adiciona inimigos na lista.
		public void AddEnemyToList(Enemy script)
		{
			enemies.Add(script);
		}
		
		
		//Encerra o jogo quando a sanidade é 0.
		public void GameOver()
		{
			levelText.text = "É... você ficou louco...";
			levelImage.SetActive(true);
			enabled = false;
		}
		
		//Co-rotina para mover inimigos.
		IEnumerator MoveEnemies()
		{
			enemiesMoving = true;
			yield return new WaitForSeconds(turnDelay);
			
			if (enemies.Count == 0) 
			{
				yield return new WaitForSeconds(turnDelay);
			}
			
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].MoveEnemy ();
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			
			playersTurn = true;
			enemiesMoving = false;
		}
	}
}

