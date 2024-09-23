using System.Windows.Media.Animation;

namespace Panuon.WPF.Charts
{
    internal static class AnimationUtil
    {
        #region CreateEasingFunction
        public static IEasingFunction CreateEasingFunction(AnimationEasing? animationEasing)
        {
            if (animationEasing == null)
            {
                return null;
            }
            switch (animationEasing)
            {
                case AnimationEasing.BackIn:
                    return new BackEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.BackOut:
                    return new BackEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.BackInOut:
                    return new BackEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.BounceIn:
                    return new BounceEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.BounceOut:
                    return new BounceEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.BounceInOut:
                    return new BounceEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.CircleIn:
                    return new CircleEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.CircleOut:
                    return new CircleEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.CircleInOut:
                    return new CircleEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.CubicIn:
                    return new CubicEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.CubicOut:
                    return new CubicEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.CubicInOut:
                    return new CubicEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.ElasticIn:
                    return new ElasticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.ElasticOut:
                    return new ElasticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.ElasticInOut:
                    return new ElasticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.ExponentialIn:
                    return new ExponentialEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.ExponentialOut:
                    return new ExponentialEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.ExponentialInOut:
                    return new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.PowerIn:
                    return new PowerEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.PowerOut:
                    return new PowerEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.PowerInOut:
                    return new PowerEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.QuadraticIn:
                    return new QuadraticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.QuadraticOut:
                    return new QuadraticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.QuadraticInOut:
                    return new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.QuarticIn:
                    return new QuarticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.QuarticOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.QuarticInOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.QuinticIn:
                    return new QuarticEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.QuinticOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.QuinticInOut:
                    return new QuarticEase() { EasingMode = EasingMode.EaseInOut };
                case AnimationEasing.SineIn:
                    return new SineEase() { EasingMode = EasingMode.EaseIn };
                case AnimationEasing.SineOut:
                    return new SineEase() { EasingMode = EasingMode.EaseOut };
                case AnimationEasing.SineInOut:
                    return new SineEase() { EasingMode = EasingMode.EaseInOut };
            }
            return null;
        }
        #endregion

    }
}
