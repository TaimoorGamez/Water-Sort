using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/UIGradient")]
public class UIGradient : BaseMeshEffect
{
    public enum GradientType
    {
        Vertical,
        Horizontal,
        DiagonalLeftToRight,
        DiagonalRightToLeft,
        FromCenterToBoundaries
    }

    public enum BlendMode
    {
        Normal,
        Additive,
        Multiply
    }

    [SerializeField]
    private Gradient gradient = new Gradient();

    [SerializeField]
    private GradientType gradientType = GradientType.Vertical; // Default gradient type is Vertical

    [SerializeField]
    private BlendMode blendMode = BlendMode.Normal; // Default blend mode is Normal

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0)
            return;

        Rect rect = graphic.rectTransform.rect;

        UIVertex vertex = new UIVertex();
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);

            // Calculate normalized position and clamp it to [0, 1]
            float normalizedPosition = Mathf.Clamp(GetNormalizedPosition(vertex.position, rect), 0f, 1f);

            // Apply gradient color with the chosen blend mode
            vertex.color = ApplyBlendMode(vertex.color, gradient.Evaluate(normalizedPosition));
            vh.SetUIVertex(vertex, i);
        }
    }

    // Method to calculate normalized position for each vertex based on gradient type
    private float GetNormalizedPosition(Vector3 position, Rect rect)
    {
        float normalizedPosition = 0f;

        switch (gradientType)
        {
            case GradientType.Vertical:
                normalizedPosition = (position.y - rect.yMin) / rect.height;
                break;

            case GradientType.Horizontal:
                normalizedPosition = (position.x - rect.xMin) / rect.width;
                break;

            case GradientType.DiagonalLeftToRight:
                normalizedPosition = ((position.x - rect.xMin) + (position.y - rect.yMin)) / (rect.width + rect.height);
                break;

            case GradientType.DiagonalRightToLeft:
                normalizedPosition = ((position.x - rect.xMax) + (position.y - rect.yMin)) / (rect.width + rect.height);
                break;

            case GradientType.FromCenterToBoundaries:
                float centerX = rect.xMin + rect.width / 2;
                float centerY = rect.yMin + rect.height / 2;
                float distanceToCenter = Vector2.Distance(new Vector2(position.x, position.y), new Vector2(centerX, centerY));
                float maxDistanceToCenter = Mathf.Sqrt(Mathf.Pow(rect.width / 2, 2) + Mathf.Pow(rect.height / 2, 2));
                normalizedPosition = distanceToCenter / maxDistanceToCenter;
                break;
        }

        return normalizedPosition;
    }

    // Method to apply the selected blend mode to the gradient color
    private Color ApplyBlendMode(Color originalColor, Color gradientColor)
    {
        switch (blendMode)
        {
            case BlendMode.Additive:
                return originalColor + gradientColor;
            case BlendMode.Multiply:
                return originalColor * gradientColor;
            case BlendMode.Normal:
            default:
                return gradientColor;
        }
    }

    // Public properties to allow changing the gradient, type, and blend mode
    public Gradient Gradient
    {
        get => gradient;
        set
        {
            gradient = value;
            graphic.SetVerticesDirty();
        }
    }

    public GradientType GradientDirection
    {
        get => gradientType;
        set
        {
            gradientType = value;
            graphic.SetVerticesDirty();
        }
    }

    public BlendMode BlendModeType
    {
        get => blendMode;
        set
        {
            blendMode = value;
            graphic.SetVerticesDirty();
        }
    }
}
