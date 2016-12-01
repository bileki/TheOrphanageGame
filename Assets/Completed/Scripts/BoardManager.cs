using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Completed
	
{
	//Controlador do cenário
	public class BoardManager : MonoBehaviour
	{
		[Serializable]
		public class Count
		{
			public int minimum;
			public int maximum;
			
			
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int columns = 8; 										//Número de colunas.
		public int rows = 8;											//Número de linhas.
		public Count wallCount = new Count (5, 9);						//Limites para o random de paredes.
		public Count foodCount = new Count (1, 5);						//Limites para o random de itens.
		public GameObject exit;											//Prefab da saída.
		public GameObject[] floorTiles;									//Vetor de prefab do chão.
		public GameObject[] wallTiles;									//Vetor de prefab de parede.
		public GameObject[] foodTiles;									//Vetor de prefab de itens.
		public GameObject[] enemyTiles;									//Vetor de prefab de inimigos.
		public GameObject[] outerWallTiles;								//Vetor de prefab de borda.
		
		private Transform boardHolder;									//Variável para guardar a referência para transformação do cenário
		private List <Vector3> gridPositions = new List <Vector3> ();	//Lista de posições possíveis para colocação dos tiles.
		
		
		//Limpa a lista gridPositions e prepara-a para gerar um novo cenário.
		void InitialiseList ()
		{
			gridPositions.Clear ();
			
			for(int x = 1; x < columns-1; x++)
			{
				for(int y = 1; y < rows-1; y++)
				{
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		
		//Define o chão e bordas.
		void BoardSetup ()
		{
			boardHolder = new GameObject ("Board").transform;
			
			for(int x = -1; x < columns + 1; x++)
			{
				for(int y = -1; y < rows + 1; y++)
				{
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
					if(x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					instance.transform.SetParent (boardHolder);
				}
			}
		}
		
		
		//Retorna posição para a lista gridPositions.
		Vector3 RandomPosition ()
		{
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			Vector3 randomPosition = gridPositions[randomIndex];
			gridPositions.RemoveAt (randomIndex);
			
			return randomPosition;
		}
		
		
		//Recebe um vetor de objetos para escolher os limites da quantidade de objetos para serem criados.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			int objectCount = Random.Range (minimum, maximum+1);
			
			for(int i = 0; i < objectCount; i++)
			{
				Vector3 randomPosition = RandomPosition();
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}
		
		
		//Inicializa o nível e chama funções para criar cenário.
		public void SetupScene (int level)
		{
			BoardSetup ();
			InitialiseList ();
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			
			int enemyCount = (int)Mathf.Log(level, 2f);
			
			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}
}
