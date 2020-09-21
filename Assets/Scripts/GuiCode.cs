/* GUI CODE SCRIPT FOR CC STARTER KIT
 * 
 * INSTRUCTIONS FOR USE
 * 1. Import GuiCode.cs, ScoreData.cs, and Score.cs into your project.
 * 2. Add the GuiCode script to an object in every scene of your project.
 * 3. On the first and last scene
 *      a. If you're using our display code check the "Uses Legacy Score Display" Box
 *      b. If you want the UI to auto generate, choose Create High Scores...for me Box
 *         if you want to handplace your UI scores display, use the high scores name and scores box.
 * 4. On every gameplay scene...
 *      a. If you have your own timer system or means of ending the game, uncheck the legacy timer box
 * 5. Be sure to add every scene in your game to the project. Order them correctly.
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GuiCode : MonoBehaviour {
    #region "variables used"
    [Header("Pause UI and Settings")]
    public bool usePauseSystem = true;
    
    [SerializeField] GameObject[] pauseUI;      //Overlay for the Pause Feature
    
    private int sIndex;                         //Index holder for new high score

    [Header("Scene Management")]
    [SerializeField] bool isOneScene;
    public GameObject quitButton, playButton;

    [Header("High Scores Display")]
    [SerializeField] private bool createHighScoresDisplayForMe = true;
    [SerializeField] private bool usesCustomCanvasHighScoreDisplay = false;
    [SerializeField] GameObject newScoreName;   //Object reference for button
    public string defaultHighScoreName = "JBOB";
    public GameObject HighScoresNameBox, HighScoresScoreBox;
    [SerializeField] private float displayYOffset, displayXOffset;
    [SerializeField] private bool usesLegacyScoreDisplay = true;
    public bool retainScoreBetweenGameScenes = true;
    [SerializeField] bool isLowerScoreBetter = false;

    [Header("Timers and Timeout")]
    [SerializeField] private bool usesLegacyTimer = false;
    [SerializeField] private bool usesApplicationTimeOut = true;
    [Range(0, 240)] public int gameCountDownTime = 5;
    [Range(0, 10)] public int gameOverDisplayTime = 2;
    

    private float timeTicker;                   //Time tracker for timeout
    private bool alreadyChecked = false,        //Flag for checking if is a HighScore once
                 highScore;                     //Flag for if it is a highscore
    private List<string> sceneNames;            //List of sceneNames, grabbed from build index
    ScoresData highScoreData;                   //HighScore information
    
    int displayTime;                            //Container for the displayed time
    string stringTime;                          //Temp space for displaying time
    string scenesData;
    #endregion
    string gameName;                            //GameName, populated by data folder
                //are high scores measured by low or high
    public static bool usesPauseSystem = true;
    public static bool highScoresScreen, gameScreen, titleScreen;
    public static bool isPaused;                //IS THE GAME PAUSED?      
    public static int score;                    //KEEPS SCORE AND IS USED ELSEWHERE TO CHECK FOR HIGH SCORE
    public static bool isGameOver;
    public static float guiTime;                //Time the game runs for
    private bool hasDisplayedDialog, isIPF;
    public GameObject newContainer;
    public GameObject legacyContainer;

    [SerializeField] private bool debugMode;
    Font displayfont;

    public string DefaultHighScoreName
    {
        get { return defaultHighScoreName; }
    }

    // Use this for initialization
    void Start () {
        highScoresScreen = false;
        hasDisplayedDialog = false;
        if (defaultHighScoreName.Length > 4) defaultHighScoreName = defaultHighScoreName.Substring(0, 4);
        gameName = Application.productName;
        if ((createHighScoresDisplayForMe || usesLegacyScoreDisplay || usesLegacyTimer) && displayfont == null)
        {
            displayfont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        isGameOver = false;
        #region "High Scores"
        // Since this script reloads every scene.
        if (highScoreData == null)
        {
            highScoreData = new ScoresData(defaultHighScoreName);
            highScoreData.GameName = gameName; //CHANGE CCStarter2018 TO NAME OF YOUR GAME
            sceneNames = new List<string>();
#if UNITY_STANDALONE
            if (!isOneScene)
            {
                scenesData = ReadSceneData();
                foreach (string sceneName in scenesData.Split(','))
                {
                    sceneNames.Add(sceneName);
                }
            }
#endif
#if UNITY_EDITOR
            if (!isOneScene)
            {
                sceneNames.Clear();
                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    sceneNames.Add(scene.path.Substring(scene.path.LastIndexOf("/") + 1, (scene.path.LastIndexOf(".") - scene.path.LastIndexOf("/")) - 1));
                }
                WriteSceneData();
            }
                WriteManagerIni();
#endif
            
            timeTicker = 0;
            highScoreData.ReadData();
        }

        if (!isOneScene)
        {
            //if the scene is the first or last scene
            if (SceneManager.GetActiveScene().name == sceneNames[0] || SceneManager.GetActiveScene().name == sceneNames[sceneNames.Count - 1])
            {
                //if the game is the opening scene, set the usesPauseSystem var to the static var
                if (SceneManager.GetActiveScene().name == sceneNames[0]) usesPauseSystem = usePauseSystem;
                //create the score display if checked
                if (createHighScoresDisplayForMe)
                {
                    CreateHighScoresDisplay();
                }

                if (GameObject.Find("btn_Play") != null)
                    GameObject.Find("btn_Play").GetComponent<Button>().onClick.AddListener(PlayGame);
                else if (playButton != null)
                    playButton.GetComponent<Button>().onClick.AddListener(PlayGame);
                DisplayScoresDisplay();
            }
            else if (SceneManager.GetActiveScene().name != sceneNames[0] && SceneManager.GetActiveScene().name != sceneNames[sceneNames.Count - 1])
            {
                //this is a game scene, it isn't the first or last scene in the project, call reset to default all values 
                if(!retainScoreBetweenGameScenes)
                    ResetGame();
                else
                {
                    if (usesLegacyTimer)
                        guiTime = gameCountDownTime;
                }
                //the game doesn't start paused
                isPaused = false;
                //make all of the UI elements invisible
                foreach (GameObject gO in pauseUI)
                {
                    gO.SetActive(false);
                }
            }
        }
        else
        {
            titleScreen = true;
            if (createHighScoresDisplayForMe)
            {
                CreateHighScoresDisplay();
            }
            //if the whole project is in one scene, then all you need is to put the information into the boxes.
            DisplayScoresDisplay();
        }
        #endregion
    }
    
    public void DisplayScoresDisplay()
    {
        // There is a legacy high scores display, 
        // that uses the gameobject textboxes attached to this script.
        if (usesCustomCanvasHighScoreDisplay)
        {
            //if there is an object called txt_HighNames, put the high scores into it
            //if not, check the public serialized box, and store the information there
            if (GameObject.Find("txt_HighNames") != null)
                GameObject.Find("txt_HighNames").GetComponent<Text>().text = highScoreData.ScoreNames();
            else if (HighScoresNameBox != null)
                HighScoresNameBox.GetComponent<Text>().text = highScoreData.ScoreNames();

            if (GameObject.Find("txt_HighScore") != null)
                GameObject.Find("txt_HighScore").GetComponent<Text>().text = highScoreData.ScoreScores();
            else if (HighScoresScoreBox != null)
                HighScoresScoreBox.GetComponent<Text>().text = highScoreData.ScoreScores();
        }
    }

    private string ReadSceneData()
    {
        string result="";
        string filePath = highScoreData.pathForDocumentsFile(gameName + "_Scenelist.ini");
        if (File.Exists(filePath))
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader(filePath);
                file.ReadLine();
                result = file.ReadLine();
                file.Close();
            }
            catch
            {
                result = "failed";
            }
        }
        return result;
    }

    private void WriteSceneData()
    {
        bool result = true;
        StreamWriter sw;
        try
        {
            
            sw = new StreamWriter(highScoreData.pathForDocumentsFile(gameName + "_Scenelist.ini"), false);
            sw.WriteLine("[" + gameName + "]");
            for (int i = 0; i < sceneNames.Count; i++) 
            {
                sw.Write(sceneNames[i]);
                if (i < sceneNames.Count - 1)
                    sw.Write(",");
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
    }

    private void WriteManagerIni()
    {
        string path = highScoreData.pathForDocumentsFile(gameName + "_Info.ini");
        if(!File.Exists(path))
        {
            StreamWriter sw;
            try
            {
                sw = new StreamWriter(path);
                sw.WriteLine("techTitle=" + gameName);
                sw.WriteLine("displayTitle=" + gameName);
                sw.WriteLine("authors=" + Application.companyName);
                sw.WriteLine("description=Your Description goes here");
                sw.WriteLine("played=0");
                sw.WriteLine("players=1");
                sw.WriteLine("favYear=");
                sw.WriteLine("year="+DateTime.Now.Year);
                sw.WriteLine("tags=comma,seperated,tags,here");
                sw.WriteLine("artRating=0");
                sw.WriteLine("gameRating=0");
                sw.WriteLine("genRating=0");
                sw.WriteLine("votes=0");
                sw.WriteLine("CabinetArt=" + gameName + "_Art.png");
                sw.WriteLine("movieFile=" + gameName + "_Movie.mp4");
                sw.Close();
            }
            catch
            {
                Debug.Log("error");
            }
            
        }
    }

    void PauseGame(bool result)
    {
        if (usesPauseSystem)
        {
            if (!isOneScene || (isOneScene && gameScreen))
            {
                foreach (GameObject gO in pauseUI)
                {
                    gO.SetActive(result);
                }
                if (result)
                {
                    Time.timeScale = 0;
                }
                else if (!result)
                {
                    Time.timeScale = 1;
                }
                isPaused = result;
            }
        }
    }

    void PauseGame()
    {
        if (usesPauseSystem)
        {
            bool toggleIt;
            if (Time.timeScale == 0)
            {
                isPaused = false;
                Time.timeScale = 1;
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0;
            }
            if (isPaused)
                toggleIt = false;
            else
                toggleIt = true;
            foreach (GameObject gO in pauseUI)
            {
                gO.SetActive(toggleIt);
            }
        }
    }


    void HighScoresScreen()
    {


        if (createHighScoresDisplayForMe)
        {
            if (newContainer != null)
            {
                if (!newContainer.activeInHierarchy)
                {
                    newContainer.SetActive(true);
                }
            }
        }
        if(usesCustomCanvasHighScoreDisplay)
        {
            if(newContainer != null)
            {
                if(!newContainer.activeInHierarchy)
                {
                    newContainer.SetActive(true);
                }
            }
        }
        if (debugMode && Input.GetKeyDown(KeyCode.F10))
            PlayGame();

        if (newScoreName == null && !createHighScoresDisplayForMe)
        {
            newScoreName = GameObject.Find("ipf_newScore");
        }

        //even if there wasn't a high scores screen made by the computer, then there is a chance that the player used an InputField
        if (newScoreName != null)
        {
            if (newScoreName.GetComponent<InputField>() != null)
            {

                isIPF = true;
                newScoreName = newScoreName.transform.Find("Text").gameObject;
            }
        }
        #region "High Scores"
        if (!alreadyChecked)
        {
            ScoresDisplayUpdate();
            //UpdateScoreDisplay();
            alreadyChecked = true;
            sIndex = highScoreData.CheckScore(score, isLowerScoreBetter);
            if (sIndex != -1) // there is a new high score
            {
                highScore = true;
                highScoreData.AddNewScore(score, defaultHighScoreName, sIndex);
            }
        }
        if (GameObject.Find("btn_Quit") != null)
            GameObject.Find("btn_Quit").GetComponent<Button>().onClick.AddListener(QuitGame);
        else if (quitButton != null)
            quitButton.GetComponent<Button>().onClick.AddListener(QuitGame);

        string message = "YOU HAVE A HIGH SCORE!\nEnter your initials", newName = "";
        if (highScore)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                highScoreData.WriteData();
            }
            if (GameObject.Find("txt_HighScoreMsg") != null)
                GameObject.Find("txt_HighScoreMsg").GetComponent<Text>().text = message;
            newScoreName.SetActive(true);
            if (createHighScoresDisplayForMe || newScoreName != null)
            {
                string acceptedInput = "abcdefghijklmnopqrstuvwxyz";
                if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
                {
                    if (newScoreName.GetComponent<Text>().text.Length > 1)
                        newScoreName.GetComponent<Text>().text = newScoreName.GetComponent<Text>().text.Substring(0, newScoreName.GetComponent<Text>().text.Length - 1);
                    else if (newScoreName.GetComponent<Text>().text.Length == 1)
                        newScoreName.GetComponent<Text>().text = "";
                    UpdateScoreDisplay();
                }
                else if (Input.anyKeyDown)
                {

                    if (newScoreName.GetComponent<Text>().text == defaultHighScoreName)
                        newScoreName.GetComponent<Text>().text = "";
                    bool allowed = false;
                    if (newScoreName.GetComponent<Text>().text.Length < 4)
                    {
                        for (int i = 0; i < acceptedInput.Length; i++)
                        {
                            if (Input.inputString != null && Input.inputString != "")
                            {
                                if (Input.inputString[0].ToString().ToLower() == acceptedInput[i].ToString())
                                    allowed = true;
                            }
                        }
                        if (allowed)
                        {
                            newScoreName.GetComponent<Text>().text += Input.inputString[0];
                        }
                    }
                    UpdateScoreDisplay();
                }
            }

            if (createHighScoresDisplayForMe)
            {
                GameObject.Find("txt_HighScoreMsg").GetComponent<Text>().text = message;
            }
        }
        else
        {
            if (newScoreName != null)
            {
                if(isIPF)
                {
                    newScoreName.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    newScoreName.SetActive(false);
                }
                
            }
            message = "GAME OVER";
            if (GameObject.Find("txt_HighScoreMsg") != null)
                GameObject.Find("txt_HighScoreMsg").GetComponent<Text>().text = message;
        }
        #endregion


        #region "only use one of these two"    
        if (Input.GetButtonDown("Quit"))
        {
            QuitGame();
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    QuitGame();
        //}
        #endregion

    }

    void OneScene()
    {
        if (gameScreen)
        {
            GameScreen();
        }
        else if (highScoresScreen)
        {
            HighScoresScreen();
        }
        else if(titleScreen)
        {
            if (debugMode && Input.GetKeyDown(KeyCode.F10))
                PlayGame();
        }
    }

    void GameScreen()
    {
        string gameOverMsg = "Game Over!";
        checkIfOver();

        if (debugMode && Input.GetKey(KeyCode.F10))
        {
            score++;
        }

        #region "only use one of these two"  
        if (Input.GetButtonDown("Quit") && isPaused)
        {
            PauseGame(false);
            SceneManager.LoadScene(sceneNames[sceneNames.Count - 1]);
        }

        //if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        //{
        //    PauseGame(false);
        //    if(!isOneScene)
        //        SceneManager.LoadScene(sceneNames[sceneNames.Count - 1]);
        //}

        #endregion

        #region "Pause Functionality - only use one"


        if (Input.GetButtonDown("Pause") || Input.GetButtonDown("Quit"))
        {
            if (isPaused)
                PauseGame(false);
            else
                PauseGame(true);
        }

        //if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (isPaused)
        //        PauseGame(false);
        //    else
        //        PauseGame(true);
        //}

        #endregion

        if (createHighScoresDisplayForMe && newContainer != null)
        {
            newContainer.SetActive(false);
        }

        if (usesLegacyScoreDisplay)
        {
            GameObject aCanvas = CreateCanvas();
            if (legacyContainer == null)
            {
                legacyContainer = new GameObject("GeneratedScoreInfo", typeof(RectTransform));

                legacyContainer.transform.SetParent(aCanvas.transform);
                legacyContainer.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                legacyContainer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                legacyContainer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                legacyContainer.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                legacyContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                legacyContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;

                legacyContainer.transform.localScale = Vector3.one;
            }




            if (GameObject.Find("txt_Score") == null)
            {
                CreateTextBox("txt_Score", legacyContainer.transform, "Score" + score, displayfont, Color.white, 30,
                    new Vector2(0, 1), new Vector2(0, 1), Vector3.one, new Vector2(0, 1), new Vector3(10, -10, 0), new Vector2(200, 100),
                    Vector3.zero, TextAnchor.UpperLeft);

            }
            GameObject.Find("txt_Score").GetComponent<Text>().text = "Score: " + score;
        }

        if (usesLegacyTimer)
        {
            GameObject aCanvas = CreateCanvas();
            if(legacyContainer == null)
            {
                legacyContainer = new GameObject("GeneratedScoreInfo", typeof(RectTransform));

                legacyContainer.transform.SetParent(aCanvas.transform);
                legacyContainer.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                legacyContainer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                legacyContainer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                legacyContainer.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                legacyContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                legacyContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                legacyContainer.transform.localScale = Vector3.one;
            }


            if (GameObject.Find("txt_TimeDisplay") == null)
            {
                CreateTextBox("txt_TimeDisplay", legacyContainer.transform, "Time: " + score, displayfont, Color.white, 50,
                    new Vector2(.5f, 1), new Vector2(.5f, 1), Vector3.one, new Vector2(.5f, 1), new Vector3(10, -10, 0), new Vector2(400, 100),
                    Vector3.zero, TextAnchor.UpperCenter);
            }
            if (guiTime > 0)
            {
                GameObject.Find("txt_TimeDisplay").GetComponent<Text>().text = "Time: " + Mathf.RoundToInt(guiTime).ToString();
            }
            else if (guiTime <= 0 && GameObject.Find("txt_TimeDisplay").GetComponent<Text>().text != gameOverMsg)
            {
                GameObject.Find("txt_TimeDisplay").GetComponent<Text>().text = gameOverMsg;
            }
        }
    }

    void TitleScreen()
    {
        if (debugMode && Input.GetKeyDown(KeyCode.F10))
            PlayGame();
    }

    void AlwaysOnScript()
    {
        if (usesApplicationTimeOut)
        {
            #region "Timeout Scripts"
            float timeToTimeout = 240;
            if (!Input.anyKeyDown && Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
            {
                timeTicker += Time.deltaTime;
                if (timeTicker > timeToTimeout && SceneManager.GetActiveScene().name != sceneNames[0])
                {
                    if (highScore)
                    {
                        highScoreData.WriteData();
                    }
                    timeTicker = 0;
                    SceneManager.LoadScene(sceneNames[0]);
                }
                else if (timeTicker > timeToTimeout && SceneManager.GetActiveScene().name == sceneNames[0])
                {
#if UNITY_EDITOR
                    if (Application.isEditor)
                    {
                        if (highScore)
                        {
                            highScoreData.WriteData();
                        }
                        EditorApplication.isPlaying = false;
                    }
#endif
                    if (!Application.isEditor)
                    {
                        if (highScore)
                        {
                            highScoreData.WriteData();
                        }
                        Application.Quit();
                    }

                }
            }
            else
            {
                timeTicker = 0;
            }
            #endregion
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!isOneScene)
        { 
            //This is the last scene in the array listed above
            //if you edit this, it should be your high scores screen.
            if (SceneManager.GetActiveScene().name == sceneNames[sceneNames.Count - 1])
            {
                HighScoresScreen();
            }
            else if (SceneManager.GetActiveScene().name != sceneNames[0] && SceneManager.GetActiveScene().name != sceneNames[sceneNames.Count - 1])
            {
                GameScreen();
            }
            else if (SceneManager.GetActiveScene().name == sceneNames[0])
            {
                TitleScreen();
            }
            AlwaysOnScript();
        }
        else
        {
            OneScene();
            AlwaysOnScript();
        }
    }

    private void CreateTextBox(string _name, Transform _parent, string _text, Font _font, Color _color, int _fontsize,
                               Vector2 _aMin, Vector2 _aMax, Vector3 _scale, Vector2 _pivots, Vector3 _position, Vector2 _size,
                               Vector3 _offset, TextAnchor _alignment)
    {
        GameObject newTextBox = new GameObject(_name, typeof(Text));
        RectTransform nTB_rt = newTextBox.GetComponent<RectTransform>();
        newTextBox.transform.SetParent(_parent);

        newTextBox.GetComponent<Text>().color = _color;
        newTextBox.GetComponent<Text>().fontSize = _fontsize;
        newTextBox.GetComponent<Text>().font = _font;
        newTextBox.GetComponent<Text>().text = _text;
        newTextBox.GetComponent<Text>().alignment = _alignment;
        newTextBox.transform.localScale = _scale;

        nTB_rt.anchorMin = _aMin;
        nTB_rt.anchorMax = _aMax;
        nTB_rt.pivot = _pivots;
        nTB_rt.anchoredPosition3D = _position + _offset;
        nTB_rt.sizeDelta = _size;
    }

    private void CreateHighScoresDisplay()
    {
        GameObject newCanvas = CreateCanvas();
        newContainer = new GameObject("GeneratedHighScores", typeof(RectTransform));
        
        newContainer.transform.SetParent(newCanvas.transform);
        newContainer.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        newContainer.GetComponent<RectTransform>().anchorMin = new Vector2(0,0);
        newContainer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        newContainer.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
        newContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        newContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        
        newContainer.transform.localScale = Vector3.one;

        //newContainer
        Vector3 offset = new Vector3(displayXOffset, -displayYOffset,0);

        CreateCamera();
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject newEventMan = new GameObject("EventSystem", typeof(EventSystem));
        }

        CreateTextBox("txt_HighNames", newContainer.transform, highScoreData.ScoreNames(), displayfont, Color.white, 30,
            new Vector2(0, 1), new Vector2(0, 1), Vector3.one, new Vector2(0, 1), new Vector3(10, -80, 0), new Vector2(100, 200),
            offset, TextAnchor.UpperLeft);
        CreateTextBox("txt_HighScore", newContainer.transform, highScoreData.ScoreScores(), displayfont, Color.white, 30,
            new Vector2(0, 1), new Vector2(0, 1), Vector3.one, new Vector2(0, 1), new Vector3(130, -80, 0), new Vector2(200, 200),
            offset, TextAnchor.UpperRight);
        CreateTextBox("txt_HighScoresTitle", newContainer.transform, "High Scores", displayfont, Color.white, 30,
            new Vector2(0, 1), new Vector2(0, 1), Vector3.one, new Vector2(0, 1), new Vector3(10, -10, 0),
            new Vector2(GameObject.Find("txt_HighNames").GetComponent<RectTransform>().sizeDelta.x +
                        GameObject.Find("txt_HighScore").GetComponent<RectTransform>().sizeDelta.x + 20 , 70),
            offset, TextAnchor.MiddleCenter);
        CreateTextBox("ipf_newScore", newContainer.transform, defaultHighScoreName, displayfont, Color.white, 30,
            new Vector2(0, 1), new Vector2(0, 1), Vector3.one, new Vector2(0, 1), new Vector3(10, -360, 0),
            new Vector2(GameObject.Find("txt_HighNames").GetComponent<RectTransform>().sizeDelta.x +
                        GameObject.Find("txt_HighScore").GetComponent<RectTransform>().sizeDelta.x + 20, 80),
            offset, TextAnchor.MiddleCenter);
        CreateTextBox("txt_HighScoreMsg", newContainer.transform, "Game Over", displayfont, Color.white, 10,
            new Vector2(0, 1), new Vector2(0, 1), Vector3.one, new Vector2(0, 1), new Vector3(10, -320, 0),
            new Vector2(GameObject.Find("txt_HighNames").GetComponent<RectTransform>().sizeDelta.x +
                        GameObject.Find("txt_HighScore").GetComponent<RectTransform>().sizeDelta.x + 20, 80),
            offset, TextAnchor.MiddleCenter);
        newScoreName = GameObject.Find("ipf_newScore");
        HighScoresNameBox = GameObject.Find("txt_HighNames");
        HighScoresScoreBox = GameObject.Find("txt_HighScore");
    }

    private GameObject CreateCamera()
    {
        GameObject result = null;
        if (FindObjectsOfType<Camera>().Length == 0)
        {
            GameObject newCamera = new GameObject("Camera", typeof(Camera));
            newCamera.tag = "MainCamera";
        }
        else
        {
            result = FindObjectOfType<Camera>().gameObject;
            result.tag = "MainCamera";
        }
        return result;
    }

    private GameObject CreateCanvas()
    {
        GameObject result;
        Canvas newCan;
        if (FindObjectOfType<Canvas>() == null)
        {
            result = new GameObject("Canvas", typeof(Canvas));
            newCan = result.GetComponent<Canvas>();
            newCan.renderMode = RenderMode.ScreenSpaceCamera;
            newCan.GetComponent<Canvas>().worldCamera = Camera.main;
        }
        else
        {
            result = FindObjectOfType<Canvas>().gameObject;
        }
        return result;
    }
    
    public void UpdateScoreDisplay()
    {
        string newName="";
        if (newScoreName != null && (createHighScoresDisplayForMe || usesCustomCanvasHighScoreDisplay))
        {
            newName = newScoreName.GetComponent<Text>().text;
        }
        else if (newScoreName.GetComponent<Text>() == null)
        {
            //get the first child that has a text component?
            newName = newScoreName.GetComponentInChildren<Text>().text;
            //newName = newScoreName.transform.Find("Text").GetComponent<Text>().text;
        }

        if (newName == "")
            newName = defaultHighScoreName;
 
        highScoreData.RewriteSingleScore(newName, sIndex);

        ScoresDisplayUpdate();
    }

    private void ScoresDisplayUpdate()
    {
        if (HighScoresNameBox == null)
        {
            if (GameObject.Find("txt_HighNames") != null)
                GameObject.Find("txt_HighNames").GetComponent<Text>().text = highScoreData.ScoreNames();
        }
        if (HighScoresScoreBox == null)
        {
            if (GameObject.Find("txt_HighScore") != null)
                GameObject.Find("txt_HighScore").GetComponent<Text>().text = highScoreData.ScoreScores();
        }
        if (usesCustomCanvasHighScoreDisplay || createHighScoresDisplayForMe)
        {
            if (HighScoresNameBox != null)
            {
                HighScoresNameBox.GetComponent<Text>().text = highScoreData.ScoreNames();
            }
            if (HighScoresScoreBox != null)
            {
                HighScoresScoreBox.GetComponent<Text>().text = highScoreData.ScoreScores();
            }
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            if (highScore)
            {
                highScoreData.WriteData();
                highScore = false;
            }
            score = 0;
            EditorApplication.isPlaying = false;
        }
#endif
        if (!Application.isEditor)
        {
            if (highScore)
            {
                highScoreData.WriteData();
                highScore = false;
            }
            score = 0;
            Application.Quit();
        }
    }

    public void ResetGame()
    {
        alreadyChecked = false;
        //PauseGame(false); //shouldn't be needed
        if(highScore)
        {
            print("hi");
            highScoreData.WriteData();
            highScore = false;
        }
        score = 0;
        if(usesLegacyTimer)
            guiTime = gameCountDownTime;
    }

    public void PlayGame()
    {
        ResetGame();
        if(!isOneScene)
            SceneManager.LoadScene(sceneNames[1]);
        else
        {
            if(newContainer != null)
            {
                if(newContainer.activeInHierarchy)
                {
                    newContainer.SetActive(false);
                }
            }
            titleScreen = false;
            highScoresScreen = false;
            gameScreen = true;
        }
    }

    void checkIfOver()
    {
        if (usesLegacyTimer && !isPaused)
        {
            guiTime -= Time.deltaTime;
            if (guiTime <= -gameOverDisplayTime)
            {
                if(!isOneScene)
                    SceneManager.LoadScene(sceneNames[sceneNames.Count - 1]);
                else
                {
                    highScoresScreen = true;
                    gameScreen = false;
                    titleScreen = false;
                }
            }
        }
    }

}
