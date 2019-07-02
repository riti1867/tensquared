﻿using UnityEngine;

/*
 * simple script that triggers an event 
 */

public class LVL3_SafeLaserTurrets : MonoBehaviour {

  private bool playedDialogAlready;

  private void Awake() {
    playedDialogAlready = false;
  }

  private void OnTriggerEnter2D(Collider2D col) {

    if (!playedDialogAlready && col.gameObject.tag == "Player") {
      playedDialogAlready = true; // only load once
      ScriptedEventsManager.Instance.LoadEvent(3, "safe_laser_turrets");
    }

  }

}
