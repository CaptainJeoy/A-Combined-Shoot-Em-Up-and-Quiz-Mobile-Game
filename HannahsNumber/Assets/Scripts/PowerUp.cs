using UnityEngine;

public class PowerUp : MonoBehaviour 
{
	public float Delay = 7f, DownSpeed = 3f;

	public GameManager.PowerUps ThisPowerUp;

	void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.PlayerObj.transform.position, Time.deltaTime * 3f);

		Destroy (this.gameObject, Delay);
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.transform.CompareTag("Player")) 
		{
			Player.Instance.Anim.Play ();

			switch (ThisPowerUp) 
			{
				case GameManager.PowerUps.Multiply:
					GameManager.Instance.IsMultiplier = true;
					break;
				case GameManager.PowerUps.Invincible:
					GameManager.Instance.IsInvincible = true;
					break;
				case GameManager.PowerUps.ExtraHealth:
					GameManager.Instance.IsExtraHealth = true;
					break;
				case GameManager.PowerUps.Laser:
					Player.Instance.LaserOn();
					break;
			}

			Destroy (this.gameObject);
		}
	}
}
