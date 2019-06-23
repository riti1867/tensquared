﻿using System.Collections.Generic;
using UnityEngine;

/*
 * script managing the global settings for every level
 */

public class LevelSettings : MonoBehaviour {

  // singleton
  public static LevelSettings Instance;
  void Awake() {
    Instance = this;
  }

  // player stats
  public bool canMove = true, // if player can use input to influence movement of character
              canJump = true, // if player can jump by key

              canMorphToCircle = true, // if player can change the form of the character
              canMorphToTriangle = true,
              canMorphToRectangle = true;

  // world stats
  public Vector2 worldSpawn,
                 playerSpawn;

  // objects
  public GameObject playerObject;

  // ground prefab colliders 
  // if false, ground prefabs won't use their inbuilt colliders,
  // so custom colliders can be set and used for every level
  public bool enableGroundColliders;



  private void Start() {

    // set spawn points at beginning to location of player object on entry in level
    worldSpawn = playerObject.transform.localPosition;
    playerSpawn = playerObject.transform.localPosition;

    if (enableGroundColliders) {

      GameObject[] groundPrefabs = GameObject.FindGameObjectsWithTag("GroundPrefab");

      // go through all placed ground prefabs 
      // and get all of their (and their childrens) box collider components
      foreach (GameObject prefab in groundPrefabs) {

        BoxCollider2D[] childColliders = prefab.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D col in childColliders) {
          col.enabled = true;
        }

      }

    }

  }

  public void setSetting(string name, bool value) {

    switch (name) {

      case "canMove":
        canMove = value;
        PlayerController.Instance.setSetting(name, value);
        break;

      case "canJump":
        canJump = value;
        PlayerController.Instance.setSetting(name, value);
        break;

      case "canMorphToCircle":
        canMorphToCircle = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      case "canMorphToTriangle":
        canMorphToTriangle = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      case "canMorphToRectangle":
        canMorphToRectangle = value;
        PlayerManager.Instance.setSetting(name, value);
        break;

      default:
        Debug.Log("LevelSettings: Setting " + name + " couldn't be found.");
        break;

    }

  }
  
  public void setSetting(string name, Vector2 pos) {
    switch (name) {

      case "playerSpawn":
        playerSpawn = pos;
        break;

      default:
        Debug.Log("LevelSettings: Setting " + name + " couldn't be found.");
        break;

    }
  }

}
