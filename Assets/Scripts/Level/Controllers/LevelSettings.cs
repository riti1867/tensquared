﻿using System.Collections.Generic;
using UnityEngine;

/*
 * script managing the global settings for every level
 */

public class LevelSettings : MonoBehaviour {

  /*
   * =================
   * === SINGLETON ===
   * =================
   */

  public static LevelSettings Instance;

  private void Awake() {
    Instance = this;
  }





  /*
   * ================
   * === SETTINGS ===
   * ================
   */

  // id of level scene
  [SerializeField] private int levelID = 1;

  // player stats
  [SerializeField] private bool canMove = true; // if player can use input to influence movement of character
  // if player can jump with space bar
  [SerializeField] private bool canJump = true;
  // can use button '0' to self-destruct
  [SerializeField] private bool canSelfDestruct = true;
  // if player can change the form of the character
  [SerializeField] private bool canMorphToCircle = true;
  [SerializeField] private bool canMorphToTriangle = true;
  [SerializeField] private bool canMorphToRectangle = true;

  // world stats
  [SerializeField] private Vector2 worldSpawn;
  [SerializeField] private Vector2 playerSpawn;

  // ground prefab colliders 
  // if false, ground prefabs won't use their inbuilt colliders,
  // so custom colliders can be set and used for every level
  [SerializeField] private bool enableGroundColliders = false;





  /*
   * ================
   * === INTERNAL ===
   * ================
   */

  private void Start() {

    Log.Print($"Initialised level settings for level {levelID} on object {gameObject.name}.", this);

    // find player object
    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

    // set spawn points at beginning to location of player object on entry in level
    worldSpawn = playerObject.transform.localPosition;
    playerSpawn = playerObject.transform.localPosition;

    if (enableGroundColliders) {

      GameObject[] groundPrefabs = GameObject.FindGameObjectsWithTag("GroundPrefab");

      // go through all placed ground prefabs 
      // and get all of their (and their children's) box collider components
      foreach (GameObject prefab in groundPrefabs) {

        BoxCollider2D[] childColliders = prefab.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D col in childColliders) {
          col.enabled = true;
        }

      }

    }

  }





  /*
   * ================
   * === EXTERNAL ===
   * ================
   */

  public void setSetting(int name, bool value) {

    /*
     * changes a settings value to the given value
     */

    switch (name) {

      case Player.CAN_MOVE:
        canMove = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      case Player.CAN_JUMP:
        canJump = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      case Player.CAN_SELF_DESTRUCT:
        canSelfDestruct = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      case Player.CAN_MORPH_TO_CIRCLE:
        canMorphToCircle = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      case Player.CAN_MORPH_TO_TRIANGLE:
        canMorphToTriangle = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      case Player.CAN_MORPH_TO_RECTANGLE:
        canMorphToRectangle = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      default:
        Log.Warn($"Setting with the id '{name}' couldn't be found.", this);
        break;

    }

  }
  
  public void setSetting(string name, Vector2 pos) {

    /*
     * changes a settings value to the given value
     */

    switch (name) {

      case "worldSpawn":
        worldSpawn = pos;
        break;

      case "playerSpawn":
        playerSpawn = pos;
        break;

      default:
        Log.Warn($"Setting {name} couldn't be found.", this);
        break;

    }

  }

  public int getInt(string name) {

    /*
     * gets an integer variable's value
     */

    switch (name) {

      case "levelID":
        return levelID;

    }

    Log.Warn($"Setting {name} couldn't be found.", this);
    return 0;

  }

  public bool getBool(string name) {

    /*
     * gets a boolean variable's value
     */

    switch (name) {

      case "canMove":
        return canMove;

      case "canJump":
        return canJump;

      case "canSelfDestruct":
        return canSelfDestruct;

      case "canMorphToCircle":
        return canMorphToCircle;

      case "canMorphToTriangle":
        return canMorphToTriangle;

      case "canMorphToRectangle":
        return canMorphToRectangle;

      case "enableGroundColliders":
        return enableGroundColliders;

    }

    Log.Warn($"Setting {name} couldn't be found.", this);
    return false;

  }

  public Vector2 getVector2(string name) {

    /*
     * gets a vector2 variable's value
     */

    switch (name) {

      case "worldSpawn":
        return worldSpawn;

      case "playerSpawn":
        return playerSpawn;

    }

    Log.Warn($"Setting {name} couldn't be found.", this);
    return Vector2.zero;

  }

}
