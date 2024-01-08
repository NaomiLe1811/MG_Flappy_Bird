using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Các biến dùng để điều khiển người chơi
    [SerializeField] private float thrust, minTiltSmooth, maxTiltSmooth, hoverDistance, hoverSpeed;

    private bool start;
    private float timer, tiltSmooth, y;
    private Rigidbody2D playerRigid;
    private Quaternion downRotation, upRotation;

    void Start()
    {
        // Khởi tạo giá trị ban đầu
        tiltSmooth = maxTiltSmooth;
        playerRigid = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        upRotation = Quaternion.Euler(0, 0, 35);
    }

    void Update()
    {
        if (!start)
        {
            // Nếu chưa bắt đầu, di chuyển người chơi lên và xuống để tạo hiệu ứng trước khi bắt đầu
            timer += Time.deltaTime;
            y = hoverDistance * Mathf.Sin(hoverSpeed * timer);
            transform.localPosition = new Vector3(0, y, 0);
        }
        else
        {
            // Nếu đã bắt đầu, quay người chơi xuống khi rơi
            transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
        }
        // Giới hạn sự quay của người chơi trong khoảng giữa downRotation và upRotation
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, Mathf.Clamp(transform.rotation.z, downRotation.z, upRotation.z), transform.rotation.w);
    }

    void LateUpdate()
    {
        if (GameManager.Instance.GameState())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!start)
                {
                    // Xử lý lần nhấp chuột đầu tiên để bắt đầu trò chơi
                    start = true;
                    GameManager.Instance.GetReady();
                    GetComponent<Animator>().speed = 2;
                }
                playerRigid.gravityScale = 1f;
                tiltSmooth = minTiltSmooth;
                transform.rotation = upRotation;
                playerRigid.velocity = Vector2.zero;
                // Đẩy người chơi lên trên
                playerRigid.AddForce(Vector2.up * thrust);
                SoundManager.Instance.PlayTheAudio("Flap");
            }
        }
        if (playerRigid.velocity.y < -1f)
        {
            // Tăng trọng lực để khi di chuyển xuống nhanh hơn di chuyển lên
            tiltSmooth = maxTiltSmooth;
            playerRigid.gravityScale = 2f;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Score"))
        {
            // Xử lý va chạm với các đối tượng có tag "Score"
            Destroy(col.gameObject);
            GameManager.Instance.UpdateScore();
        }
        else if (col.transform.CompareTag("Obstacle"))
        {
            // Hủy các chướng ngại vật sau khi chúng đạt một khu vực cụ thể trên màn hình
            foreach (Transform child in col.transform.parent.transform)
            {
                child.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
            KillPlayer();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Ground"))
        {
            // Xử lý va chạm với đối tượng có tag "Ground"
            playerRigid.simulated = false;
            KillPlayer();
            transform.rotation = downRotation;
        }
    }

    public void KillPlayer()
    {
        // Xử lý việc kết thúc trò chơi khi người chơi thua
        GameManager.Instance.EndGame();
        playerRigid.velocity = Vector2.zero;
        // Dừng hoạt hình đập cánh
        GetComponent<Animator>().enabled = false;
    }
}
