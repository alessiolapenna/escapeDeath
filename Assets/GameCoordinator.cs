using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCoordinator : MonoBehaviour
{
    bool animating = false;
    Player player;
    Sword sword;
    GameObject uiSword;
    Enemy enemy;
    CursorController cursorController;
    bool playerDied = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        sword = FindObjectOfType<Sword>();
        enemy = FindObjectOfType<Enemy>();
        cursorController = FindObjectOfType<CursorController>();
        uiSword = GameObject.FindWithTag("UISword");
    }

    private void Start()
    {
        UpdateTilesStatus();
    }

    public void MovePlayerTo(Tile tile)
    {
        if (!animating && !playerDied)
        {
            animating = true;
            player.MoveTo(tile, PlayerMovementCompleted);
        }
    }

    public void AttackTo(Tile tile)
    {
        if (!animating && !playerDied)
        {
            animating = true;
            player.AttackTo(tile, PlayerAttackCompleted);
        }
    }

    public void AttackWithoutSwordTo(GameObject tile)
    {
        if (!animating && !playerDied)
        {
            animating = true;
            enemy.AttackTo(player.currentTile, EnemyAttackCompleted);
        }
    }

    private void EnemyAttackCompleted()
    {
        playerDied = true;
        Invoke("PlayerDied", 3f);
        animating = false;
    }

    private void PlayerAttackCompleted()
    {
        UpdateTilesStatus();
        Invoke("NextLevel", 4f);
        animating = false;
    }

    private void PlayerMovementCompleted()
    {
        UpdateTilesStatus();
        player.UpdateStatus(sword);
        //was it a trap?
        if (player.currentTile.inGameStatus == TileStatus.trap)
        {
            player.currentTile.UpdateActiveStatus(TileStatus.trap);
            player.Die();
            playerDied = true;
            Invoke("PlayerDied", 3f);
        }
        animating = false;
    }

    private void UpdateTilesStatus()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles) {
            tile.UpdateStatus(player);
        }
    }

    public void PlayerPickedupSword()
    {
        uiSword.GetComponent<RawImage>().enabled = true;
    }

    void NextLevel()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneIndex % 2 == 0)
        {
            sceneIndex++;
        }
        if (sceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            sceneIndex = 0;
        }
        SceneManager.LoadScene(sceneIndex);
    }

    void PlayerDied()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        int inverseSceneIndex = InverseSceneIndex(sceneIndex);
        GlobalGameStatus.shared.userJustDied = true;
        SceneManager.LoadScene(inverseSceneIndex);
    }
     
    int InverseSceneIndex(int sceneIndex)
    {
        if (sceneIndex % 2 == 1)
        {
            return sceneIndex + 1;
        } else
        {
            return sceneIndex - 1;
        }
    }
}
