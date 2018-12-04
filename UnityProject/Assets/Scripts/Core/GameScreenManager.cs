using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameScreenManager : MonoBehaviour {

    static bool s_AllowLog => true;

    public static GameScreenManager Instance { get; private set; }

    public ScreenType ScreenToLoad = ScreenType.Home;

    public float transitionTime = 2f;

    public Easing.Style transitionEasing = Easing.Style.InOutSine;

    public GameObject Previous;
    public GameObject Current;
    public GameObject Next;

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

        List<GameObject> panelsGameObjects = new List<GameObject> { Previous, Current, Next };

        int numPanels = panelsGameObjects.Count;
        PanelRects = new Dictionary<GameObject, RectTransform>(numPanels);

        foreach (GameObject panelGO in panelsGameObjects)
        {
            Assert.IsNotNull(panelGO, $"Missing one of the {numPanels} panels gameobjects");
            PanelRects[panelGO] = panelGO.GetComponent<RectTransform>();
        }

        foreach (ScreenType screen in Enum.GetValues(typeof(ScreenType)))
        {
            Transform prefab = GetScreenPrefab(screen);
            Assert.IsNotNull(prefab, $"Please specify a {screen} screen prefab");
        }

        CurrentlyLoadedPrefab = LoadScreen(ScreenToLoad, Current);
    }

    public void LoadNextScreen(ScreenType screen)
    {
        Log($"Loading screen {screen}");

        GameObject previousPrefab = CurrentlyLoadedPrefab.gameObject;
        CurrentlyLoadedPrefab = LoadScreen(screen, Next);

        // move Previous panel to the far right to be used as Next
        PanelRects[Previous].anchoredPosition = PanelRects[Next].anchoredPosition; // TODO: DOESN'T WORK?

        Vector2 originalCurrentPanelPos = PanelRects[Current].anchoredPosition;
        Vector2 originalNextPanelPos = PanelRects[Next].anchoredPosition;
        Func<float, float> easing = Easing.GetEasing(transitionEasing);

        (this).Transition(0, 1, transitionTime, (float t) =>
        {
            Vector2 offset = easing(t) * -Screen.width * Vector2.right;
            PanelRects[Current].anchoredPosition = originalCurrentPanelPos + offset;
            PanelRects[Next].anchoredPosition = originalNextPanelPos + offset;
        }, onComplete: () => {
            // Unload old prefab
            Destroy(previousPrefab);

            // Rename every panel for their new proper use
            GameObject tmpPrevious = Previous;
            Previous = Current;
            Current = Next;
            Next = tmpPrevious;
            Previous.name = "Previous";
            Current.name = "Current";
            Next.name = "Next";
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
