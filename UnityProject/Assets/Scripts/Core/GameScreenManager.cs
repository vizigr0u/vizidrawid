using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class GameScreenManager : MonoBehaviour {

    static bool s_AllowLog => true;

    public static GameScreenManager Instance { get; private set; }

    public ScreenType ScreenToLoad = ScreenType.Home;

    public float transitionTime = 2f;

    public Easing.Style transitionEasing = Easing.Style.InOutSine;

    public GameObject PreviousPanel;
    public GameObject CurrentPanel;
    public GameObject NextPanel;

    Dictionary<GameObject, RectTransform> PanelRects;

    Transform CurrentlyLoadedPrefab;

    public Transform HomeScreenPrefab;
    public Transform GameScreenPrefab;
    public Transform EndScreenPrefab;

    public enum ScreenType
    {
        Home, Game, End
    };

    public Transform GetScreenPrefab(ScreenType screen)
    {
        switch (screen)
        {
            case ScreenType.Home:
                return HomeScreenPrefab;
            case ScreenType.Game:
                return GameScreenPrefab;
            case ScreenType.End:
                return EndScreenPrefab;
            default:
                Debug.LogError($"Unhandled screentype {screen}");
                return null;
        }
    }

    private void Awake()
    {
        Assert.IsNull(Instance, "There can only be one GameScreenManager!");

        Instance = this;

        List<GameObject> PanelsObjects = new List<GameObject> { PreviousPanel, CurrentPanel, NextPanel };

        Assert.IsFalse(PanelsObjects.Any(o => o == null), $"One of the Panels wasn't properly assigned");

        PanelRects = PanelsObjects.ToDictionary(o => o, o => o.GetComponent<RectTransform>());

        foreach (ScreenType screen in Enum.GetValues(typeof(ScreenType)))
        {
            Transform prefab = GetScreenPrefab(screen);
            Assert.IsNotNull(prefab, $"Please specify a {screen} screen prefab");
        }

        CurrentlyLoadedPrefab = LoadScreen(ScreenToLoad, CurrentPanel);
    }

    public void LoadNextScreen(ScreenType screen)
    {
        Log($"Loading screen {screen}");

        GameObject previousPrefab = CurrentlyLoadedPrefab.gameObject;
        CurrentlyLoadedPrefab = LoadScreen(screen, NextPanel);

        Vector2 originalCurrentPanelMin = PanelRects[CurrentPanel].anchorMin;
        Vector2 originalCurrentPanelMax = PanelRects[CurrentPanel].anchorMax;
        Vector2 originalNextPanelMin = PanelRects[NextPanel].anchorMin;
        Vector2 originalNextPanelMax = PanelRects[NextPanel].anchorMax;

        // move Previous panel to the far right to become Next
        PanelRects[PreviousPanel].anchorMin = new Vector2(1, 0);
        PanelRects[PreviousPanel].anchorMax = new Vector2(2, 1);

        Func<float, float> easing = Easing.GetEasing(transitionEasing);

        (this).Transition(0, 1, transitionTime, (float t) =>
        {
            Vector2 offset = easing(t) * Vector2.left;
            PanelRects[CurrentPanel].anchorMin = originalCurrentPanelMin + offset;
            PanelRects[CurrentPanel].anchorMax = originalCurrentPanelMax + offset;
            PanelRects[NextPanel].anchorMin = originalNextPanelMin + offset;
            PanelRects[NextPanel].anchorMax = originalNextPanelMax + offset;
        }, onComplete: () => {
            // Unload old prefab
            Destroy(previousPrefab);

            // Swap games objects to their proper purpose
            GameObject savedPreviousPanel = PreviousPanel;
            PreviousPanel = CurrentPanel;
            CurrentPanel = NextPanel;
            NextPanel = savedPreviousPanel;

            // Update names to reflect their purpose
            PreviousPanel.name = "Previous";
            CurrentPanel.name = "Current";
            NextPanel.name = "Next";
        });
    }

    private Transform LoadScreen(ScreenType screen, GameObject parent)
    {
        Transform prefab = GetScreenPrefab(screen);
        Assert.IsNotNull(prefab, $"Prefab not specified for {screen}");
        return Instantiate(prefab, parent.transform);
    }

    private void Log(string message)
    {
        if (s_AllowLog)
            Debug.Log("GameScreenManager: " + message);
    }
}
