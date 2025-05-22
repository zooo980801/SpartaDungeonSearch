using UnityEngine;
using TMPro;

public class MovingPlatForm : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    public Vector3 launchDirection = Vector3.up + Vector3.forward;
    public float launchForce = 10f;
    public float launchDelay = 3f;

    private Vector3 target;
    private Rigidbody playerRb;
    private bool playerOnPlatform = false;
    private float launchTimer = 0f;

    [Header("발사 안내 UI")]
    public GameObject launchTextUI;
    public TextMeshProUGUI launchText;

    void Start()
    {
        target = pointB.position;
        if (launchTextUI != null)
            launchTextUI.SetActive(false); // 처음엔 안 보이게
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }

        if (playerOnPlatform && playerRb != null)
        {
            launchTimer += Time.deltaTime;

            // UI 갱신
            if (launchTextUI != null && launchText != null)
            {
                float remaining = Mathf.Max(0f, launchDelay - launchTimer);
                launchText.text =
                    $"<color=#00FF00><b>[스페이스]</b> 누르면 즉시 발사</color>\n" +
                    $"<color=#FFA500>가만히 있어도 {remaining:F1}초 후 발사</color>";
            }


            // 자동 발사
            if (launchTimer >= launchDelay)
            {
                LaunchPlayer();
            }

            // 수동 발사
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LaunchPlayer();
            }
        }
    }

    void LaunchPlayer()
    {
        playerRb.velocity = Vector3.zero;
        playerRb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);

        launchTimer = 0f;
        playerOnPlatform = false;
        playerRb = null;

        // UI 숨김
        if (launchTextUI != null)
            launchTextUI.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerOnPlatform = true;
                launchTimer = 0f;

                if (launchTextUI != null)
                    launchTextUI.SetActive(true);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            playerRb = null;
            launchTimer = 0f;

            if (launchTextUI != null)
                launchTextUI.SetActive(false);
        }
    }
}
