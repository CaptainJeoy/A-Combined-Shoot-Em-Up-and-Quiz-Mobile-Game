using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
	public float SlowRate = 0.2f;
	public float SlowMoSpeed = 2f;
	public bool IsTimeFrozen = false;

	[HideInInspector] public bool IsTimeSlowTime = false; 
	[HideInInspector] public bool IsPause = false;

	void Update()
	{
		if (IsPause == true) 
			return;

		Time.timeScale += SlowMoSpeed * Time.unscaledDeltaTime;
		Time.timeScale = Mathf.Clamp01 (Time.timeScale);

		Time.fixedDeltaTime = Time.timeScale * 0.02f;
	}

	public void SlowTime()
	{
		Time.timeScale = SlowRate;
	}

	public void SlowTime(float SlowDownRate)
	{
		Time.timeScale = SlowDownRate;
	}

	public void FreezeTime()
	{
		IsTimeFrozen = true;
		IsPause = true;
		Time.timeScale = 0f;
	}

	public void UnFreezeTime()
	{
		IsTimeFrozen = false;
		IsPause = false;
		Time.timeScale = 1f;
	}
}
