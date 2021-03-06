using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class GameScreenManager : MonoBehaviour {

    static bool s_AllowLog => true;

    public static GameScreenManager Instance { get; private set; }

    public ScreenType firstScreenToLoad = ScreenType.Home;

    public float transitionTime = 2f;

    public Easing.Style transitionEasing = Easing.Style.InOutSine;

    public GameObject previousPanel;
    public GameObject currentPanel;
    public GameObject nextPanel;

    public Transform homeScreenPrefab;
    public Transform gameScreenPrefab;
    public Transform endScreenPrefab;

    public enum ScreenType
    {
        Home, Game, End
    };

    Vector2 PanelDefaultSize => Vector2.one;
    Vector2 CurrentPanelDefaultPos => Vector2.zero;

    Vector2 PreviousPanelDefaultPos => CurrentPanelDefaultPos - Vector2.left;
    Vector2 NextPanelDefaultPos => CurrentPanelDefaultPos + Vector2.right;

    Dictionary<GameObject, RectTransform> m_PanelRects;
    Dictionary<ScreenType, Transform> m_ScreenPrefabs;

    Transform m_CurrentlyLoadedPrefab;

    private void Awake()
    {
        Assert.IsNull(Instance, "There can only be one GameScreenManager!");
        Instance = this;

        List<GameObject> panelsObjects = new List<GameObject> { previousPanel, currentPanel, nextPanel };

        Assert.IsFalse(panelsObjects.Any(o => o == null), "One of the Panels wasn't properly assigned");

        m_PanelRects = panelsObjects.ToDictionary(o => o, o => o.GetComponent<RectTransform>());

        m_ScreenPrefabs = new Dictionary<ScreenType, Transform>()
        {
            { ScreenType.Home, homeScreenPrefab },
            { ScreenType.Game, gameScreenPrefab },
            { ScreenType.End, endScreenPrefab },
        };

        Assert.IsTrue(Enum.GetValues(typeof(ScreenType)).Length == m_ScreenPrefabs.Count);
        Assert.IsFalse(m_ScreenPrefabs.Values.Any(t => t == null), "One of the screen prefabs wasn't set properly");

        m_CurrentlyLoadedPrefab = LoadScreen(firstScreenToLoad, currentPanel);
    }

    public void LoadNextScreen(ScreenType screen)
    {
        Log($"Loading screen {screen}");

        GameObject previousPrefab = m_CurrentlyLoadedPrefab.gameObject;
        m_CurrentlyLoadedPrefab = LoadScreen(screen, nextPanel);

        // move Previous panel to the far right to become Next
        SetPanelPosByAnchors(m_PanelRects[previousPanel], NextPanelDefaultPos);

        Func<float, float> easing = Easing.GetEasing(transitionEasing);

        (this).Transition(0, 1, transitionTime, (float t) =>
        {
            Vector2 offset = easing(t) * Vector2.left;
            SetPanelPosByAnchors(m_PanelRects[currentPanel], CurrentPanelDefaultPos + offset);
            SetPanelPosByAnchors(m_PanelRects[nextPanel], NextPanelDefaultPos + offset);
        }, onComplete: () => {
            // Unload old prefab
            Destroy(previousPrefab);

            // Swap games objects to their proper purpose
            GameObject savedPreviousPanel = previousPanel;
            previousPanel = currentPanel;
            currentPanel = nextPanel;
            nextPanel = savedPreviousPanel;

            // Update names to reflect their purpose
            previousPanel.name = "Previous";
            currentPanel.name = "Current";
            nextPanel.name = "Next";
        });
    }

    void SetPanelPosByAnchors(RectTransform panel, Vector2 newPos)
    {
        panel.anchorMin = newPos;
        panel.anchorMax = newPos + PanelDefaultSize;
    }

    private Transform LoadScreen(ScreenType screen, GameObject parent)
    {
        return Instantiate(m_ScreenPrefabs[screen], parent.transform);
    }

    private void Log(string message)
    {
        if (s_AllowLog)
        {
            Debug.Log("GameScreenManager: " + message);
        }
    }
}
