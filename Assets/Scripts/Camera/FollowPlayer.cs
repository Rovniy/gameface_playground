using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new (0, 3f, -10f);

    private void LateUpdate()
    {
        transform.position = player.position + offset;
        transform.LookAt(player);
    }
}
