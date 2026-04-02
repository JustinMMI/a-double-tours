using UnityEngine;
using System.Collections.Generic;

public class ObstacleGenerator : MonoBehaviour
{
    void Start()
    {
        GenerateObstacles();
    }

    public void GenerateObstacles()
    {
        List<string> allObstacles = new List<string> { "Énergie", "Pierre", "Inondation", "Feu", "Tentacule de BOB", "Serpents" };
        List<string> selectedObstacles = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, allObstacles.Count);
            selectedObstacles.Add(allObstacles[randomIndex]);
            allObstacles.RemoveAt(randomIndex);
        }

        List<int> tower1PossibleFloors = new List<int> { 6, 7, 8, 9, 10, 11, 12, 13 };
        List<int> tower2PossibleFloors = new List<int> { 21, 22, 23, 24, 25, 26, 27, 28 };

        List<int> bannedFloors = new List<int>();

        List<int> finalFloors = new List<int>();

        int obstaclesCountTower1 = Random.Range(1, 3);
        int obstaclesCountTower2 = 3 - obstaclesCountTower1;

        // --- PLACEMENT TOWER 1 ---
        for (int i = 0; i < obstaclesCountTower1; i++)
        {
            int floor = GetRandomFloor(tower1PossibleFloors, bannedFloors);
            finalFloors.Add(floor);
            BanNeighborsAndMirror(floor, bannedFloors);
        }

        // --- PLACEMENT TOWER 2 ---
        for (int i = 0; i < obstaclesCountTower2; i++)
        {
            int floor = GetRandomFloor(tower2PossibleFloors, bannedFloors);
            finalFloors.Add(floor);
            BanNeighborsAndMirror(floor, bannedFloors);
        }

        string sentenceObstacle = "Analyse terminée. Placez les dangers sur le plateau :\n\n";

        for (int i = 0; i < 3; i++)
        {
            int floor = finalFloors[i];
            string towerName = floor <= 15 ? "Tour 1" : "Tour 2";
            sentenceObstacle += "- " + selectedObstacles[i] + " -> " + towerName + ", étage " + floor + "\n";
        }

        _obstacleSentence = sentenceObstacle;
        Debug.Log(sentenceObstacle);
    }

    int GetRandomFloor(List<int> availableFloors, List<int> banned)
    {
        bool found = false;
        int testedFloor = 0;

        while (found == false)
        {
            int index = Random.Range(0, availableFloors.Count);
            testedFloor = availableFloors[index];

            if (banned.Contains(testedFloor) == false)
            {
                found = true;
            }
        }

        return testedFloor;
    }

    void BanNeighborsAndMirror(int floor, List<int> banned)
    {
        banned.Add(floor);
        banned.Add(floor - 1);
        banned.Add(floor + 1);

        int mirror = 0;
        if (floor <= 15) // Tour 
        {
            mirror = floor + 15;
        }
        else // Tour 2
        {
            mirror = floor - 15;
        }

        banned.Add(mirror);
        banned.Add(mirror - 1);
        banned.Add(mirror + 1);
    }

    public string sentenceObstacle
    {
        get { return _obstacleSentence; }
    }

    private string _obstacleSentence;
}

