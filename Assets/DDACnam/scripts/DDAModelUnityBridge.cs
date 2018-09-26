using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAModelUnityBridge : MonoBehaviour {

    DDAModel DdaModel;
    public string PlayerId = "UnknownPlayer";
    public string ChallengeId = "UnknownChallenge";
    public float ThetaStart = 0.2f;
    public bool DoNotUpdateAccuracy = false;
    
    public void setPlayerId(string playerId)
    {
        PlayerId = playerId;
        DdaModel.PlayerId = PlayerId;
    }

    public void setChallengeId(string challengeId)
    {
        ChallengeId = challengeId;
        DdaModel.ChallengeId = ChallengeId;
    }

    public void initPMAlgorithm(double lastTheta, bool wonLastTime = false)
    {
        DdaModel.setPMInit(lastTheta, wonLastTime);
    }
        
    void Awake () {
        DdaModel  = new DDAModel(new DDADataManagerLocalCSV(), PlayerId, ChallengeId);
        initPMAlgorithm(ThetaStart);
    }

    public void addLastAttempt(DDADataManager.Attempt attempt)
    {
        DdaModel.addLastAttempt(attempt);
    }

    public DDAModel.DiffParams computeNewDiffParams(double targetDifficulty, bool doNotUpdateLRAccuracy = false)
    {
        return DdaModel.computeNewDiffParams(targetDifficulty, doNotUpdateLRAccuracy || DoNotUpdateAccuracy);
    }

    //For test purpose only
    public bool checkDataAgainst(List<DDADataManager.Attempt> attempts)
    {
        return DdaModel.checkDataAgainst(attempts);
    }
}

