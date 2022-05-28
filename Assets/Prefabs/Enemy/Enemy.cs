using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public void GetHit()
    {
        Invoke("PerformGetHit", 0.7f);
    }

    void PerformGetHit()
    {
        GetComponent<Animator>().SetTrigger("hit");
    }

    public void AttackTo(Tile tile,
       AnimationCompletedCallback completionHandler)
    {
        StartCoroutine(AnimateAttackToTile(tile, completionHandler));
    }

    IEnumerator AnimateAttackToTile(Tile tile,
       AnimationCompletedCallback completionHandler)
    {
        Vector3 startPosition = transform.parent.position;
        Vector3 endPosition = new Vector3(transform.parent.position.x + ((tile.transform.position.x - transform.parent.position.x) / 2),
            tile.transform.position.y,
            transform.parent.position.z + ((tile.transform.position.z - transform.parent.position.z) / 2));
        float travelPercent = 0;

        transform.LookAt(endPosition);
        GetComponent<Animator>().SetBool("move", true);

        while (travelPercent < 1)
        {
            travelPercent += speed * Time.deltaTime;
            transform.parent.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
            yield return new WaitForEndOfFrame();
        }
        GetComponent<Animator>().SetBool("move", false);
        GetComponent<Animator>().SetTrigger("attack");
        player.GetHit();
        completionHandler();
    }
}
