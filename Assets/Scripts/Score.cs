using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score{
    int pScore;
    string name;

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            //should sanitize/censor
            name = value;
        }
    }

    public int PScore
    {
        get { return pScore; }
        set { pScore = value; }
    }
	
    public Score()
    {
        pScore = 0;
        name = "bob";
    }

    public Score(string _name, int _score)
    {
        pScore = _score;
        name = _name;
    }

}
