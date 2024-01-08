using UnityEngine;

public class ScrollSprite : MonoBehaviour
{
    public float scrollSpeed = 1.0f;  // Tốc độ cuộn
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        // Lấy offset hiện tại của material
        Vector2 offset = rend.material.mainTextureOffset;

        // Tính toán offset mới dựa trên tốc độ và thời gian
        float offsetX = Time.time * scrollSpeed;

        // Áp dụng offset mới
        offset.x = offsetX;

        // Cập nhật offset cho material
        rend.material.mainTextureOffset = offset;
    }
}
