using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Audio audioObj;

    [SerializeField] private Audio audioObj2;

    [SerializeField] private float laserLength = 100f, delay = 0.05f;

    [SerializeField] private LayerMask layer;

    [SerializeField] private Transform originPosition;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Enemy enemy;

    [SerializeField] private Vector2 originOffset, endOffset;

    [SerializeField] private GameObject hitVfx;

    RaycastHit2D hitInfo;

    GameObject hv;

    bool switchedOff = true;

    float span;

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;

        hv = Instantiate(hitVfx);
        hv.SetActive(false);

        span = delay;
    }

    private void FixedUpdate()
    {
        if (!switchedOff)
        {
            hitInfo = Physics2D.Raycast((Vector2)originPosition.position + originOffset, transform.up, laserLength, layer);
            lineRenderer.SetPosition(0, originPosition.position + (Vector3)originOffset);

            if (hitInfo.collider != null)
            {
                lineRenderer.SetPosition(1, (Vector3)hitInfo.point);
                hv.SetActive(true);
                hv.transform.position = (Vector3)(hitInfo.point + endOffset);

                audioObj2.PlayGun();
                audioObj2.SetVolume(0.5f);

                span -= Time.deltaTime;

                if (span <= 0f)
                {
                    span = delay;
                    enemy.DepleteHealth(1);
                }  
            }
            else
            {
                lineRenderer.SetPosition(1, transform.up * laserLength);
                hv.SetActive(false);
                audioObj2.SetVolume(0.2f);
            }
        }
    }

    public void SwitchOffLaser()
    {
        lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        hv.SetActive(false);
        audioObj.StopAudio();
        audioObj2.StopAudio();

        switchedOff = true;
    }

    public void SwitchOnLaser()
    {
        switchedOff = false;
        audioObj.PlayGun();
    }
}
