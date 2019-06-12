﻿using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour {

  public static LevelTimer Instance;

  public GameObject timer;
  private bool timerIsActive = false;
  private bool timerLockedIn = false;
  private float currentTimer = 0f; 
  private string currentTimerString = "00:00:000";

  void Awake() {

    Instance = this;

    // if the next level relative to this one is already unlocked
    // activate the timer for this level
    if (PlayerPrefs.HasKey("lvls_unlocked") &&
        PlayerPrefs.GetInt("lvls_unlocked") > ScriptedEventsManager.Instance.levelID) {
      timerIsActive = true;
      GetComponent<CanvasGroup>().alpha = 1f;
    }


  }

  void Update() {

    if (!timerIsActive || timerLockedIn) return;

    // increase timer and display new number
    currentTimer += Time.fixedDeltaTime;

    // max timer value 95:99:999
    if (currentTimer > 9599999f) currentTimer = 9599999f;

    timer.GetComponent<TextMeshProUGUI>().text = convertToTimerFormat();

  }

  public string convertToTimerFormat() {

    int temp = (int) (currentTimer * 1000f);

    string newTimerValue = temp.ToString();
    // pad left side with zeros
    newTimerValue = newTimerValue.PadLeft(7, '0');
    // insert colons into timer value
    newTimerValue = newTimerValue.Insert(2, ":");
    newTimerValue = newTimerValue.Insert(5, ":");

    currentTimerString = newTimerValue;

    return newTimerValue;

  }

  // save timer value in player prefs at the end of an level
  public void saveTimer() {

    if (!timerIsActive) return;

    timerLockedIn = true;

    int lvlID = ScriptedEventsManager.Instance.levelID;

    Debug.Log("LevelTimer: Saved timer " + currentTimerString + " for level " + lvlID + ".");

    PlayerPrefs.SetString("lvl" + lvlID + "_timer", currentTimerString);

  }

}
