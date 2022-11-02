using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;

    private void OnTriggerEnter2D(Collider2D col)
    {
		if (col.transform.CompareTag("EnemyBullet") && GameManager.Instance.IsStartGame && !GameManager.Instance.IsGameOver)
		{
			Destroy(col.gameObject);
		}
	}
    private void Update()
    {
        transform.position = player.position + offset;
    }
}
