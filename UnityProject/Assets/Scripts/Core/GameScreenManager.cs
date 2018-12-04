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

    Vector2 PanelDefaultSize => Vector2.one;
    Vector2 CurrentPanelDefaultPos => Vector2.zero;

    Vector2 PreviousPanelDefaultPos => CurrentPanelDefaultPos - Vector2.left;
    Vector2 NextPanelDefaultPos => CurrentPanelDefaultPos + Vector2.right;

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

        // move Previous panel to the far right to become Next
        SetPanelPosByAnchors(PanelRects[PreviousPanel], NextPanelDefaultPos);

        Func<float, float> easing = Easing.GetEasing(transitionEasing);

        (this).Transition(0, 1, transitionTime, (float t) =>
        {
            Vector2 offset = easing(t) * Vector2.left;
            SetPanelPosByAnchors(PanelRects[CurrentPanel], CurrentPanelDefaultPos + offset);
            SetPanelPosByAnchors(PanelRects[NextPanel], NextPanelDefaultPos + offset);
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

    void SetPanelPosByAnchors(RectTransform panel, Vector2 newPos)
    {
        panel.anchorMin = newPos;
        panel.anchorMax = newPos + PanelDefaultSize;
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
