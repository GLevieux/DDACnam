using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTester : MonoBehaviour {

    DDAModelUnityBridge Dda ;

    const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

    List<DDADataManager.Attempt> Attempts = new List<DDADataManager.Attempt>();

    public void testDataSaveAndLoad()
    {
        Dda = GetComponent<DDAModelUnityBridge>();
        
        string challengeId = "Ch_";
        for (int i = 0; i < 20; i++)
            challengeId += glyphs[Random.Range(0, glyphs.Length)];
        string playerId = "Pl_";
        for (int i = 0; i < 20; i++)
            playerId += glyphs[Random.Range(0, glyphs.Length)];

        Dda.setChallengeId(challengeId);
        Dda.setPlayerId(playerId);
        Attempts.Clear();

        Debug.Log("Adding data to model, to make it save them");

        for (int i = 0; i < 500; i++)
        {
            //On sauve le résultat
            float diff = Random.Range(0.0f, 1.0f);
            DDADataManager.Attempt attempt = new DDADataManager.Attempt();
            attempt.Result = diff > 0.5 ? 1.0 : 0.0;
            attempt.Thetas = new double[1];
            attempt.Thetas[0] = diff;
            Dda.addLastAttempt(attempt);
            Attempts.Add(attempt);
        }

        Debug.Log("Comparing saved data to actual ones");
        bool testok = Dda.checkDataAgainst(Attempts);

        if(testok)
            Debug.Log("Test ok");
        else
            Debug.LogError("Test Data FAILED !");

    }
}
