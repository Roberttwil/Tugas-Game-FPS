using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isShooting, readyToShoot;
    private bool allowReset = true;

    [Header("Shooting Settings")]
    public float shootingDelay = 2f;
    public int bulletsPerBurst = 3;
    public int bulletBurstLeft;
    public float spreadIntensity;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    [Header("Effects")]
    public GameObject muzzleEffect;

    [Header("Laser Settings")]
    public Material laserMaterial;
    public float laserWidth = 0.03f;
    public float laserDuration = 0.05f;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        bulletBurstLeft = bulletsPerBurst;

        // Nonaktifkan efek muzzle saat awal
        if (muzzleEffect != null)
            muzzleEffect.SetActive(false);
    }

    void Update()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (readyToShoot && isShooting)
        {
            bulletBurstLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        // Suara tembakan
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.shootingSoundM1911.Play();
        }

        // Efek muzzle flash
        if (muzzleEffect != null)
        {
            muzzleEffect.SetActive(true);
            muzzleEffect.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DisableMuzzleEffect());
        }

        readyToShoot = false;

        // Hitung arah tembakan
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // Buat laser hanya saat tembak
        Vector3 laserEndPoint = bulletSpawn.position + shootingDirection * 100f;
        CreateLaserEffect(bulletSpawn.position, laserEndPoint);

        // Delay penembakan berikutnya
        if (allowReset)
        {
            Invoke(nameof(ResetShot), shootingDelay);
            allowReset = false;
        }

        // Untuk burst mode
        if (currentShootingMode == ShootingMode.Burst && bulletBurstLeft > 1)
        {
            bulletBurstLeft--;
            Invoke(nameof(FireWeapon), shootingDelay);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        // Tambahkan efek spread (acak)
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private IEnumerator DisableMuzzleEffect()
    {
        yield return new WaitForSeconds(0.5f);
        if (muzzleEffect != null)
            muzzleEffect.SetActive(false);
    }

    // 💡 Membuat laser secara instan ketika tembakan
    private void CreateLaserEffect(Vector3 start, Vector3 end)
    {
        if (laserMaterial == null) return;

        GameObject laserGO = new GameObject("LaserShot");
        LineRenderer lr = laserGO.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;

        lr.material = laserMaterial;
        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.numCapVertices = 2;

        Destroy(laserGO, laserDuration);
    }
}
