using UnityEngine;

public class RotatingLaserTrap : MonoBehaviour
{
    [Header("레이저 설정")]
    public float rayLength = 10f;
    public float rotationSpeed = 60f; // 회전 속도 (도/초)
    public LayerMask detectionLayer;

    private LineRenderer[] lasers = new LineRenderer[4];
    private Vector3[] directions = new Vector3[4]
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    void Start()
    {
        // 4개의 레이저 만들기
        for (int i = 0; i < 4; i++)
        {
            GameObject laserObj = new GameObject("Laser_" + i);
            laserObj.transform.parent = transform;
            laserObj.transform.localPosition = Vector3.zero;

            LineRenderer lr = laserObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.material.color = Color.red;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.positionCount = 2;

            lasers[i] = lr;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // 전체 회전

        for (int i = 0; i < lasers.Length; i++)
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.TransformDirection(directions[i]);

            Ray ray = new Ray(origin, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength, detectionLayer))
            {
                lasers[i].SetPosition(0, origin);
                lasers[i].SetPosition(1, hit.point);

                // ✅ 플레이어 데미지 처리
                PlayerCondition player = hit.collider.GetComponent<PlayerCondition>();
                if (player != null && !player.IsInvincible())
                {
                    player.TakePhysicalDamage(10);       // 체력 감소
                    player.SetInvincible(1.0f);          // 1초 무적
                }

                Debug.Log($"⚠️ 레이저 {i} 감지됨: {hit.collider.name}");
            }
            else
            {
                lasers[i].SetPosition(0, origin);
                lasers[i].SetPosition(1, origin + direction * rayLength);
            }
        }
    }
}
