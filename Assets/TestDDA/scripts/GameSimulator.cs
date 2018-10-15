using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSimulator : MonoBehaviour {

    public float PlayerLevel = 0.5f;
    public DDAModelUnityBridge Dda;
    
    List<double> Thetas = new List<double>();
    List<double> TargetDiff = new List<double>();
    List<double> Accuracies = new List<double>();
    List<bool> Used = new List<bool>();
    List<LineDrawer> LineDrawersTheta = new List<LineDrawer>();
    List<LineDrawer> LineDrawersTargetDiff = new List<LineDrawer>();
    List<LineDrawer> LineDrawersAccu = new List<LineDrawer>();
    List<LineDrawer> LineDrawersSquare = new List<LineDrawer>();

    // Use this for initialization
    void Start () {
        LineDrawersSquare.Add(new LineDrawer(0.01f));
        LineDrawersSquare.Add(new LineDrawer(0.01f));
        LineDrawersSquare.Add(new LineDrawer(0.01f));
        LineDrawersSquare.Add(new LineDrawer(0.01f));


        LineDrawersSquare[0].DrawLineInGameView(new Vector3(-0.8f, -0.8f, 1), new Vector3(0.8f, -0.8f, 1), Color.gray);
        LineDrawersSquare[1].DrawLineInGameView(new Vector3(0.8f, -0.8f, 1), new Vector3(0.8f, 0.8f, 1), Color.gray);
        LineDrawersSquare[2].DrawLineInGameView(new Vector3(0.8f, 0.8f, 1), new Vector3(-0.8f, 0.8f, 1), Color.gray);
        LineDrawersSquare[3].DrawLineInGameView(new Vector3(-0.8f, 0.8f, 1), new Vector3(-0.8f, -0.8f, 1), Color.gray);

    }

    // Update is called once per frame
    void Update () {

        if(Thetas.Count > 0 && LineDrawersTheta.Count != Thetas.Count)
        {
            while (LineDrawersTheta.Count < Thetas.Count)
                LineDrawersTheta.Add(new LineDrawer(0.01f));

            double X = -0.8;
            double Y = Thetas[0];
            float step = 1.6f / Thetas.Count;
            int i = 0;
            foreach (double theta in Thetas)
            {
                if (i > 0)
                {
                    LineDrawersTheta[i].DrawLineInGameView(new Vector3((float)X, (float)Y, 0), new Vector3((float)X + step, (float)(theta * 1.6) - 0.8f, 0), Color.blue);
                    X += step;
                }
                Y = (theta * 1.6f)-0.8f;
                i++;

            }
        }

        if (Accuracies.Count > 0 && LineDrawersAccu.Count != Accuracies.Count)
        {
            while (LineDrawersAccu.Count < Accuracies.Count)
                LineDrawersAccu.Add(new LineDrawer(0.01f));

            double X = -0.8;
            double Y = Thetas[0];
            float step = 1.6f / Thetas.Count;
            int i = 0;
            foreach (double accu in Accuracies)
            {
                if (i > 0)
                {
                    Color color = Used[i] ? Color.green : Color.red;
                    LineDrawersAccu[i].DrawLineInGameView(new Vector3((float)X, (float)Y, 0), new Vector3((float)X + step, (float)(accu * 1.6) - 0.8f, 0), color);
                    X += step;
                }
                Y = (accu * 1.6f) - 0.8f;
                i++;

            }
        }

        if (TargetDiff.Count > 0 && LineDrawersTargetDiff.Count != TargetDiff.Count)
        {
            while (LineDrawersTargetDiff.Count < TargetDiff.Count)
                LineDrawersTargetDiff.Add(new LineDrawer(0.01f));

            double X = -0.8;
            double Y = TargetDiff[0];
            float step = 1.6f / TargetDiff.Count;
            int i = 0;
            foreach (double diff in TargetDiff)
            {
                if (i > 0)
                {
                    LineDrawersTargetDiff[i].DrawLineInGameView(new Vector3((float)X, (float)Y, 0), new Vector3((float)X + step, (float)(diff * 1.6) - 0.8f, 0), Color.yellow);
                    X += step;
                }
                Y = (diff * 1.6f) - 0.8f;
                i++;

            }
        }


    }

    //Le joueur joue au challenge. 
    //Pour un vrai jeu on utilise la variable en entrée pour faire un LERP sur les params du jeu
    bool playerPlays(double thetaDiff)
    {
        double tryResult = RandomNormal.NextGaussian()*0.2 + PlayerLevel;
        if (tryResult > thetaDiff)
            return true;
        return false;
    }



    public void simulatePlayerTries(int nbtries)
    {
        simulatePlayerTries(nbtries, 0);
    }

    public void simulatePlayerTriesJunk(int nbtries)
    {
        simulatePlayerTries(nbtries, 1);
    }

    public void simulatePlayerTriesJunkWin(int nbtries)
    {
        simulatePlayerTries(nbtries, 2);
    }

    public void simulatePlayerTriesJunkFail(int nbtries)
    {
        simulatePlayerTries(nbtries, 3);
    }

    public void simulatePlayerTries(int nbtries, int junkDataType = 0)
    {
        for(int i=0;i< nbtries; i++)
        {
            //Courbe de difficulté aléatoire, a remplacer par la courbe voulue
            float nextDiff = Random.Range(0.0f, 1.0f); 

            //On demande au modèle de nous donner le bon paramètre theta en fonction de la proba voulue
            DDAModel.DiffParams diffParams = Dda.computeNewDiffParams(nextDiff);

            Debug.Log("New game ! \n"
                      + "Difficulty : " + diffParams.TargetDiff + "\n"
                      + "Difficulty with explo : " + diffParams.TargetDiffWithExplo + "\n"
                      + "Theta : " + diffParams.Theta + "\n"
                      + "Log Reg Used : " + diffParams.LRUsed + "\n"
                      + "Log Reg Accuracy : " + diffParams.LRAccuracy + "\n"
                      + "Nb Attempts Used : " + diffParams.NbAttemptsUsedToCompute + "\n");

            Thetas.Add(diffParams.Theta);
            Accuracies.Add(diffParams.LRAccuracy);
            Used.Add(diffParams.LRUsed);
            TargetDiff.Add(diffParams.TargetDiffWithExplo);


            //On fait jouer
            bool win = playerPlays(diffParams.Theta);

            if (junkDataType == 1)
                win = Random.Range(0.0f, 1.0f) > 0.5;
            if (junkDataType == 2)
                win = true;
            if (junkDataType == 3)
                win = false;

            if (win)
                Debug.Log("win !!");
            else
                Debug.Log("fail !!");

            //On sauve le résultat
            DDADataManager.Attempt attempt = new DDADataManager.Attempt();
            attempt.Result = win ? 1.0 : 0.0;
            attempt.Thetas = new double[1];
            attempt.Thetas[0] = diffParams.Theta;
            Dda.addLastAttempt(attempt);
        }
        
    }
    public void reloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    
}
