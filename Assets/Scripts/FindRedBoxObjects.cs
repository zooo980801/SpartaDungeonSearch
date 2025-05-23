using UnityEngine;
using UnityEditor;

public class FindRedBoxObjects
{
    [MenuItem("Tools/🔍 Find Red Box Objects")]
    public static void FindRedBoxes()
    {
        int found = 0;

        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            if (!go.activeInHierarchy) continue;

            // Renderer가 있는지 확인
            var rend = go.GetComponent<Renderer>();
            var line = go.GetComponent<LineRenderer>();

            if (rend != null && rend.sharedMaterial != null)
            {
                Color color = rend.sharedMaterial.color;
                if (IsRed(color))
                {
                    Debug.Log($"🟥 Renderer 빨간 오브젝트: {go.name}", go);
                    found++;
                }
            }

            if (line != null && line.sharedMaterial != null)
            {
                Color color = line.sharedMaterial.color;
                if (IsRed(color))
                {
                    Debug.Log($"🟥 LineRenderer 빨간 오브젝트: {go.name}", go);
                    found++;
                }
            }
        }

        Debug.Log($"✅ 총 빨간 오브젝트 {found}개 발견 완료");
    }

    static bool IsRed(Color c)
    {
        return c.r > 0.9f && c.g < 0.2f && c.b < 0.2f && c.a > 0.5f;
    }
}
