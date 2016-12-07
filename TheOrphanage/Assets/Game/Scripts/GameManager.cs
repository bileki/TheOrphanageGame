using UnityEngine;
using System.Collections;

namespace Game
{
	using System.Collections.Generic; 
	using UnityEngine.UI;
	
	public class GameManager : MonoBehaviour
	{
		public float startDelay = 5f;							//Tempo para esperar na tela inicial, em segundos.
		public float levelStartDelay = 2f;						//Tempo para esperar antes de iniciar o nível, em segundos.
		public float turnDelay = 0.1f;							//Delay entre os turnos do player.
		public int playerSanitPoints = 100;						//Valor inicia de sanidade do player.
		public static GameManager instance = null;				//Instância estática do GameManager que permite que ele seja acessado por qualquer outro script.
		[HideInInspector] public bool playersTurn = true;		//Boolean para verificar se os jogadores estão ativados, escondidos no inspetor, mas públicos.


		private Text levelText;									//Texto para exibir o número do nível atual.
		private GameObject levelImage;							//Imagem para bloquear o nível enquanto os níveis estão sendo configurados, background para o levelText.
		private Text pauseText;									//Texto do pause.
		private GameObject initialImage;						//Imagem de entrada do jogo.
		private GameObject pauseImage;							//Imagem de pause do jogo.
		private GameObject gameOverImage;						//Imagem de fim de jogo.
		private BoardManager boardScript;						//Armazena uma referência ao BoardManager, que configurará o nível.
		private int level = 1;									//Número do nível atual.
		private List<Enemy> enemies;							//Lista de todos os inimigos, usadas para emitir comandos de movimento.
		private bool enemiesMoving;								//Boolean para verificar se os inimigos estão se movendo.
		private bool doingSetup = true;							//Boolean para verificar se o cenário está configurado, impede que o player se mova durante a configuração.

		
		
		//Antes de executar o Start.
		void Awake()
		{
			//Verifica se a instância existe.
			if (instance == null)
				
				//Se não, cria.
				instance = this;
			
			//Se existir, mas não for esta:
			else if (instance != this)
				
				//Destrói, pois só pode ter uma instância de GameManager.
				Destroy(gameObject);	
			
			//Define que gameObject não seja destruído ao recarregar o cenário.
			DontDestroyOnLoad(gameObject);
			
			//Associa a enemies a lista de inimigos.
			enemies = new List<Enemy>();
			
			//Pega a referência do script do BoardManager.
			boardScript = GetComponent<BoardManager>();
			
			//Chama a função de iniciar o jogo do nível.
			InitGame();
		}
		
		//Quando o nível é carregado.
		void OnLevelWasLoaded(int index)
		{
			//Incrementa o nível.
			level++;
			//Chama a função de iniciar o jogo do nível.
			InitGame();
		}
		
		//Inicializa o jogo para cada nível.
		void InitGame()
		{
			//Enquanto doingSetup é true o player não pode se mover, para que o player não se mova durante as transições de nível.
			doingSetup = true;

			//Pega a referência da InitialImage.
			initialImage = GameObject.Find ("InitialImage");

			//Pega a referência da LevelImage.
			levelImage = GameObject.Find ("LevelImage");

			//Pega a referência da GameOverImage.
			gameOverImage = GameObject.Find ("GameOverImage");

			//Pega a referência da InitialImage.
			pauseImage = GameObject.Find ("PauseImage");

			//Pega a referência da PauseText.
			pauseText = GameObject.Find("PauseText").GetComponent<Text>();

			//Pega a referência da LevelText.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();

			//Desativa a gameOverImage.
			gameOverImage.SetActive(false);

			//Desativa a initialImage.
			initialImage.SetActive (false);

			//Desativa a initialImage.
			levelImage.SetActive (false);

			//Desativa o pauseImage.
			pauseImage.SetActive (false);

			//Mostra a tela inicial.
			if (level == 1) 
			{
				//Define initialImage como ativo bloqueando o player visualizar o setup.
				initialImage.SetActive (true);

				//Espera alguns segundos
				Invoke ("HideInitialImage", levelStartDelay);
			} 
			else if (level == 10) 
			{
				//TODO Player muda de personagem.
				playerSanitPoints = 0;
			}
			else
			{
				//Define levelImage como ativo bloqueando o player visualizar o setup.
				levelImage.SetActive (true);

				//Mostra o cômodo atual.
				levelText.text = "Cômodo " + level;

				//Espera alguns segundos
				Invoke ("HideLevelImage", levelStartDelay);
			}

			//Apaga a lista de inimigos para usá-la no próximo nível.
			enemies.Clear();
			
			//Chama o setup do novo nível.
			boardScript.SetupScene(level);

			//Define doingSetup para false para que o player possa se mover.
			doingSetup = false;
			
		}

		//Oculta a imagem inicial.
		void HideInitialImage()
		{
			//Define o initialImage como inativo.
			initialImage.SetActive(false);
		}

		//Oculta a imagem do nível.
		void HideLevelImage()
		{
			//Define o levelImage como inativo.
			levelImage.SetActive(false);
		}
		

		//Atualização a cada frame.
		void Update()
		{
			//Se for a vez do player, ou o inimigo estiver se movendo ou o setup ativo, não começa o movimento dos inimigos.
			if(playersTurn || enemiesMoving || doingSetup)
				return;
			
			//Se não inicia o movimento dos inimigos.
			StartCoroutine (MoveEnemies ());
		}
		
		//Adiciona inimigos na lista.
		public void AddEnemyToList(Enemy script)
		{
			enemies.Add(script);
		}
		
		
		//Quando a sanidade é 0, chama a cena final.
		public void GameOver()
		{
			//Ativa a gameOverImage.
			gameOverImage.SetActive(true);

			//Espera alguns segundos
			Invoke("HideGameOverImage", levelStartDelay);

			//Desabilita o GameManager.
			enabled = false;

			Application.Quit ();
		}

		//Oculta a imagem de GameOver.
		void HideGameOverImage()
		{
			//Define o gameOverImage como inativo.
			gameOverImage.SetActive(false);
		}

		//Pausa o jogo
		public void Pause(bool p)
		{
			//Enquanto doingSetup é true o player não pode se mover, para que o player não se mova durante as transições de nível.
			doingSetup = p;

			//Ativa o pause.
			pauseImage.SetActive(p);
		}
		
		//Co-rotina de movimento de inimigos.
		IEnumerator MoveEnemies()
		{
			//Enquanto inimigos estão se movendo, player não pode se mover.
			enemiesMoving = true;
			
			//Espera pelo turnDelay (100 ms).
			yield return new WaitForSeconds(turnDelay);
			
			//Se não há inimigos(no primeiro nível, apenas:
			if (enemies.Count == 0) 
			{
				//Espera pelo turnDelay entre os movimentos.
				yield return new WaitForSeconds(turnDelay);
			}
			
			//Loop dentre os inimigos na lista.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Chama o movimento do i-ésimo inimigo.
				enemies[i].MoveEnemy ();
				
				//Espera o tempo de movimento do inimigo antes de mover o próximo.
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			//Quando todos os inimigos fizeram seu movimento, o player pode se mover.
			playersTurn = true;
			
			//Seta flag de que os inimigos não estão se movendo.
			enemiesMoving = false;
		}
	}
}

