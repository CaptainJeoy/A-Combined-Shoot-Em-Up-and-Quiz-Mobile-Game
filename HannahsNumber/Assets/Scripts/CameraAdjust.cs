using UnityEngine;

public class CameraAdjust : MonoBehaviour 
{
	enum AutoZoomOrientation
	{
		HeightBased,
		WidthBased,
		Both
	}

	public SpriteRenderer AreaReference;
	public Camera cam;

	[SerializeField]
	private AutoZoomOrientation chooseOrientation = AutoZoomOrientation.HeightBased;

	void Awake()
	{
		switch (chooseOrientation) 
		{
			case AutoZoomOrientation.HeightBased:
				AutoHeightZoomAdjust (AreaReference, cam);
				break;
			case AutoZoomOrientation.WidthBased:
				AutoWidthZoomAdjust (AreaReference, cam);
				break;
			case AutoZoomOrientation.Both:
				AutoRectZoomAdjust(AreaReference, cam);
				break;
		}
	}

	void AutoHeightZoomAdjust (SpriteRenderer area, Camera cam)
	{
		cam.orthographicSize = area.bounds.size.y / 2f;
	}

	void AutoWidthZoomAdjust (SpriteRenderer area, Camera cam)
	{
		float widthReso = (area.bounds.size.x * (float)Screen.height) / (float)Screen.width;

		cam.orthographicSize = widthReso / 2f;
	}

	void AutoRectZoomAdjust (SpriteRenderer area, Camera cam)
    {
		float screenRatio = (float)Screen.width / (float)Screen.height;
		float areaRatio = area.bounds.size.x / area.bounds.size.y;

        if (screenRatio >= areaRatio)
        {
			cam.orthographicSize = area.bounds.size.y / 2f;
		}
        else
        {
			float diffInSize = areaRatio / screenRatio;
			cam.orthographicSize = area.bounds.size.y / 2f * diffInSize;
		}
	}
}

