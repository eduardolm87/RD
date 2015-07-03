using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    #region StageInfo

    public class StageInfo
    {
        public Stage Stage = null;

        public bool Unlocked = false;

        public int TimesPlayed = 0;
        public int TimesCompleted = 0;
        public int BestTimeInSeconds = -1;






        public StageInfo() { }
        public StageInfo(Stage stg)
        {
            this.Stage = stg;
        }
        public StageInfo(string serializedStage)
        {
            string _stgName = Detag(Tag_Name, serializedStage);
            Stage = GameManager.Instance.Collections.StagesInGame.FirstOrDefault(x => x.Name == _stgName);
            if (Stage == null)
            {
                Debug.LogError("Error reading a stage from saved data: " + _stgName + " does not exist.");
            }

            Unlocked = System.Convert.ToBoolean(Detag(Tag_Unlocked, serializedStage));
            TimesPlayed = System.Convert.ToInt32(Detag(Tag_TimesPlayed, serializedStage));
            TimesCompleted = System.Convert.ToInt32(Detag(Tag_TimesCompleted, serializedStage));
            BestTimeInSeconds = System.Convert.ToInt32(Detag(Tag_BestTime, serializedStage));
        }

        public override string ToString()
        {
            return (Stage.Name ?? "???") + "(" + (Unlocked ? "unlocked" : "locked") + ")";
        }
    }

    #endregion





    const string K_Stages = "K_Stages";
    const string K_Heroes = "K_Heroes";
    const string K_CurrentHero = "K_C_Hero";
    const string K_Settings_Music = "K_Settings_Music";
    const string K_Settings_SFX = "K_Settings_SFX";
    const string K_LastVisitedStage = "K_LastVisitedStage";


    const string Tag_Name = "name";
    const string Tag_Unlocked = "unl";
    const string Tag_BestTime = "btime";
    const string Tag_TimesPlayed = "tplay";
    const string Tag_TimesCompleted = "twin";
    const string StageSerialSeparator = "<%STG%>";
    const string HeroSerialSeparator = "<%HRO%>";


    public void ReadProgress()
    {
        if (IsFirstTime())
        {
            FirstTime();
        }

        ReadHeroes();

        ReadStages();

        ReadLastVisitedStage();

        ReadSettings();
    }

    public void SaveProgress()
    {
        SaveHeroes();

        SaveStages();

        SaveLastVisitedStage();

        SaveSettings();

        SaveVersion();
    }

    public void SaveLastVisitedStage()
    {
        if (GameManager.Instance.lastLoadedStagePrefab != null)
        {
            PlayerPrefs.SetString(K_LastVisitedStage, GameManager.Instance.lastLoadedStagePrefab.name);
        }
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance.Run.Reset();
        FirstTime();
    }


    bool IsFirstTime()
    {
        if (!PlayerPrefs.HasKey("VERSIONSTRING"))
            return true;

        string versionString = PlayerPrefs.GetString("VERSIONSTRING");

        if (versionString == GameManager.VERSIONSTRING)
            return false;

        //We can make a more detailed port from one version to the next here
        return true;
    }

    void DeleteAnyPreviousData()
    {
        PlayerPrefs.DeleteAll();
    }

    void FirstTime()
    {
        DeleteAnyPreviousData();
        GameManager.Instance.Run.Reset();
        SaveProgress();
        Debug.Log("Created data for the first time");
    }

    void ReadLastVisitedStage()
    {
        if (PlayerPrefs.HasKey(K_LastVisitedStage))
        {
            string lastVisitedStageName = PlayerPrefs.GetString(K_LastVisitedStage);

            Stage lastVisitedStage = GameManager.Instance.Run.UnlockedStages.FirstOrDefault(s => s.name == lastVisitedStageName);
            if (lastVisitedStage != null)
            {
                GameManager.Instance.lastLoadedStagePrefab = lastVisitedStage;
            }
        }
    }

    void ReadStages()
    {
        GameManager.Instance.Run.UnlockedStages.Clear();

        string _completeStagesList = PlayerPrefs.GetString(K_Stages);

        List<string> _stagesSerialList = _completeStagesList.Split(new string[] { StageSerialSeparator }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

        foreach (string _serializedStage in _stagesSerialList)
        {
            string stageName = Detag(Tag_Name, _serializedStage);

            Stage stageDeserialized = GameManager.Instance.Collections.StagesInGame.FirstOrDefault(s => s.name == stageName);

            if (stageDeserialized != null)
            {
                GameManager.Instance.Run.UnlockedStages.Add(stageDeserialized);
            }
            else
            {
                //stage not found. Deprecated?
            }
        }
    }

    void ReadHeroes()
    {
        GameManager.Instance.Run.UnlockedHeroes.Clear();
        List<string> heroesNamesList = PlayerPrefs.GetString(K_Heroes).Split(new string[] { HeroSerialSeparator }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

        foreach (string serHeroName in heroesNamesList)
        {
            string heroname = Detag(Tag_Name, serHeroName);
            Hero hero = GameManager.Instance.Collections.HeroesInGame.FirstOrDefault(h => h.name == heroname);
            if (hero != null)
            {
                GameManager.Instance.Run.UnlockedHeroes.Add(hero);
            }
            else
            {
                //Hero not found. deprecated?
            }
        }


        string currentHeroDs = PlayerPrefs.GetString(K_CurrentHero);
        currentHeroDs = Detag(Tag_Name, currentHeroDs);
        Hero currentHero = GameManager.Instance.Run.UnlockedHeroes.FirstOrDefault(h => h.name == currentHeroDs);
        if (currentHero != null)
        {
            GameManager.Instance.Run.CurrentHero = currentHero;
        }

    }

    void SaveVersion()
    {
        PlayerPrefs.SetString("VERSIONSTRING", GameManager.VERSIONSTRING);
    }

    void SaveHeroes()
    {
        string _completeSerialization = "";

        foreach (Hero hero in GameManager.Instance.Run.UnlockedHeroes)
        {
            _completeSerialization += SerializeHero(hero);
            _completeSerialization += HeroSerialSeparator;
        }

        PlayerPrefs.SetString(K_Heroes, _completeSerialization);


        string currentHero = "";
        if (GameManager.Instance.Run.CurrentHero != null)
            currentHero = GameManager.Instance.Run.CurrentHero.name;

        PlayerPrefs.SetString(K_CurrentHero, Intag(Tag_Name, currentHero));
    }

    public void SaveStages()
    {
        string _completeSerialization = "";

        foreach (Stage _stg in GameManager.Instance.Collections.StagesInGame)
        {
            if (GameManager.Instance.Run.UnlockedStages.Any(s => s.name == _stg.name))
            {
                //Stage is unlocked
                _completeSerialization += SerializeStage(_stg);
                _completeSerialization += StageSerialSeparator;
            }
        }

        if (_completeSerialization.EndsWith(StageSerialSeparator))
        {
            _completeSerialization = _completeSerialization.Remove(_completeSerialization.Length - StageSerialSeparator.Length);
        }

        PlayerPrefs.SetString(K_Stages, _completeSerialization);
    }

    string SerializeStage(Stage stg)
    {
        string _s = "";

        _s += Intag(Tag_Name, stg.name);

        return _s;
    }

    string SerializeHero(Hero zHero)
    {
        string _s = "";

        _s += Intag(Tag_Name, zHero.name);
        //Serialize upgrades here

        return _s;
    }

    void ReadSettings()
    {
        GameManager.Instance.MuteMusic = PlayerPrefs.GetInt(K_Settings_Music) == 0 ? false : true;
        GameManager.Instance.MuteSFX = PlayerPrefs.GetInt(K_Settings_SFX) == 0 ? false : true;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(K_Settings_Music, GameManager.Instance.MuteMusic ? 1 : 0);
        PlayerPrefs.SetInt(K_Settings_SFX, GameManager.Instance.MuteSFX ? 1 : 0);
    }

    public static string Intag(string tag, string msg)
    {
        return "<" + tag + ">" + msg + "</" + tag + ">";
    }

    public static string Detag(string tag, string emsg)
    {
        return emsg.Split(new string[] { "<" + tag + ">" }, System.StringSplitOptions.None)[1].Split(new string[] { "</" + tag + ">" }, System.StringSplitOptions.None)[0];
    }

}
