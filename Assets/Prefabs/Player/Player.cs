using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void AnimationCompletedCallback();

enum PlayerStatus
{
    emptyhanded,
    hasSword
}

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;
    [SerializeField] public Tile currentTile;
    [SerializeField] PlayerStatus status = PlayerStatus.emptyhanded;
    GameCoordinator coordinator;
    Enemy enemy;
    AudioSource audioSource;
    [SerializeField] AudioClip walkAudioClip;
    [SerializeField] AudioClip attackAudioClip;
    [SerializeField] AudioClip dieAudioClip;

    private void Awake()
    {
        coordinator = FindObjectOfType<GameCoordinator>();
        enemy = FindObjectOfType<Enemy>();
        audioSource = GetComponent<AudioSource>();
    }

    public void MoveTo(Tile tile,
        AnimationCompletedCallback completionHandler)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = walkAudioClip;
        audioSource.Play();
        StartCoroutine(AnimateMoveToTile(tile, completionHandler));
    }

    public void GetHit()
    {
        Invoke("PerformGetHit", 0.7f);
    }

    void PerformGetHit()
    {
        GetComponent<Animator>().SetTrigger("hit");
    }

    public bool HasSword()
    {
        return status == PlayerStatus.hasSword;
    }

    public void AttackTo(Tile tile,
        AnimationCompletedCallback completionHandler)
    {
        StartCoroutine(AnimateAttackToTile(tile, completionHandler));
    }

    IEnumerator AnimateMoveToTile(Tile tile,
        AnimationCompletedCallback completionHandler)
    {
        Vector3 startPosition = transform.parent.position;
        Vector3 endPosition = tile.transform.position;
        float travelPercent = 0;

        transform.LookAt(tile.transform.position);
        GetComponent<Animator>().SetBool("move", true);

        while (travelPercent < 1)
        {
            travelPercent += speed * Time.deltaTime;
            transform.parent.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
            yield return new WaitForEndOfFrame();
        }
        GetComponent<Animator>().SetBool("move", false);
        currentTile = tile;
        completionHandler();
    }

    IEnumerator AnimateAttackToTile(Tile tile,
       AnimationCompletedCallback completionHandler)
    {
        Vector3 startPosition = transform.parent.position;
        Vector3 endPosition = new Vector3(transform.parent.position.x + ((tile.transform.position.x - transform.position.x) / 2),
            tile.transform.position.y,
            transform.parent.position.z + ((tile.transform.position.z - transform.parent.position.z) / 2));
        float travelPercent = 0;

        transform.LookAt(tile.transform.position);
        GetComponent<Animator>().SetBool("move", true);

        while (travelPercent < 1)
        {
            travelPercent += speed * Time.deltaTime;
            transform.parent.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
            yield return new WaitForEndOfFrame();
        }
        GetComponent<Animator>().SetBool("move", false);
        GetComponent<Animator>().SetTrigger("attack");
        Invoke("PlayAttackAudio", 0.8f);
        currentTile = tile;
        enemy.GetHit();
        completionHandler();
    }

    void PlayAttackAudio()
    {
        audioSource.clip = attackAudioClip;
        audioSource.Stop();
        audioSource.Play();
    }

    public void UpdateStatus(Sword sword)
    {
        switch (status)
        {
            case PlayerStatus.emptyhanded: 
                // should pick up sword?
                // calculate player distance from sword to determine state
                float x = Math.Abs(sword.transform.position.x - transform.parent.position.x);
                float z = Math.Abs(sword.transform.position.z - transform.parent.position.z);
                if (Math.Abs(x + z) < 0.001)
                {
                    status = PlayerStatus.hasSword;
                    sword.Hide();
                    coordinator.PlayerPickedupSword();
                    UpdateMesh();
                }
                break;
            default:
                break;
        }
    }

    private void UpdateMesh()
    {
        GameObject sword = GameObject.FindGameObjectWithTag("Sword");
        MeshRenderer mesh = sword.GetComponent<MeshRenderer>();
        switch (status) {
            case PlayerStatus.emptyhanded:
                mesh.enabled = false;
                break;
            case PlayerStatus.hasSword:
                mesh.enabled = true;
                break;
        }
    }

    public void Die()
    {
        GetComponent<Animator>().SetTrigger("die");
        PlayDieAudio();
    }

    void PlayDieAudio()
    {
        audioSource.clip = dieAudioClip;
        audioSource.Stop();
        audioSource.Play();
    }
}
