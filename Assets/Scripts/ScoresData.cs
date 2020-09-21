using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class ScoresData
{
    private List<Score> highScores;
    private List<string> validate;
    private string defaultName;
    private string gameName;    //name of the game
    private string path;

    string testString;

    //public string Path
    public string GameName
    {
        get { return gameName; }
        set { gameName = value; }
    }

    public ScoresData(string _dName)
    {
        if (_dName.Length > 4)
            _dName = _dName.Substring(0, 4);
        defaultName = _dName;
        highScores = new List<Score>();
        validate = new List<string>();
        ReadValid();
    }

    public ScoresData() : this("JBOB")
    {
        
    }

    public bool ReadValid()
    {
        bool result = true;
        string filePath;
        if (Application.isEditor)
        {
            filePath = "UnityPatchManager";
            filePath = pathForDocumentsFile(filePath);
        }
        else
        {
            filePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
            filePath = filePath.Substring(0, filePath.LastIndexOf("/"));
            filePath = filePath.Substring(0, filePath.LastIndexOf("/"));
            filePath += "/UnityPatchManager";
        }
        if (File.Exists(filePath))
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader(filePath);
                while (!file.EndOfStream)
                {
                    validate.Add(file.ReadLine());
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }
        return result;
    }

    public bool ReadData()
    {
        highScores.Clear();
        bool result = true;
        int scoreCount = 0;
        Score newScore;
        string filePath = gameName + ".ini";
        filePath = pathForDocumentsFile(filePath);
        if (File.Exists(filePath))
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader(filePath);
                string nameLine;
                string scoreLine;
                string gm = file.ReadLine();
                while (!file.EndOfStream && scoreCount < 5)
                {
                    newScore = new Score();
                    nameLine = file.ReadLine();
                    scoreLine = file.ReadLine();
                    //Debug.Log(nameLine);
                    newScore.PScore = int.Parse(scoreLine.Substring(scoreLine.IndexOf('=')+1, scoreLine.Length - (scoreLine.IndexOf('=')+1)));
                    newScore.Name = nameLine.Substring(nameLine.IndexOf('=')+1, nameLine.Length - (nameLine.IndexOf('=') + 1));
                    //Debug.Log(newScore.Name);
                    highScores.Add(newScore);
                    scoreCount++;    
                }
                file.Close();
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }
        else
        {
            for(int i =0; i < 5; i++)
            {
                highScores.Add(new Score(defaultName, (5 - i) * 2));
            }

        }
        return result;
    }

    public string ScoreNames()
    {
        string temp = "";
        foreach(Score sc in highScores)
        {
            temp += sc.Name + "\n";
        }
        return temp;
    }

    public string ScoreScores()
    {
        string temp = "";
        foreach (Score sc in highScores)
        {
            temp += sc.PScore.ToString() + "\n";
        }
        return temp;
    }

    public bool WriteData()
    {
        bool result = true;
        string path = pathForDocumentsFile(gameName + ".ini");
        StreamWriter sw;
        try
        {
            sw = new StreamWriter(path, false);
            sw.WriteLine("[" + gameName + "]");
            for (int i = 1; i <= highScores.Count; i++)
            {
                sw.WriteLine("name" + i + "=" + highScores[i - 1].Name);
                sw.WriteLine("hscore" + i + "=" + highScores[i - 1].PScore);
            }
            sw.Close();
        }
        catch
        {
            Debug.Log("ERRRORRRR");
            result = false;
        }
        finally
        {
            
        }
        return result;
    }

    public void RewriteSingleScore(string name, int index)
    {
        bool outs = false;
        foreach(string valid in validate)
        {
            if((name.ToLower() == valid.ToLower() || name.ToLower().Contains(valid.ToLower())) && !outs)
            {
                name = defaultName;
                outs = true;
            }
        }
        highScores[index].Name = name;
    }

    public int CheckScore(int newscore,bool isLowBetter)
    {
        
        int insertPosition = -1;
        for (int i = highScores.Count-1; i >= 0; i--)
        {
            
            if ((highScores[i].PScore < newscore && !isLowBetter)|| (highScores[i].PScore > newscore && isLowBetter))
            {
                insertPosition = i;
            }
        }
        return insertPosition;
    }

    public void AddNewScore(int newScore, string newName, int insertPosition)
    {
        Score insertScore = new Score();
        insertScore.PScore = newScore;
        insertScore.Name = newName;
       for(int i= highScores.Count -1; i >= insertPosition; i--)
        {
            if (i == insertPosition)
                highScores[i] = insertScore;
            else
                highScores[i] = highScores[i - 1];
        }
    }

    public void DebugScores()
    {
        foreach (Score Sc in highScores)
        {
            Debug.Log(Sc.Name + "/" + Sc.PScore);
        }
    }


    public string pathForDocumentsFile(string filename)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }

        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }

        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }
}
