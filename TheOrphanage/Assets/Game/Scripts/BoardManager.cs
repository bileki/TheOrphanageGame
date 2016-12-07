using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Game
	
{
	
	public class BoardManager : MonoBehaviour
	{
		// Uso do Serializable permite usar as sub-propriedades do Inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Valor máximo.
			public int maximum; 			//Valor mínimo.

			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int columns = 15; 										//Número de colunas.
		public int rows = 8;											//Número de linhas.
		public Count objCount = new Count (5, 12);						//Limites para o random de paredes.
		public Count itemCount = new Count (1, 8);						//Limites para o random de itens.
		public GameObject exit;											//Prefab da saída.
		public GameObject[] floorTiles;									//Vetor de prefab do chão.
		public GameObject[] objTiles;									//Vetor de prefab de parede.
		public GameObject[] itemTiles;									//Vetor de prefab de itens.
		public GameObject[] enemyTiles;									//Vetor de prefab de inimigos.
		public GameObject[] outerWallTiles;								//Vetor de prefab de borda.
		
		private Transform boardHolder;									//Variável para guardar a referência para transformação do cenário.
		private List <Vector3> gridPositions = new List <Vector3> ();	//Lista de posições possíveis para colocação dos tiles.
		
		
		//Limpa a lista gridPositions e prepara-a para gerar um novo cenário.
		void InitialiseList ()
		{
			//Limpa a lista gridPositions.
			gridPositions.Clear ();
			
			//Loop para as colunas.
			for(int x = 1; x < columns-1; x++)
			{
				//Loop para as linhas.
				for(int y = 1; y < rows-1; y++)
				{
					//Para cada índice adiciona um novo Vector3 à lista com as coordenadas x e y.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		
		//Define o chão e bordas.
		void BoardSetup ()
		{
			//Instancia o cenário e define boardHolder para sua transformação.
			boardHolder = new GameObject ("Board").transform;
			
			//Loop em x, começando de -1 para preencher as bordas.
			for(int x = -1; x < columns + 1; x++)
			{
				//Loop em y, começando de -1 para preencher as bordas.
				for(int y = -1; y < rows + 1; y++)
				{
					//Escolhe um chão aleatório do vetor de texturas para o chão.
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
					//Verifica se a posição é borda, para escolher uma parede aleatória.
					if(x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					//Instancia o GameObject usando o prefab escolhido pra atual grid de posição no loop.
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//Organiza a hierarquia do objeto.
					instance.transform.SetParent (boardHolder);
				}
			}
		}
		
		
		//Retorna uma posição aleatória para a lista gridPositions.
		Vector3 RandomPosition ()
		{
			//Declara um número aleatório de 0 até o tamanho do gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Usando o índice aleatório, define a posição aleatória.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Retira o índice da lista para não ser mais usado.
			gridPositions.RemoveAt (randomIndex);
			
			//Retorna o valor.
			return randomPosition;
		}
		
		
		//Define os objetos aleatórios que serão criados no cenário.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Declara um número aleatório de 0 até o tamanho do objectCount.
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instancia objetos aleatórios até o tamanho de objectCount.
			for(int i = 0; i < objectCount; i++)
			{
				//Define a posição aleatória.
				Vector3 randomPosition = RandomPosition();
				
				//Define o tipo de objeto da lista de objetos possíveis.
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//Instancia o objeto na posição aleatória.
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}
		
		
		//Inicializa o nível e chama funções para criar cenário.
		public void SetupScene (int level)
		{
			//Aleatoriza o tamanho do cenário.
			columns = Random.Range (8, 14);
			rows = Random.Range (6, 8);

			//Cria chão e paredes.
			BoardSetup ();
			
			//Inicializa a lista de posições.
			InitialiseList ();
			
			//Instancia os obstáculos dentro no cenário.
			LayoutObjectAtRandom (objTiles, objCount.minimum, objCount.maximum);
			
			//Instancia os tesouros e notas no cenário.
			LayoutObjectAtRandom (itemTiles, itemCount.minimum, itemCount.maximum);
			
			//Define o número de inimigos por nível baseado em progressão logarítmica.
			int enemyCount = (int)Mathf.Log(level, 2f);
			
			//Instancia os inimigos no cenário.
			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			
			//Instancia a saída do nível, sempre no canto superior direito.
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}
}
