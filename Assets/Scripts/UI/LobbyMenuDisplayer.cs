using UnityEngine;

public class LobbyMenuDisplayer : LowerMenuDisplayer
{
    [SerializeField] private InteriorManager interiorManager;
    [SerializeField] private HighScoreDisplayer highScoreDisplayer;

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void StartMission()
    {
        if (GameManager.Instance != null)
        {
            PlayClickSound();
            interiorManager.StartMission();
            Hide(0.5f);
        }
    }

    public void ShowHighScores()
    {
        PlayClickSound();
        highScoreDisplayer.Focus();
        Hide(0.5f);
    }
}
