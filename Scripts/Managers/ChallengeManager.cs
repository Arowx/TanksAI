using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Complete
{
    public class ChallengeManager : MonoBehaviour
    {

        public GameObject[] tanks;  // all of the tanks in the challenge
        int[,] tankScores;
        string[] tankNames;

        public GameManager gm;

        private int tankA = 0;
        private int tankB = 1;

        private int tankCount;

        public Text mainText;
        public Text tankText;
        public Text tankAtxt;
        public Text tankBtxt;

        void Start()
        {
            gm.cm = this;

            tankCount = tanks.Length;
            tankScores = new int[tankCount, tankCount];
            tankNames = new string[tankCount];

            for (int i = 0; i < tankCount; i++)
            {
                tankNames[i] = tanks[i].GetComponent<TankAI>().tankName;
            }            

            BeginGame();
        }


        void BeginGame()
        {
            gm.m_TankPrefab[0] = tanks[tankA];
            gm.m_TankPrefab[1] = tanks[tankB];

            tankAtxt.text = tankNames[tankA];
            tankBtxt.text = tankNames[tankB];

            UpdateScores();

            gm.BeginGame();
        }


        public void UpdateScores()
        {
            string tankNameText = "";

            for (int i = 0; i < tankCount; i++)
            {
                int total = 0;
                tankNameText += tankNames[i] + "\t";

                for (int j = 0; j < tankCount; j++)
                {
                    if (i == j) tankNameText += "--\t";
                    else tankNameText += tankScores[i, j].ToString("00") + "\t";
                    total += tankScores[i, j];
                }

                tankNameText += total.ToString("000");
                if (i < tankCount - 1) tankNameText += "\n";
            }

            tankText.text = tankNameText;
            Debug.Log("Scores:\n" + tankNameText);
        }

        public void EndGame(int scoreA, int scoreB)
        {
            tankScores[tankA, tankB] = scoreA;
            tankScores[tankB, tankA] = scoreB;

            UpdateScores();
            NextGame();
        }

        void NextGame()
        {
            tankB++;

            if (tankB >= tankCount)
            {
                tankA++;
                tankB = tankA + 1;

                if (tankA >= tankCount - 1)
                {
                    ChallengeOver();
                    return;
                }
            }

            BeginGame();
        }

        void ChallengeOver()
        {
            mainText.text = "TANK AI\nCHALLENGE OVER";
            tankText.color = Color.white;
            Debug.Log("Challenge Over");
        }
    }
}