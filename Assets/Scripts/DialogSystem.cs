﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogSystem : MonoBehaviour
{

  // singleton

  public static DialogSystem Instance;

  void Awake() {
    Instance = this;
    dialogQueue = new ArrayList();
  }


  private static ArrayList dialogQueue;

  // read-only states of dialog box
  private static bool dialogBoxVisible = false;

  // array containg all roboter icons as sprites
  private static Sprite[] dialogIcons = null;

  // elements of the dialog box
  private static Animator animator = null;
  private static AudioSource audioSource = null;
  private static GameObject dialogBox = null;
  private static GameObject textElement = null;
  private static GameObject panelElement = null;
  private static Image iconElement = null;



  void Start() {

    Debug.Log("DialogSystem: Loaded.");

    // load icons into sprite array
    dialogIcons = Resources.LoadAll<Sprite>("DialogIcons/");

    // get inner elements
    foreach (Transform child in gameObject.transform) {

      GameObject obj = child.gameObject;

      if (obj.name == "DialogBox") {

        dialogBox = obj;
        animator = dialogBox.GetComponent<Animator>();

        foreach (Transform child2 in obj.transform) {

          GameObject obj2 = child2.gameObject;

          // get parts of dialog system
          switch (obj2.name) {
            case "Background":  panelElement = obj2; break;
            case "Text":        textElement = obj2; break;
            case "Icon":        iconElement = obj2.GetComponent<Image>(); break;
            default: break;
          }

        }
        
      }
      else if (obj.name == "DialogAudioSource") {
        audioSource = obj.GetComponent<AudioSource>();
      }

    }

  }

  // adds the dialog to queue 
  // -> will be played the earliest where no other dialog is playing
  public static void LoadDialog(string name)
  {
    dialogQueue.Add(name);
  }

  private void Update() {
    if (!dialogBoxVisible && dialogQueue.Count > 0) {
      LoadDialogSettings();
    }
  }





  // load first dialog in queue into memory
  public static void LoadDialogSettings() {
    
    void setDialogue(string t, string a, string e) {

      // dialog is currently playing, break it
      if (dialogBoxVisible) {
        audioSource.Stop();
        DialogSystem.Instance.StopCoroutine(PlayDialog());
      }
      // settings for new dialogue
      text = t;
      audio_path = a;
      iconElement.sprite = getIcon(e);

      DialogSystem.Instance.StartCoroutine(PlayDialog());
    }

    // load an icon from array by key
    Sprite getIcon(string iconName) {

      // get index of icon in sprite array by key
      int getIndex() {
        switch (iconName) {
          case "neutral": return 0;
          case "happy": return 1;
          case "laughing": return 2;
          case "surprised": return 3;
          case "sleepy": return 4;
          case "annoyed": return 5;
          case "angry": return 6;
          case "furious": return 7;
          case "sad": return 8;
          default:
            Debug.Log("DialogSystem: Given dialog item " + iconName + 
                      " couldn't be found. Displaying neutral icon.");
            break;
        }
        return 0;
      }

      return dialogIcons[getIndex()];

    }



    string name = dialogQueue[0].ToString();
    dialogQueue.RemoveAt(0);

    switch (name) {

      // level 1
      case "lvl1_hello":
        setDialogue("Hello, my little friend!\nDo you have some time to help me out?", "lvl1_hello", "laughing"); break;
      case "lvl1_asleep":
        setDialogue("Is this fellow asleep perhaps?", "lvl1_asleep", "annoyed"); break;
      case "lvl1_move":
        setDialogue("Finally, that's better!\nNow that you're awake, can you walk or roll around?", "lvl1_move", "laughing"); break;
      case "lvl1_jump":
        setDialogue("Little friend, are you able to jump over that thing?", "lvl1_jump", "neutral"); break;
      case "lvl1_dont_jump_into_water":
        setDialogue("I wouldn't jump into the water if I were you.\nBecause that isn't water, and it is deadly.", "lvl1_dont_jump_into_water", "neutral"); break;
      case "lvl1_not_the_smartest_circle":
        setDialogue("You really aren't the smartest circle out there, isn't that right?", "lvl1_not_the_smartest_circle", "annoyed"); break;
      case "lvl1_quick_compared_to_other_circles":
        setDialogue("I have to compliment you! Once you finally woke up, you're actually quite quick on foot, especially in comparison to other circles!", "lvl1_quick_compared_to_other_circles", "happy"); break;
      case "lvl1_morph":
        setDialogue("No matter how fast you are, sometimes you just can't overcome an obstacle as a circle.", "lvl1_morph", "neutral"); break;

      default:
        Debug.Log("DialogSystem: Could not find dialog \"" + name + "\".");
        break;

    }

  }


  

  
  private static float audioClipLength = 0.0f;

  // text is still being written on screen
  private static string text = "",
          audio_path = "", // path to audio
          currentText = ""; // temporary, current progress of typewriter
  private static float delayBetweenChars = 0f; // calculated by audio clip and text length

  private static IEnumerator PlayDialog() {

    bool dialogBoxWasVisibleOnStart = dialogBoxVisible;

    dialogBoxVisible = true;
    animator.SetBool("ShowDialog", true);

    currentText = "";
    textElement.GetComponent<TextMeshProUGUI>().SetText("");

    AudioClip clip = Resources.Load("Dialog/" + audio_path, typeof(AudioClip)) as AudioClip;
    audioClipLength = clip.length;

    yield return new WaitForSeconds(dialogBoxWasVisibleOnStart ? 0.15f : 0.5f);

    // play roboter voice
    audioSource.PlayOneShot(clip);

    // delay in seconds after each character before next one is written
    delayBetweenChars = audioClipLength / text.Length;

    for (int i = 1; i <= text.Length; i++) {
      currentText = text.Substring(0, i);
      textElement.GetComponent<TextMeshProUGUI>().SetText(currentText);
      yield return new WaitForSeconds(delayBetweenChars);
    }

    yield return new WaitForSeconds(.4f);
    animator.SetBool("ShowDialog", false);

    yield return new WaitForSeconds(0.3f);
    textElement.GetComponent<TextMeshProUGUI>().SetText("");
    dialogBoxVisible = false;
    DialogSystem.Instance.StopCoroutine(PlayDialog());

  }

}