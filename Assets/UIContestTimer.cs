using RTSEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContestTimer : MonoBehaviour {
    public Text text;
    public int TeamIndex;
    private MFactionManager mgr = null;
	// Use this for initialization
	void Start () {
        mgr = MFactionManager.GetServerManager();
    }
	
	// Update is called once per frame
	void Update () {
		if(mgr == null)
        {
            mgr = MFactionManager.GetServerManager();
            return;
        }
        

        if(TeamIndex == 0 && text.text != mgr.Team1ContestTimer.ToString())
            text.text = mgr.Team1ContestTimer.ToString("F2");
        if(TeamIndex == 1 && text.text != mgr.Team2ContestTimer.ToString())
            text.text = mgr.Team2ContestTimer.ToString("F2");
    }
}
