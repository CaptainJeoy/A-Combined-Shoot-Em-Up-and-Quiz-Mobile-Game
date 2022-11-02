using UnityEngine;
using SwiputAPI;

public class Enemy : Singleton<Enemy>
{
    public Transform[] Waypoints;

	public SpriteRenderer EnemySprite;

	public Sprite[] EnemiesFx;

	public Audio audioObj;

    public Transform Gun;

	public Player player;

	public Animation Anim;

    public Rigidbody2D EnemyRB;

	public Bullet[] EnemyBullets;

	public float BulletSpeed, MoveSpeed = 3f, PubMinDelay = 0.2f, PubMaxDelay = 0.7f, AddOneVelocityAfterSeconds = 30f;

	[HideInInspector] 
	public int EnemyHealth;

	private float DelayTime, MinDelay, MaxDelay;

	private int currentPoint = 0, bulletSelector = 0;

	float bulletSelectCounter = 10f, currentHealthPercent;

	Color defaultBoxColour;

	Material mat;

	bool runOnce;

	void Awake()
	{
		MinDelay = PubMinDelay;
		MaxDelay = PubMaxDelay;

		mat = EnemySprite.material;
		defaultBoxColour = EnemySprite.color;
	}

	void Start ()
	{
		EnemyHealth = GameManager.Instance.EnemyMaxHealth;

		runOnce = false;

		if (!GameManager.Instance.isFirstTime)
			bulletSelector = Random.Range (0, 1);
		else
			bulletSelector = 0;
	}

    void Update()
    {
		if (GameManager.Instance.HasAnybodyWon == GameManager.WhoWon.PlayerWon) 
			return;
		
		Patrol ();
		HealthUpdate ();

		if (Time.time > DelayTime && (GameManager.Instance.IsStartGame && !GameManager.Instance.IsGameOver))
		{
			DelayTime = Time.time + Random.Range (MinDelay, MaxDelay);
			Shoot ();
			audioObj.PlayGun();
		}

		bulletSelectCounter -= Time.deltaTime;

		currentHealthPercent = ((float)EnemyHealth / (float)GameManager.Instance.EnemyMaxHealth) * 100f;
    }

    void Patrol()
    {
        if (transform.position == Waypoints[currentPoint].position)
        {
			int tempRandPoint = Swiput.GenerateRandomNumberBetweenWithIgnoreNum (0, Waypoints.Length, currentPoint);
            currentPoint = tempRandPoint;
        }

        if (currentPoint >= Waypoints.Length)
        {
			currentPoint = Swiput.GenerateRandomNumberBetweenWithIgnoreNum (0, Waypoints.Length, Waypoints.Length - 1);
        }

		transform.position = Vector3.MoveTowards(transform.position, Waypoints[currentPoint].position, MoveSpeed * Time.deltaTime);
    }

    void Shoot()
    {
		if (bulletSelectCounter <= 0f && currentHealthPercent < 80f) 
		{
			bulletSelectCounter = Random.Range (5f, 10f);

			bulletSelector = Random.Range (0, 3);

			if (bulletSelector > 0 && currentHealthPercent < 50f && Random.Range(0, 11) > 5)
				EnemyHealth += 5;
		}

		if (bulletSelector == 0) 
		{
			Anim.Play ();
			EnemySprite.sprite = EnemiesFx [0];
			EnemySprite.color = defaultBoxColour;
			BulletSelectShoot (EnemyBullets [0]);
		} 
		else if (bulletSelector == 1) 
		{
			Anim.Play ();
			EnemySprite.sprite = EnemiesFx [1];
			EnemySprite.color = Color.red;
			BulletSelectShoot (EnemyBullets [1]);
		}
		else 
		{
			Anim.Play ();
			EnemySprite.sprite = EnemiesFx [2];
			EnemySprite.color = Color.yellow;
			BulletSelectShoot (EnemyBullets [2]);
		}
    }

	void BulletSelectShoot(Bullet bulletSelected)
	{
		GameManager.Instance.PlayerDamageAmount = bulletSelected.BulletProperties.DamagePoint;

		Rigidbody2D bulletRb = Instantiate <Rigidbody2D>(bulletSelected.BulletProperties.BulletRB, Gun.position, Quaternion.identity);

		Vector2 downVel = Vector2.down * BulletSpeed;

		downVel.y -= ((Time.time - GameManager.Instance.TimeElasped) / AddOneVelocityAfterSeconds); 

		if (MaxDelay - 0.1f > MinDelay)
			MaxDelay -= (((Time.time - GameManager.Instance.TimeElasped) / (AddOneVelocityAfterSeconds * 2f)) * (PubMaxDelay / 100f));

		bulletRb.velocity = downVel;
	}

	void HealthUpdate()
	{
		if (EnemyHealth <= 0 && !runOnce) 
		{
			GameManager.Instance.HasAnybodyWon = GameManager.WhoWon.PlayerWon;
			GameManager.Instance.IsGameOver = true;

			runOnce = true;
		}
	}

	public void DepleteHealth(int hurtAmt)
    {
		EnemyHealth = EnemyHealth - hurtAmt;

		mat.SetFloat("_Fade", (float)EnemyHealth / (float)GameManager.Instance.EnemyMaxHealth);

		Anim.Play();

		if (EnemyHealth <= 0)
		{
			player.SwitchOffFx();
			EnemySprite.enabled = false;
			EnemyHealth = 0;
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.transform.CompareTag("PlayerBullet"))
		{
			Destroy(col.gameObject);

			DepleteHealth(GameManager.Instance.EnemyDamageAmount);
		}
	}
}
