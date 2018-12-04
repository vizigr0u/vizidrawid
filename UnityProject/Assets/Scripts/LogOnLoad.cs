using UnityEngine;

public class LogOnLoad : MonoBehaviour {

    public enum LogTime
    {
        Start, Awake, Both
    }

    public string message = "Object was loaded";
    public LogTime logTime = LogTime.Start;

    void Start()
    {
        if (logTime == LogTime.Start || logTime == LogTime.Both)
            Debug.Log("(Start) " + message);
	}

    private void Awake()
    {
        if (logTime == LogTime.Awake || logTime == LogTime.Both)
            Debug.Log("(Awake) " + message);
    }
}
