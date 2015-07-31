using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RunProgress
{
    public List<Stage> UnlockedStages = new List<Stage>();
    public List<Hero> UnlockedHeroes = new List<Hero>();
    public const int NumberOfStagesInRun = 10;
    public Hero CurrentHero = null;
    public int Money = 0;

    public Stage GetLastUnlockedStage()
    {
        return UnlockedStages.Last();
    }

    public int GetStageIndex(Stage zStage)
    {
        return GameManager.Instance.Collections.StagesInGame.FindIndex(s => s.name == zStage.name);
    }

    public void Reset()
    {
        UnlockedHeroes.Clear();

        UnlockedStages.Clear();

        GetDefaultHero();

        UnlockedStages.Add(GameManager.Instance.Collections.StagesInGame.First());
    }

    public void AddNewStage()
    {
        if (UnlockedStages.Count >= NumberOfStagesInRun)
            return;

        Stage stg;

        stg = GetNewRandomStage();

        if (stg == null)
        {
            Debug.LogError("Error adding a new stage.");
            return;
        }

        UnlockedStages.Add(stg);
    }

    public void AddHero(Hero zHero)
    {
        if (!UnlockedHeroes.Any(h => h.name == zHero.name))
        {
            UnlockedHeroes.Add(zHero);
        }
    }

    Stage GetNewRandomStage()
    {
        int currentStageOrder = UnlockedStages.Count + 1;
        List<Stage> candidates = GameManager.Instance.Collections.StagesInGame.Where(stg => currentStageOrder >= stg.StageOrderFrom && currentStageOrder <= stg.StageOrderTo).ToList();

        candidates.RemoveAll(s => s.Frequency == Stage.Frequencies.NEVER);
        candidates.RemoveAll(s => UnlockedStages.Contains(s));

        if (candidates.Count < 1)
        {
            Debug.LogError("There are no more available stages!");
            return null;
        }

        int numberOfAlways = candidates.Count(s => s.Frequency == Stage.Frequencies.ALWAYS);
        if (numberOfAlways == 1)
        {
            return candidates.FirstOrDefault(s => s.Frequency == Stage.Frequencies.ALWAYS);
        }

        if (numberOfAlways > 0)
        {
            candidates.RemoveAll(x => x.Frequency != Stage.Frequencies.ALWAYS);
        }

        List<string> weightedList = new List<string>();
        candidates.ForEach(s =>
        {
            for (int i = 0; i < (int)s.Frequency; i++)
            { weightedList.Add(s.name); }
        });

        int chosen = Random.Range(0, weightedList.Count);

        return candidates.FirstOrDefault(s => s.name == weightedList[chosen]);
    }

    public void GetDefaultHero()
    {
        UnlockedHeroes.Add(GameManager.Instance.Collections.HeroesInGame.First());
        CurrentHero = UnlockedHeroes.First();
    }

    public void UnlockNewStageFrom(Stage zCompletedStage)
    {
        Debug.Log("Estoy en stage " + zCompletedStage.name);
        Debug.Log("cuyas zCompletedStage.NextStagesUnlocked tiene : " + zCompletedStage.NextStagesUnlocked.Count);

        List<Stage> stagesToUnlock = zCompletedStage.NextStagesUnlocked;

        Debug.Log("stagesToUnlock: " + string.Join(",", stagesToUnlock.ConvertAll(x => x.name).ToArray()));

        stagesToUnlock.RemoveAll(st => !GameManager.Instance.Collections.StagesInGame.Contains(st));
        stagesToUnlock.RemoveAll(st => GameManager.Instance.Run.UnlockedStages.Contains(st));

        Debug.Log("stagesToUnlock despues del remove: " + string.Join(",", stagesToUnlock.ConvertAll(x => x.name).ToArray()));


        if (stagesToUnlock.Count == 0)
        {
            Debug.Log("No stages unlocked.");
        }
        else
        {
            foreach (Stage st in stagesToUnlock)
            {
                UnlockedStages.Add(st);
                Debug.Log("Unlocked " + st.name);
            }
        }
    }

    public void ChangeMoney(int zQuantity)
    {
        Money = Mathf.Clamp(Money + zQuantity, 0, int.MaxValue);
    }

}

