using UnityEngine;

public static class ScreenBoundary
{
    public static Vector3 ClampToScreen(Vector3 targetPosition, Vector2 padding)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return targetPosition;

        // คำนวณขอบเขตของกล้องใน World Space
        Vector3 minBounds = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxBounds = mainCam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // ล็อคค่า X และ Y ไม่ให้เกินขอบ (บวกค่า Padding เพื่อไม่ให้ไฟล์จมหายไปครึ่งตัว)
        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x + padding.x, maxBounds.x - padding.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y + padding.y, maxBounds.y - padding.y);

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }
}