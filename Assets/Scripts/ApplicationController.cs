using UnityEngine;

public abstract class ApplicationController : MonoBehaviour {
    public abstract void RestartAlignment();
    public abstract void FinishAlignment();
    public abstract void ToggleTutorial();
    public abstract void SkipIntro();
    public abstract void ChooseSong(int songNo);
    public abstract void MuteTutorial(bool muted);
    public abstract void SetTutorialSpeed(float speed);
}
