using System;
using UnityEngine;

// Implementation of easing functions found on https://easings.net/fr
public class Easing
{
    public enum Style
    {
        Linear, InSine, OutSine, InOutSine, InQuad, OutQuad, InOutQuad, InCubic, OutCubic, InOutCubic, InQuart, OutQuart, InOutQuart, InQuint, OutQuint, InOutQuint, InExpo, OutExpo, InOutExpo, InCirc, OutCirc, InOutCirc, InBack, OutBack, InOutBack, InElastic, OutElastic, InOutElastic, InBounce, OutBounce, InOutBounce
    }

    public static Func<float, float> GetEasing(Style style)
    {
        switch (style)
        {
            case Style.Linear:
                return (t) => t;
            case Style.InOutSine:
                return (t) => 0.5f * (1f + Mathf.Sin(Mathf.PI * (t - 0.5f)));

            case Style.InSine:
                return (t) => Mathf.Sin(1.5707963f * t);

            case Style.OutSine:
                return (t) => 1f + Mathf.Sin(1.5707963f * (--t));

            case Style.InQuad:
                return (t) => t * t;

            case Style.OutQuad:
                return (t) => t * (2f - t);

            case Style.InOutQuad:
                return (t) => t < 0.5 ? 2f * t * t : t * (4 - 2f * t) - 1;

            case Style.InCubic:
                return (t) => t * t * t;

            case Style.OutCubic:
                return (t) => 1f + (--t) * t * t;

            case Style.InOutCubic:
                return (t) =>
                {
                    if (t < 0.5)
                    {
                        return 4 * t * t * t;
                    }
                    else
                    {
                        float f = ((2 * t) - 2);
                        return 0.5f * f * f * f + 1;
                    }
                };

            case Style.InQuart:
                return (t) =>
                {
                    t *= t;
                    return t * t;
                };

            case Style.OutQuart:
                return (t) =>
                {
                    t = (--t) * t;
                    return 1f - t * t;
                };

            case Style.InOutQuart:
                return (t) =>
                {
                    if (t < 0.5)
                    {
                        t *= t;
                        return 8f * t * t;
                    }
                    else
                    {
                        t = (--t) * t;
                        return 1f - 8f * t * t;
                    }
                };

            case Style.InQuint:
                return (t) =>
                {
                    float t2 = t * t;
                    return t * t2 * t2;
                };

            case Style.OutQuint:
                return (t) =>
                {
                    float t2 = (--t) * t;
                    return 1f + t * t2 * t2;
                };

            case Style.InOutQuint:
                return (t) =>
                {
                    float t2;
                    if (t < 0.5)
                    {
                        t2 = t * t;
                        return 16 * t * t2 * t2;
                    }
                    else
                    {
                        t2 = (--t) * t;
                        return 1f + 16 * t * t2 * t2;
                    }
                };

            case Style.InExpo:
                return (t) =>
                {
                    return (Mathf.Pow(2, 8f * t) - 1f) / 255;
                };

            case Style.OutExpo:
                return (t) =>
                {
                    return 1f - Mathf.Pow(2, -8f * t);
                };

            case Style.InOutExpo:
                return (t) =>
                {
                    if (t < 0.5f)
                    {
                        return (Mathf.Pow(2, 16f * t) - 1f) / 510f;
                    }
                    else
                    {
                        return 1f - 0.5f * Mathf.Pow(2, -16f * (t - 0.5f));
                    }
                };

            case Style.InCirc:
                return (t) => 1f - Mathf.Sqrt(1f - t);

            case Style.OutCirc:
                return (t) => Mathf.Sqrt(t);

            case Style.InOutCirc:
                return (t) =>
                {
                    if (t < 0.5f)
                    {
                        return (1f - Mathf.Sqrt(1f - 2f * t)) * 0.5f;
                    }
                    else
                    {
                        return (1f + Mathf.Sqrt(2f * t - 1f)) * 0.5f;
                    }
                };

            case Style.InBack:
                return (t) => t * t * (2.70158f * t - 1.70158f);

            case Style.OutBack:
                return (t) => 1f + (--t) * t * (2.70158f * t + 1.70158f);

            case Style.InOutBack:
                return (t) =>
                {
                    if (t < 0.5)
                    {
                        return t * t * (7f * t - 2.5f) * 2f;
                    }
                    else
                    {
                        return 1f + (--t) * t * 2f * (7f * t + 2.5f);
                    }
                };

            case Style.InElastic:
                return (t) =>
                {
                    float t2 = t * t;
                    return t2 * t2 * Mathf.Sin(t * Mathf.PI * 4.5f);
                };

            case Style.OutElastic:
                return (t) =>
                {
                    float t2 = (t - 1f) * (t - 1f);
                    return 1f - t2 * t2 * Mathf.Cos(t * Mathf.PI * 4.5f);
                };

            case Style.InOutElastic:
                return (t) =>
                {
                    float t2;
                    if (t < 0.45f)
                    {
                        t2 = t * t;
                        return 8f * t2 * t2 * Mathf.Sin(t * Mathf.PI * 9f);
                    }
                    else if (t < 0.55f)
                    {
                        return 0.5f + 0.75f * Mathf.Sin(t * Mathf.PI * 4f);
                    }
                    else
                    {
                        t2 = (t - 1f) * (t - 1f);
                        return 1f - 8f * t2 * t2 * Mathf.Sin(t * Mathf.PI * 9);
                    }
                };

            case Style.InBounce:
                return (t) =>
                {
                    return Mathf.Pow(2, 6 * (t - 1f)) * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 3.5f));
                };

            case Style.OutBounce:
                return (t) =>
                {
                    return 1f - Mathf.Pow(2, -6 * t) * Mathf.Abs(Mathf.Cos(t * Mathf.PI * 3.5f));
                };

            case Style.InOutBounce:
                return (t) =>
                {
                    if (t < 0.5)
                    {
                        return 8f * Mathf.Pow(2, 8f * (t - 1f)) * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 7));
                    }
                    else
                    {
                        return 1f - 8f * Mathf.Pow(2, -8f * t) * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 7));
                    }
                };
            default:
                return null;
        }
    }
}
