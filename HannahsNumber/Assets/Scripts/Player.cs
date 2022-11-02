using UnityEngine;
using SwiputAPI;

public class Player : Singleton<Player> 
{
	public enum Bullets
	{
		NormalSquare,
		DiamondMultiplier
	}

    public Rigidbody2D PlayerRb;
	public Rigidbody2D PlayerBullet, PlayerBullet2;

	public FloatingJoystick floatingJoystick;

	public GameObject Muzzle;

	public Audio audioObj;

	public LineRenderer lineRenderer;

	public SpriteRenderer PlayerSprite;

	public Laser laser;

    public Transform Gun;

	public Camera Cam;

	public RectTransform RightScreenArea;
	public RectTransform LeftScreenArea;

	public Animation Anim;

	public bool isLaserOn = false;

	public Bullets ChooseCurrentBullet = Bullets.NormalSquare;

	[HideInInspector]
	public int PlayerHealth;

    public float MaxSpeed;
    public float BulletSpeed;
	public float MoveSpeed;
	public float Delay = 0.25f;

	private Vector2 Movement;

    private float currAngle = 0f;
    private float DesiAngle = 0f;

    private float DelayTime;
	private float HalfScreenWidth;

	private Material mat;

	int counter = 0;

	bool runOnce;

	float xAxisPositive, xAxisNegative;

	void Awake()
	{
		mat = PlayerSprite.material;
	}

	void Start()
	{
		HalfScreenWidth = Cam.aspect * Cam.orthographicSize;
		PlayerHealth = GameManager.Instance.PlayerMaxHealth;

		runOnce = false;
	}

	public void LaserOn()
    {
		laser.SwitchOnLaser();
		Muzzle.SetActive(true);

		isLaserOn = true;
	}

	public void LaserOff()
    {
		laser.SwitchOffLaser();
		Muzzle.SetActive(false);

		isLaserOn = false;
	}
	
    void FixedUpdate()
	{
		if (GameManager.Instance.HasAnybodyWon == GameManager.WhoWon.EnemyWon) 
			return;

        currAngle = Mathf.LerpAngle(currAngle, DesiAngle, Time.deltaTime * 3f);
        transform.localEulerAngles = new Vector3(0f, 0f, currAngle);

        if (Input.GetKeyDown(KeyCode.Space))
        {
			counter++;

			if (counter % 2 != 0)
			{
				laser.SwitchOnLaser();
				Muzzle.SetActive(true);
			}
			else
			{
				laser.SwitchOffLaser();
				Muzzle.SetActive(false);
			}
		}

		if (Mathf.Abs(PlayerRb.velocity.x) == 0f && Time.time > DelayTime && (GameManager.Instance.IsStartGame && !GameManager.Instance.IsGameOver) && !isLaserOn)
		{
			DelayTime = Time.time + Delay;
			ShootBullet();
			audioObj.PlayGun();
		}

		//Wraps the player's movement position around the screen
		if (transform.position.x - transform.lossyScale.x > HalfScreenWidth) 
		{
			transform.position = new Vector3 (-HalfScreenWidth, transform.position.y, transform.position.z);
		}
		if (transform.position.x + transform.lossyScale.x < -HalfScreenWidth) 
		{
			transform.position = new Vector3 (HalfScreenWidth, transform.position.y, transform.position.z);
		}

        switch (GameManager.Instance.controlSyle)
        {
            case GameManager.ControlSyle.Tap:
				xAxisPositive = Swiput.TouchInRectTransform(RightScreenArea, 1f, true, false);
				xAxisNegative = Swiput.TouchInRectTransform(LeftScreenArea, -1f, true, false);
				break;
            case GameManager.ControlSyle.Joystick:
				xAxisPositive = floatingJoystick.Horizontal * Time.deltaTime * 3.5f;
				xAxisNegative = floatingJoystick.Vertical * Time.deltaTime * 3.5f;
				break;
        }

        float totalXAxis = xAxisPositive + xAxisNegative;

		PlayerMovement (totalXAxis);
		HealthUpdate ();
    }

    void ShootBullet()
    {
		if (ChooseCurrentBullet == Bullets.NormalSquare) 
			BulletChoice (PlayerBullet);

		if (ChooseCurrentBullet == Bullets.DiamondMultiplier) 
			BulletChoice (PlayerBullet2);
    }

	void BulletChoice(Rigidbody2D chosenBullet)
	{
		Rigidbody2D bulletRb = Instantiate <Rigidbody2D>(chosenBullet, Gun.position, Quaternion.identity);

		bulletRb.velocity = Vector2.up * BulletSpeed;
	}

    void PlayerMovement (float xAxis)
    {
        //Physics based movement
        if (PlayerRb.velocity.x < MaxSpeed)
        {
			Movement.Set(xAxis * MoveSpeed * PlayerRb.mass, 0f);
            PlayerRb.velocity = Movement;
        }
        if (Mathf.Abs(PlayerRb.velocity.x) > MaxSpeed)
        {
            PlayerRb.velocity = new Vector2(Mathf.Sign(PlayerRb.velocity.x) * MaxSpeed, PlayerRb.velocity.y);
        }

        //.......................................................................................................................................................

        //Banking Rotation of the Player
        if (xAxis > 0f)
        {
            DesiAngle = -20f;
        }
        else if (xAxis < 0f)
        {
            DesiAngle = 20f;
        }
        else
        {
            DesiAngle = 0f;
            PlayerRb.velocity = Vector2.zero;
        }
    }

	void HealthUpdate()
	{
		if (PlayerHealth <= 0 && !runOnce) 
		{
			GameManager.Instance.HasAnybodyWon = GameManager.WhoWon.EnemyWon;
			GameManager.Instance.IsGameOver = true;

			runOnce = true;
		}
	}

	public void SwitchOffFx()
    {
		laser.SwitchOffLaser();
		Muzzle.SetActive(false);
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.transform.CompareTag("EnemyBullet") && !GameManager.Instance.IsInvincible && GameManager.Instance.IsStartGame && !GameManager.Instance.IsGameOver) 
		{
			Destroy (col.gameObject);

			PlayerHealth = PlayerHealth - GameManager.Instance.PlayerDamageAmount;

			mat.SetFloat("_Fade", (float)PlayerHealth / (float)GameManager.Instance.PlayerMaxHealth);

			Anim.Play ();

			if (PlayerHealth <= 0) 
			{
				SwitchOffFx();
				PlayerSprite.enabled = false;
				PlayerHealth = 0;
			}
		}
	}
}
