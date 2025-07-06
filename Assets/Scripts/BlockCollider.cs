using Unity.Cinemachine;
using UnityEngine;

public class BlockCollider : MonoBehaviour
{
    private BoxCollider boxCol;

    void Start()
    {
        boxCol = GetComponent<BoxCollider>();
    }
    void OnCollisionStay(Collision collision)
    {
        float obstacleHeight = transform.GetComponent<BoxCollider>().bounds.max.y;
        float playerHeight = collision.transform.GetComponent<CapsuleCollider>().bounds.max.y;

        Vector3 playerPos = collision.transform.position;
        Vector3 transformPos = transform.position;

        if(playerPos.z > transformPos.z)
        {
            collision.gameObject.GetComponent<Move>().canPressD = false;
        }
        else
        {
            collision.gameObject.GetComponent<Move>().canPressA = false;
        }

        if (obstacleHeight - playerHeight < -1.0f && obstacleHeight - playerHeight > -1.85f)
        {
            if (playerPos.z > transformPos.z && !collision.gameObject.GetComponent<Move>().isLeft)
                StartCoroutine(MovePlayerOntoBox(collision.gameObject, obstacleHeight));
            else if(playerPos.z < transformPos.z && collision.gameObject.GetComponent<Move>().isLeft)
                StartCoroutine(MovePlayerOntoBox(collision.gameObject, obstacleHeight));
        }
        else if(obstacleHeight - playerHeight <= -1.85f)
        {
            collision.gameObject.GetComponent<Move>().canPressD = true;
            collision.gameObject.GetComponent<Move>().canPressA = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        collision.gameObject.GetComponent<Move>().canPressD = true;
        collision.gameObject.GetComponent<Move>().canPressA = true;
    }

    private System.Collections.IEnumerator MovePlayerOntoBox(GameObject player, float targetHeight)
    {
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = new Vector3(player.transform.position.x, boxCol.bounds.max.y - 0.01f, boxCol.bounds.min.z + 0.4f);

        float elapsedTime = 0f;
        // float duration = 0.45f; 
        float duration = (targetHeight - startPos.y)/3;

        if (startPos.z > targetPos.z)
        {
            player.GetComponent<Move>().Convey(2 * duration / 3, true);
        }
        else
        {
            player.GetComponent<Move>().Convey(2 * duration / 3, false);
        }

        while (elapsedTime < duration)
        {
            player.transform.position = (Vector3.Lerp(startPos, targetPos, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<Move>().canMove = true;
        player.GetComponent<CapsuleCollider>().isTrigger = false;
    }
}
