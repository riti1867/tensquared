﻿using UnityEngine;

/*
 * defines the structure of a dialog
 */

public struct Dialog {

  /*
   * ==================
   * === ATTRIBUTES ===
   * ==================
   */

  // in which level dialogue is playing
  public int level;

  // text attributes
  public string text;
  public int textLength;
  
  // audio attributes
  public AudioClip audioClip;
  public float audioClipLength;

  // visual information
  public string icon;
  public bool isEvil;

  public Dialog(int definedLevel) {
    level = definedLevel;
    text = "";
    textLength = 0;
    audioClip = null;
    audioClipLength = 0f;
    icon = "";
    isEvil = false;
  }





  /*
   * ==============
   * === SETTER ===
   * ==============
   */

  public void setText(params string[] text_) {

    /*
     * sets the text for the dialog with line breaks,
     * and counts the text length
     */

    text = "";
    for (int i = 0; i < text_.Length; i++) {
      text += text_[i];

      // add new line
      if (i  < text_.Length - 1) {
        text += "\n";
      }
    }

    textLength = text.Length;

  }

  public void setAudioClip(string path) {

    /*
     * sets the audio clip for the dialog,
     * and determines the length of the audioclip
     */

    audioClip = Resources.Load("Dialog/" + path, typeof(AudioClip)) as AudioClip;
    audioClipLength = audioClip.length;

  }

}