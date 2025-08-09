using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UITransition
{
    //对RectTransform的拓展方法
    public static bool CheckMousePosIsIn(this RectTransform rectTransform)
    {
        //获取四个角的世界坐标
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        //转换为屏幕坐标
        Vector2[] screenCorners = new Vector2[4];
        for (int i = 0; i < corners.Length; i++)
        {
            screenCorners[i] = Camera.main.WorldToScreenPoint(corners[i]);
        }
        //分为两个三角形分别判断
        bool inTriangle1 = IsPointInTriangle(Input.mousePosition, screenCorners[0], screenCorners[1], screenCorners[2]);
        bool inTriangle2 = IsPointInTriangle(Input.mousePosition, screenCorners[0], screenCorners[2], screenCorners[3]);
        //如果在任意一个三角形内，则返回true
        return inTriangle1 || inTriangle2;
    }

    //判断点是否在三角形内
    private static bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
        float s = 1f / (2f * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * point.x + (a.x - c.x) * point.y);
        float t = 1f / (2f * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * point.x + (b.x - a.x) * point.y);
        return s >= 0 && t >= 0 && (s + t) <= 1;
    }
}
