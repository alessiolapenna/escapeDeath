using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public enum TileStatus
{
    unknown,
    trap,
    safe,
    selectable,
    selectableWithFocus,
    notSelectableWithFocus
}

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] public TileStatus inGameStatus = TileStatus.unknown;
    [SerializeField] public TileStatus status = TileStatus.unknown;
    [SerializeField] public TileStatus activeStatus = TileStatus.unknown;

    MeshRenderer meshRenderer;
    GameCoordinator coordinator;
    CursorController cursorController;
    bool wasDiscovered = false;
    Player player;
    Sword sword;
    Enemy enemy;

    private void Awake()
    {
        GameObject cubeMesh = transform.Find("Cube").gameObject;
        meshRenderer = cubeMesh.GetComponent<MeshRenderer>();
        UpdateMaterial();
        coordinator = FindObjectOfType<GameCoordinator>();
        cursorController = FindObjectOfType<CursorController>();
        player = FindObjectOfType<Player>();
        sword = FindObjectOfType<Sword>();
        enemy = FindObjectOfType<Enemy>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        ProcessStatusOnMouseOver();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        ProcessStatusOnMouseExit();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        ProcessClickEvent();
    }

    void ProcessClickEvent()
    {
        switch (status)
        {
            case TileStatus.selectable:
                if (IsEnemyHere())
                {
                    if (player.HasSword())
                    {
                        coordinator.AttackTo(this);
                    } else
                    {
                        coordinator.AttackWithoutSwordTo(transform.gameObject);
                    }
                    cursorController.UpdateCursor(CursorType.pointer);
                }
                else
                {
                    coordinator.MovePlayerTo(this);
                    cursorController.UpdateCursor(CursorType.pointer);
                }
                break;
            default:
                break;
        }
    }

    void ProcessStatusOnMouseOver()
    {
        if (activeStatus == TileStatus.trap)
        {
            return;
        }
        switch (status)
        {
            case TileStatus.unknown:
                activeStatus = TileStatus.notSelectableWithFocus;
                break;
            case TileStatus.selectable:
                activeStatus = TileStatus.selectableWithFocus;
                break;
            default:
                break;
        }
        UpdateMaterial();
        UpdateCursor();
    }

    void ProcessStatusOnMouseExit()
    {
        if (activeStatus == TileStatus.trap)
        {
            return;
        }
        activeStatus = status;
        UpdateMaterial();
        UpdateCursor();
    }

    public void UpdateActiveStatus(TileStatus newStatus)
    {
        activeStatus = newStatus;
        UpdateMaterial();
    }

    bool IsSwordHere()
    {
        float distanceFromSword = DistanceFrom(sword.gameObject);
        bool swordIsHere = (distanceFromSword < 0.001);
        return swordIsHere && !player.HasSword();
    }

    bool IsEnemyHere()
    {
        float distanceFromEnemy = DistanceFrom(enemy.gameObject);
        return (distanceFromEnemy < 0.001);
    }

    void UpdateCursor()
    {
        //check first if I have a pickable or an enemy on myself

        if (IsSwordHere() && activeStatus == TileStatus.selectableWithFocus)
        {
            cursorController.UpdateCursor(CursorType.pickup);
        }
        else if (IsEnemyHere() && activeStatus == TileStatus.selectableWithFocus)
        {
            cursorController.UpdateCursor(CursorType.attack);
        }
        else
        {
            switch (activeStatus)
            {
                case TileStatus.selectableWithFocus:
                    cursorController.UpdateCursor(CursorType.walk);
                    break;
                default:
                    cursorController.UpdateCursor(CursorType.pointer);
                    break;
            }
        }
    }

    void UpdateMaterial()
    {
        string materialName = $"Materials/{activeStatus.ToString()}";
        meshRenderer.material = Resources.Load<Material>(materialName);
    }

    private float DistanceFrom(GameObject gameObject)
    {
        float x = Math.Abs(gameObject.transform.position.x - transform.position.x);
        float z = Math.Abs(gameObject.transform.position.z - transform.position.z);
        return Math.Abs(x + z);
    }

    public void UpdateStatus(Player player)
    {
        // calculate player distance from tile to determine state
        float distanceFromPlayer = DistanceFrom(player.gameObject);
        float xDistance = Math.Abs(player.gameObject.transform.position.x - transform.position.x);
        float zDistance = Math.Abs(player.gameObject.transform.position.z - transform.position.z);
        if (distanceFromPlayer < 0.001)
        {
            // it's the tile the player is on
            status = TileStatus.safe;
            activeStatus = TileStatus.safe;
            wasDiscovered = true;
            player.currentTile = this;
        }
        else if (xDistance - 10f < 0.001 && // we allow diagonal movement so we check both the distance separately
            zDistance - 10f < 0.001)
        {
            status = TileStatus.selectable;
            activeStatus = TileStatus.selectable;
        }
        else
        {
            if (wasDiscovered)
            {
                status = inGameStatus;
                activeStatus = inGameStatus;
            }
            else
            {
                status = TileStatus.unknown;
                activeStatus = TileStatus.unknown;
            }
        }
        UpdateMaterial();
    }
}
