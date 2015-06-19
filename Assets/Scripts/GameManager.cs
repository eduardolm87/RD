using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    /*
    * VERSION HISTORY:
    * No key: Old first drafts
    * RD002: June 2015
    * 
    */
    public const string VERSIONSTRING = "RD002";

    #region Pseudo-singleton

    public static GameManager Instance = null;

    void Awake()
    {
        Instance = this;
    }
    #endregion

    ///DEBUG TOOLS
    public static bool MoveWithMouseMode = true;
    public static bool UsePowerBarWithTouchDuration = true;
    public static bool ShowRotatingArrow = false;
    public static bool SwipeToChargePower = true;
    public static bool DebuggingStageMode = true;

    public bool ResetAtStart = false;
    public bool MuteMusic = false;
    public bool MuteSFX = false;
    public bool WinStagesWith1Key = true;


    [HideInInspector]
    public bool GamePaused = false;

    [HideInInspector]
    public Stage currentlyLoadedStage = null;


    [HideInInspector]
    public Stage lastLoadedStagePrefab = null;

    [HideInInspector]
    public Player currentPlayer = null;

    [HideInInspector]
    public float TimeRealplay = 0;

    public enum Window { Title, Credits, Options, StageSelect, Store, Game };
    Window currentWindow = Window.Title;


    public EffectsManager Effects;
    public HUD HUD;
    public GamePopup GamePopup;
    public SoundManager SoundManager;
    public SaveManager Progress;
    public Collections Collections;
    public RunProgress Run = new RunProgress();
    public LevelSelectMenu LevelSelectMenu;

    public GameObject GameCamera;
    public GameObject PlayerObject;
    public GameObject GameWindow;
    public GameObject SelectStageWindow;
    public GameObject TitleScreenWindow;
    public GameObject StoreWindow;
    public GameObject CreditsWindow;
    public GameObject OptionsWindow;

    public GameObject StageButtonPrefab;
    public Transform StageList;



    [HideInInspector]
    List<StageMarker> _alreadyInSceneStages = new List<StageMarker>();
    bool _loadingStageLocked = false;





    public void Start()
    {
        if (ResetAtStart)
        {
            Debug.Log("Careful! Progress won't be saved in this build!");
            Progress.ResetAll();
        }

        DebugStageSwitchoff();

        Progress.ReadProgress();

        ChangeWindow(TitleScreenWindow);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackKeyControl();
        }
    }

    void DebugStageSwitchoff()
    {
        Stage _stage = GameObject.FindObjectOfType<Stage>();
        if (_stage == null)
            return;

        if (DebuggingStageMode)
        {
            _stage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Careful! There is another Stage already loaded in the scene. Maybe a debug stage?");
        }
    }

    public IEnumerator LoadStage(Stage stg)
    {
        if (_loadingStageLocked)
        {
            Debug.LogError("Stage " + stg.Name + " won't load because another is already loading.");
            yield break;
        }

        _loadingStageLocked = true;
        lastLoadedStagePrefab = stg;

        yield return new WaitForSeconds(0.1f);

        //Curtain
        Curtain.Instance.Show();
        yield return new WaitForSeconds(Curtain.Instance.Duration);
        Curtain.Instance.Text.text = "Loading\n" + "<b>" + stg.Name + "</b>";
        yield return new WaitForSeconds(2);

        //Load the stage
        GameObject _stageObj = GameObject.Instantiate(stg.gameObject, stg.gameObject.transform.position, Quaternion.identity) as GameObject;
        _stageObj.transform.parent = GameWindow.transform;
        currentlyLoadedStage = _stageObj.GetComponent<Stage>();
        _stageObj.name = stg.Name;


        //Create the player
        GameObject _playerObj = Instantiate(PlayerObject, currentlyLoadedStage.StartPoint.position, Quaternion.identity) as GameObject;
        currentPlayer = _playerObj.GetComponent<Player>();
        _playerObj.transform.parent = _stageObj.transform;
        currentPlayer.LoadHero(Run.CurrentHero);

        //Open the game window
        ChangeWindow(GameWindow);

        Input.ResetInputAxes();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        Input.ResetInputAxes();

        //Update stats
        TimeRealplay = 0;

        LoadRandomMusic();

        //Curtain
        Curtain.Instance.Hide();
        yield return new WaitForSeconds(Curtain.Instance.Duration);

        _loadingStageLocked = false;
    }

    void LoadRandomMusic()
    {
        int _musicClips;

        do
        {
            _musicClips = Random.Range(0, SoundManager.MusicClips.Length);
        } while (SoundManager.MusicClips[_musicClips] == TitleScreenWindow.GetComponent<TitleScreen>().MusicClip);

        SoundManager.PlayMusic(SoundManager.MusicClips[_musicClips].name);
    }

    public void ChangeWindow(GameObject which)
    {
        //Enable/disable windows
        GameWindow.SetActive(which == GameWindow);
        StoreWindow.SetActive(which == StoreWindow);
        TitleScreenWindow.SetActive(which == TitleScreenWindow);
        SelectStageWindow.SetActive(which == SelectStageWindow);
        CreditsWindow.SetActive(which == CreditsWindow);
        OptionsWindow.SetActive(which == OptionsWindow);

        //Play music on title screen
        if (TitleScreenWindow.gameObject.activeInHierarchy && !MuteMusic)
        {
            if (SoundManager.MusicSource.clip != TitleScreenWindow.GetComponent<TitleScreen>().MusicClip || !SoundManager.MusicSource.isPlaying)
            {
                SoundManager.PlayMusic(TitleScreenWindow.GetComponent<TitleScreen>().MusicClip.name);
            }
        }

        //Set the window which has been opened
        if (which == GameWindow) { currentWindow = Window.Game; }
        if (which == StoreWindow) { currentWindow = Window.Store; }
        if (which == TitleScreenWindow) { currentWindow = Window.StageSelect; }
        if (which == SelectStageWindow) { currentWindow = Window.StageSelect; }
        if (which == CreditsWindow) { currentWindow = Window.Credits; }
        if (which == OptionsWindow) { currentWindow = Window.Options; }

    }

    public void OpenStageSelect()
    {
        SoundManager.StopMusic();

        DestroyCurrentScene();

        //Play music

        LevelSelectMenu.Theatre.Curtains.InstantClose();

        ChangeWindow(SelectStageWindow);

        LevelSelectMenu.Open();
    }

    void DestroyCurrentScene()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer.gameObject);

        if (currentlyLoadedStage != null)
            Destroy(currentlyLoadedStage.gameObject);
    }

    public void WinStage()
    {
        SoundManager.MusicFadeOff();

        UnlockNextStage();

        //Message & save
        //GamePopup.Show("Stage cleared!\n" + "Time: <color=" + (_newRecord ? "yellow" : "white") + ">" + SecondsToTime(_completedTime) + "</color>", new PopupButton[] { new PopupButton("Ok", () => 
        SoundManager.Play("winstage");
        GamePopup.Show("Stage cleared!\n", new PopupButton[] { new PopupButton("Ok", () => 
        { 
            //Win
            Progress.SaveProgress();

            OpenStageSelect(); 

        }) });
    }

    public void LoseStage()
    {
        SoundManager.MusicFadeOff();

        SoundManager.Play("losestage");
        GamePopup.Show("Game Over", new PopupButton[] { 
            new PopupButton("Try Again", () => 
            {
                //Try again

                DestroyCurrentScene();

                if (lastLoadedStagePrefab != null)
                {
                    StartCoroutine(LoadStage(lastLoadedStagePrefab));
                }
                else
                    Debug.LogError("Last loaded stage is null!");
                
            }), 
            new PopupButton("Back", () => 
            { 
                //Lose
                OpenStageSelect(); 

            }) });
    }

    void UnlockNextStage()
    {
        Run.AddNewStage();
    }

    public void BackFromSelectToTitle()
    {
        ChangeWindow(TitleScreenWindow);
    }

    public static string SecondsToTime(int seconds)
    {
        string _mins = Mathf.FloorToInt(seconds / 60).ToString();
        string _secs = Mathf.FloorToInt(seconds % 60).ToString();

        if (_mins.Length < 2)
            _mins = "0" + _mins;

        if (_secs.Length < 2)
            _secs = "0" + _secs;

        return _mins + ":" + _secs;
    }

    void BackKeyControl()
    {
        //Behavior of Back key depending on context
        switch (currentWindow)
        {
            case Window.Title:
                Application.Quit();
                break;

            case Window.Options:
                ChangeWindow(TitleScreenWindow);
                break;

            case Window.Credits:
                ChangeWindow(TitleScreenWindow);
                break;

            case Window.StageSelect:
                ChangeWindow(TitleScreenWindow);
                break;
        }
    }

    public string GetPauseMenuText()
    {
        return
           "<b>" + GameManager.Instance.currentlyLoadedStage.Name + "</b>" + "  " +
            "<color=orange>" + GameManager.SecondsToTime(Mathf.FloorToInt(GameManager.Instance.TimeRealplay)) + "</color>" + "\n"
            + GameManager.Instance.currentlyLoadedStage.VerboseGoal();
    }
}
