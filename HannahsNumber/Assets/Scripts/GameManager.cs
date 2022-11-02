using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> 
{
	public enum WhoWon
	{
		PlayerWon,
		EnemyWon,
		NobodyYet
	}

	public enum PowerUps
	{
		Multiply,
		Invincible,
		ExtraHealth,
		Laser
	}

	public enum ControlSyle
    {
		Tap,
		Joystick
    }

	public Audio audioObj, audioObj2;

	public Camera Cam;

	public Image[] ControlVisualizers;

	[HideInInspector]
	public bool IsGameOver = false, isFirstTime = false, IsStartGame = false,
	IsMultiplier = false, IsInvincible = false, IsExtraHealth = false, IsInMenu = false;

	[HideInInspector]
	public float TimeElasped;

	[HideInInspector]
	public int PlayerDamageAmount = 0, EnemyDamageAmount = 0;

	[HideInInspector]
	public ControlSyle controlSyle;

	public Transform[] PowerUpSpawnPoints;

	public Transform LeftEndWayPoint, RightEndWayPoint, 
	LeftPowerUpPoint, RightPowerUpPoint;

	public WhoWon HasAnybodyWon = WhoWon.NobodyYet;

	public GameObject Shield;

	public GameObject PlayerObj;
	public GameObject EnemyObj;
	public GameObject PowerUpPanel;
	public GameObject HUDPanel;

	public GameObject MenuPanel;
	public GameObject StartPanel;
	public GameObject LosePanel;
	public GameObject WinPanel;
	public GameObject HUDCanvas;

	public GameObject MultiplierPanel;
	public GameObject InvinciblePanel;
	public GameObject LaserPanel;
	public GameObject pointerHand;

	public GameObject MultiplierPowerUp;
	public GameObject InvinciblePowerUp;
	public GameObject ExtraHealthPowerUp;
	public GameObject LaserPowerUp;

	public Image EnemyEnergy, PlayerEnergy, MultiplierRadial, InvincibleRadial, QuizRadial, LaserRadial;

	public Button QuizButton;

	public Text StartCounterText, QuizText;

	public Text WinScore, WinHighScore, LoseHighScore, CurrentScore;

	public float StartDelay = 7, PowerUpDelay = 20f,
	MultiPlierWaitTime = 30f, InvincibleWaitTime = 30f, ExtraHealthWaitTime = 20f, QuizWaitTime = 10f,
		LaserWaitTime = 30f;

	public int SetPlayerDamageAmount = 10, SetEnemyDamageAmount = 1, ExtraHealth = 50;
	public int EnemyMaxHealth = 100, PlayerMaxHealth = 100;

	private float startCounter = 0f, multiplierCountDown = 0f, invincibleCountDown = 0f, quizCountDown = 0f,
		laserCountdown = 0f;

	float HalfScreenWidth;

	private const string key = "Hannah";

	void Awake()
	{
		//Working on setting up menu 
		IsInMenu = true;

		/*
		if (PlayerPrefs.HasKey(highScoreKey)) 
		{
			FirstTime = false;

			LastHighScore = PlayerPrefs.GetInt (highScoreKey);
		} 
		else 
		{
			FirstTime = true;

			PlayerPrefs.SetInt (highScoreKey, 1000000);

			LastHighScore = PlayerPrefs.GetInt (highScoreKey);
		}
		*/

		if (PlayerPrefs.HasKey(key))
		{
			isFirstTime = false;
		}
		else
		{
			isFirstTime = true;
			PlayerPrefs.SetInt(key, 0);
		}

		audioObj2.PlayGun();
	}

	public void StartGame()
	{
		AudioSingle.Instance.Play();
		PlayerObj.SetActive(true);
		EnemyObj.SetActive(true);

		MenuPanel.SetActive(false);
		StartPanel.SetActive(true);

		audioObj2.StopAudio();
		audioObj.PlayGun();

		IsInMenu = false;
	}

	void Start()
	{
		HalfScreenWidth = Cam.aspect * Cam.orthographicSize;
		startCounter = StartDelay;
		IsGameOver = false;

		PlayerDamageAmount = SetPlayerDamageAmount;
		EnemyDamageAmount = SetEnemyDamageAmount;

		quizCountDown = QuizWaitTime + StartDelay;

		multiplierCountDown = invincibleCountDown = laserCountdown = PowerUpDelay;

		//Auto Adjust Enemy WayPoint
		float autoDistance = (2.25f - 2.7f);

		Vector3 tempRightPos = RightEndWayPoint.position;
		tempRightPos.x = HalfScreenWidth - autoDistance;
		RightEndWayPoint.position = tempRightPos;

		Vector3 tempLeftPos = LeftEndWayPoint.position;
		tempLeftPos.x = -HalfScreenWidth + autoDistance;
		LeftEndWayPoint.position = tempLeftPos;

		//Auto Adjust PowerUp SpawnPoint
		float autoDistancePowerUp = (2.25f - 1.3f);

		Vector3 tempRightPosPower = RightPowerUpPoint.position;
		tempRightPosPower.x = HalfScreenWidth - autoDistancePowerUp;
		RightPowerUpPoint.position = tempRightPosPower;

		Vector3 tempLeftPosPower = LeftPowerUpPoint.position;
		tempLeftPosPower.x = -HalfScreenWidth + autoDistancePowerUp;
		LeftPowerUpPoint.position = tempLeftPosPower;
	}

	void Update()
	{
		if (IsInMenu)
			return;

		PreStart ();
	
		CheckWhetherYouLost ();
		CheckWhetherYouWin ();

		//UpdateEnergyBar ();

		QuizButtonSpan();

		//if (IsStartGame && !IsGameOver) 
		//CurrentScore.text = Mathf.RoundToInt (Time.time - TimeElasped).ToString ();

		if (IsMultiplier)
			PowerUpMultiplier ();

		if (IsInvincible)
			PowerUpInvincible ();

		if (IsExtraHealth)
			PowerUpExtraHealth ();

		if (Player.Instance.isLaserOn)
			LaserMultiplier();
	}

	void PreStart()
	{
		if (IsStartGame)
			return;

		StartCounterText.text = Mathf.RoundToInt(startCounter).ToString ();
		startCounter -= Time.deltaTime;

		if (startCounter <= 0f) 
		{
			IsStartGame = true;
			IsGameOver = false;
			HasAnybodyWon = WhoWon.NobodyYet;

			StartPanel.SetActive (false);

			TimeElasped = Time.time;

			PowerUpPanel.SetActive (true);
			HUDPanel.SetActive(true);
		}
	}

	void UpdateEnergyBar()
	{
		EnemyEnergy.fillAmount = float.Parse(Enemy.Instance.EnemyHealth.ToString()) / float.Parse(EnemyMaxHealth.ToString());
		PlayerEnergy.fillAmount = float.Parse(Player.Instance.PlayerHealth.ToString()) / float.Parse(PlayerMaxHealth.ToString());
	}

	void CheckWhetherYouLost()
	{
		if (HasAnybodyWon == WhoWon.EnemyWon)
		{
			HUDCanvas.SetActive (false);
			PowerUpPanel.SetActive(false);

			WinPanel.SetActive (false);
			LosePanel.SetActive (true);

			HUDPanel.SetActive(false);

			/*
			if (LastHighScore == 1000000) 
			{
				LoseHighScore.gameObject.SetActive (false);
			} 
			else 
			{
				LoseHighScore.gameObject.SetActive (true);
				LoseHighScore.text = "BEST TIME : " + LastHighScore.ToString ();
			}
			*/
				
			HasAnybodyWon = WhoWon.NobodyYet;
		}
	}

	void CheckWhetherYouWin()
	{
		if (HasAnybodyWon == WhoWon.PlayerWon)
		{
			HUDCanvas.SetActive (false);
			PowerUpPanel.SetActive(false);

			LosePanel.SetActive (false);
			WinPanel.SetActive (true);

			HUDPanel.SetActive(false);

			/*
			WinScore.text = Mathf.RoundToInt (Time.time - TimeElasped).ToString ();

			if (LastHighScore < Mathf.RoundToInt (Time.time - TimeElasped)) 
			{
				WinHighScore.text = "BEST TIME : " + LastHighScore.ToString ();
			} 
			else
			{
				WinHighScore.text = "BEST TIME : " + Mathf.RoundToInt (Time.time - TimeElasped).ToString ();

				PlayerPrefs.SetInt (highScoreKey, Mathf.RoundToInt (Time.time - TimeElasped));
			}
			*/

			HasAnybodyWon = WhoWon.NobodyYet;
		}
	}

	///*
	void SpawnMultiplier()
	{
		Instantiate (MultiplierPowerUp, PowerUpSpawnPoints [Random.Range (0, PowerUpSpawnPoints.Length)].position, Quaternion.identity);
	}

	void SpawnInvincible()
	{
		Instantiate (InvinciblePowerUp, PowerUpSpawnPoints [Random.Range (0, PowerUpSpawnPoints.Length)].position, Quaternion.identity);
	}

	void SpawnExtraHealth()
	{
		Instantiate (ExtraHealthPowerUp, PowerUpSpawnPoints [Random.Range (0, PowerUpSpawnPoints.Length)].position, Quaternion.identity);
	}

	void SpawnLaser()
    {
		Instantiate(LaserPowerUp, PowerUpSpawnPoints[Random.Range(0, PowerUpSpawnPoints.Length)].position, Quaternion.identity);
	}

	public void RandomSpawnPowerUp()
    {
		int rand = Random.Range(1, 7);

        switch (rand)
        {
			case 1:
				SpawnMultiplier();
				SpawnExtraHealth();
			break;
			case 2:
				SpawnInvincible();
				SpawnExtraHealth();
			break;
			case 3:
				SpawnLaser();
				SpawnExtraHealth();
			break;
			case 4:
				SpawnLaser();
				SpawnExtraHealth();
				break;
			case 5:
				SpawnLaser();
				SpawnExtraHealth();
				break;
			case 6:
				SpawnInvincible();
				SpawnExtraHealth();
				break;
		}
    }

	void LaserMultiplier()
    {
		if (laserCountdown <= 0f || IsGameOver)
		{
			IsMultiplier = false;
			LaserPanel.SetActive(false);
			laserCountdown = PowerUpDelay;
			EnemyDamageAmount = SetEnemyDamageAmount;

			Player.Instance.LaserOff();

			return;
		}

		LaserPanel.SetActive(true);

		laserCountdown -= Time.deltaTime;
		float countDown = ((PowerUpDelay - laserCountdown) / PowerUpDelay);

		LaserRadial.fillAmount = countDown;
	}

	void PowerUpMultiplier ()
	{
		if (multiplierCountDown <= 0f || IsGameOver) 
		{
			IsMultiplier = false;
			MultiplierPanel.SetActive (false);
			multiplierCountDown = PowerUpDelay;
			EnemyDamageAmount = SetEnemyDamageAmount;

			Player.Instance.ChooseCurrentBullet = Player.Bullets.NormalSquare;

			return;
		}

		MultiplierPanel.SetActive (true);

		Player.Instance.ChooseCurrentBullet = Player.Bullets.DiamondMultiplier;

		EnemyDamageAmount = 3;

		multiplierCountDown -= Time.deltaTime;
		float countDown = ((PowerUpDelay - multiplierCountDown) / PowerUpDelay);

		MultiplierRadial.fillAmount = countDown;
	}

	void PowerUpInvincible ()
	{
		if (invincibleCountDown <= 0f || IsGameOver) 
		{
			IsInvincible = false;
			InvinciblePanel.SetActive (false);
			invincibleCountDown = PowerUpDelay;

			Shield.SetActive(false);

			return;
		}

		InvinciblePanel.SetActive (true);

		Shield.SetActive(true);

		invincibleCountDown -= Time.deltaTime;
		float countDown = ((PowerUpDelay - invincibleCountDown) / PowerUpDelay);

		InvincibleRadial.fillAmount = countDown;
	}

	void PowerUpExtraHealth ()
	{
		if (IsGameOver)
			return;
		
		if (Player.Instance.PlayerHealth + ExtraHealth >= PlayerMaxHealth) 
			Player.Instance.PlayerHealth = PlayerMaxHealth;
		else
			Player.Instance.PlayerHealth += ExtraHealth;

		IsExtraHealth = false;
	}

	void QuizButtonSpan()
    {
		if (Player.Instance.isLaserOn || IsMultiplier || IsInvincible)
			return;

		quizCountDown -= Time.deltaTime;

		QuizRadial.fillAmount = ((QuizWaitTime - quizCountDown) / QuizWaitTime);

        if (quizCountDown <= 0f)
        {
			QuizButton.interactable = true;
			QuizText.enabled = true;

			if (isFirstTime)
			{
				pointerHand.SetActive(true);
				isFirstTime = false;
			}
		}
        else
        {
			pointerHand.SetActive(false);
			QuizButton.interactable = false;
			QuizText.enabled = false;
		}
    }

	public void DeactivateQuizButton()
    {
		QuizRadial.fillAmount = 0f;
		pointerHand.SetActive(false);
		QuizButton.interactable = false;
		QuizText.enabled = false;
	}

	public void SetTapControls()
    {
		controlSyle = ControlSyle.Tap;
    }

	public void SetJoystickControls()
    {
		controlSyle = ControlSyle.Joystick;
    }

	public void ResetQuizCounter()
    {
		quizCountDown = QuizWaitTime;
    }

	public void RestartLevel ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		AudioSingle.Instance.Play();
	}
}
