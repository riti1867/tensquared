﻿using UnityEngine;

/*
 * simple script that triggers an event 
 */

public class LVL1_HideJumpTooltip : MonoBehaviour {

  private bool playedEventAlready;

  private void Awake() {
    playedEventAlready = false;
  }

  private void OnTriggerEnter2D(Collider2D col) {
    
    if (!playedEventAlready && col.gameObject.tag == "Player") {
      playedEventAlready = true; // only load once
      ScriptedEventsManager.Instance.LoadEvent(1, "hide_jump_tooltip");
    }

  }

}
