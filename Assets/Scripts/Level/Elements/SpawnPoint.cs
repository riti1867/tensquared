﻿using System.Collections;
using UnityEngine;

/*
 * powers the 'spawnpoint' prefab
 */

public class SpawnPoint : MonoBehaviour {

  /*
   * ==================
   * === COMPONENTS ===
   * ==================
   */

  // texture object at spawn point top
  private SpriteRenderer textureObjectSR;

  // metallic arm pushing the player out of the spawnpoint
  private GameObject playerHolder;
  public Sprite playerHolderTextureOpen;
  private Sprite playerHolderTextureClosed;
  private SpriteRenderer playerHolderSR;
  private int sortingOrderPlayerHolder;
  private Vector3 playerHolderPosition;

  // little floating title above spawnpoint
  private GameObject spawnPointMessageObject;





  /*
   * ==================
   * === ATTRIBUTES ===
   * ==================
   */

  // top texture of spawnpoint 
  [SerializeField] private Sprite activatedSprite = null;
  [SerializeField] private Sprite deactivatedSprite = null;
  private int sortingOrderTexture;

  // attributes
  private Vector2 spawnCoordinates;
  private bool isActivated = false;
  private bool activeInLastFrame = false;





  /*
   * ================
   * === INTERNAL ===
   * ================
   */

  private void Awake() {

    // get inner objects contained in spawn point
    foreach (Transform child in gameObject.transform) {

      GameObject obj = child.gameObject;

      switch (obj.name) {

        case "Texture":
          textureObjectSR = obj.GetComponent<SpriteRenderer>();
          sortingOrderTexture = textureObjectSR.sortingOrder;
          break;

        case "PlayerHolder":
          playerHolder = obj;
          playerHolderPosition = obj.transform.position;
          playerHolderSR = playerHolder.GetComponentInChildren<SpriteRenderer>();
          playerHolderTextureClosed = playerHolderSR.sprite;
          sortingOrderPlayerHolder = playerHolderSR.sortingOrder;
          break;

        case "SpawnPointMessage":
          spawnPointMessageObject = obj;
          spawnPointMessageObject.SetActive(false);
          break;

        case "SpawnPointCoordinates":
          spawnCoordinates = obj.transform.position;
          break;

      }

    }

  }

  private void Update() {

    // while death animation, display textures in front of player
    // **display spawn point texture above player and player-holder (used in animateHoldingArm)
    if (isActivated && PlayerManager.Instance.getBool(Player.IS_DEAD)) {
      textureObjectSR.sortingOrder = PlayerManager.Instance
                                     .getObject(Player.Objects.TEXTURE_OBJECT)
                                     .GetComponent<SpriteRenderer>().sortingOrder + 2;
    }
    
    // only change settings once every time the spawnpoint activated or deactivated
    if (!activeInLastFrame) {

      // activate it
      if (isActivated) {
        textureObjectSR.sprite = activatedSprite;
        spawnPointMessageObject.SetActive(true);
        LevelSettings.Instance.setSetting(Level.PLAYER_SPAWN, spawnCoordinates);
        SoundController.Instance.playSound("activateSpawnpointSound");
      }
      // deactivate it
      else {
        textureObjectSR.sprite = deactivatedSprite;
        spawnPointMessageObject.SetActive(false);
      }

    }
    
    // to check if spawn point activated in this frame in next update
    activeInLastFrame = isActivated;

  }

  private void OnTriggerEnter2D(Collider2D collision) {
    
    if (!isActivated && LevelSettings.Instance.getVector2(Level.PLAYER_SPAWN).x < spawnCoordinates.x) {

      PlayerManager.Instance.setValue(Player.HAS_SPAWN_POINT, true);

      // deactivate all spawn points
      GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
      foreach (GameObject sp in spawnPoints) {
        if (sp != gameObject) {
          SpawnPoint sp_script = sp.GetComponent<SpawnPoint>();
          if (sp_script != null) {
            sp_script.isActivated = false;
          }
        }
      }

      isActivated = true;

    }

  }





  /*
   * ================
   * === EXTERNAL ===
   * ================
   */

  public void animateHoldingArm() {

    /*
     * animates the little metallic arm holding the player,
     * after they died, respawned and get pushed out of the spawnpoint
     */

    // run animation
    StopCoroutine(moveOutHoldingArm());
    StartCoroutine(moveOutHoldingArm());



    // animate the metallic arm holding the player on respawn
    IEnumerator moveOutHoldingArm() {

      playerHolderSR.sortingOrder = PlayerManager.Instance
                                    .getObject(Player.Objects.TEXTURE_OBJECT)
                                    .GetComponent<SpriteRenderer>().sortingOrder + 1;

      int steps = 50;
      float spawnPointmoveCharBy = 19f / steps;

      // move out the holding arm
      for (int i = 0; i < steps; i++) {
        playerHolder.transform.position += new Vector3(spawnPointmoveCharBy, 0f, 0f);
        yield return new WaitForSeconds(0.03f);
      }

      playerHolderSR.sprite = playerHolderTextureOpen;

      yield return new WaitForSeconds(0.8f);

      playerHolderSR.sprite = playerHolderTextureClosed;

      // move back inside much faster
      steps /= 4;
      spawnPointmoveCharBy *= 4f;

      // move it back into the tube
      for (int i = 0; i < steps; i++) {
        playerHolder.transform.position -= new Vector3(spawnPointmoveCharBy, 0f, 0f);
        yield return new WaitForSeconds(0.01f);
      }

      // if prior calculations are imprecise on Unity's side
      // also reset position of player arm back to its fixed position
      playerHolder.transform.position = playerHolderPosition;

      textureObjectSR.sortingOrder = sortingOrderTexture;
      playerHolderSR.sortingOrder = sortingOrderPlayerHolder;

      StopCoroutine(moveOutHoldingArm());

    }

  }

}