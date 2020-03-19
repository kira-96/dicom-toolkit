/* original source file
 * https://github.com/aduskin/AduSkin/blob/master/src/AduSkin/Controls/Metro/AduLoading.cs
 */

using System.Windows;
using System.Windows.Controls;

namespace SimpleDICOMToolkit.Controls
{
    /// <summary>
    /// original source file
    /// https://github.com/aduskin/AduSkin/blob/master/src/AduSkin/Controls/ControlEnum.cs
    /// </summary>
    public enum EnumLoadingType
    {
        /// <summary>
        /// 两个圆
        /// </summary>
        DoubleRound,
        /// <summary>
        /// 一个圆
        /// </summary>
        SingleRound,
        /// <summary>
        /// 仿Win10加载条
        /// </summary>
        Win10,
        /// <summary>
        /// 仿Android加载条
        /// </summary>
        Android,
        /// <summary>
        /// 仿苹果加载条
        /// </summary>
        Apple,
        Cogs,
        Normal,
        /// <summary>
        /// 线条动画
        /// </summary>
        Lines,
        /// <summary>
        /// 方格动画
        /// </summary>
        Grids,
        /// <summary>
        /// 中心旋转动画
        /// </summary>
        Rotate,
        /// <summary>
        /// 版块加载
        /// </summary>
        Block
    }

    public class AduLoading : Control
    {
        #region Private属性
        private FrameworkElement PART_Root;
        #endregion

        #region 依赖属性定义
        public bool IsActived
        {
            get { return (bool)GetValue(IsActivedProperty); }
            set { SetValue(IsActivedProperty, value); }
        }

        public static readonly DependencyProperty IsActivedProperty =
            DependencyProperty.Register("IsActived", typeof(bool), typeof(AduLoading), new PropertyMetadata(true, OnIsActivedChangedCallback));

        private static void OnIsActivedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AduLoading AduLoading = d as AduLoading;
            if (AduLoading.PART_Root == null)
            {
                return;
            }
            VisualStateManager.GoToElementState(AduLoading.PART_Root, (bool)e.NewValue ? "Active" : "Inactive", true);
        }

        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpeedRatio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(AduLoading), new PropertyMetadata(1d, OnSpeedRatioChangedCallback));

        private static void OnSpeedRatioChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AduLoading AduLoading = d as AduLoading;
            if (AduLoading.PART_Root == null || !AduLoading.IsActived)
            {
                return;
            }
            AduLoading.SetSpeedRatio(AduLoading.PART_Root, AduLoading.SpeedRatio);
        }

        public EnumLoadingType Type
        {
            get { return (EnumLoadingType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(EnumLoadingType), typeof(AduLoading), new PropertyMetadata(EnumLoadingType.SingleRound));


        #endregion

        #region Constructors
        static AduLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AduLoading), new FrameworkPropertyMetadata(typeof(AduLoading)));
        }
        #endregion

        #region Override方法
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Root = this.GetTemplateChild("PART_Root") as FrameworkElement;
            if (this.PART_Root != null)
            {
                VisualStateManager.GoToElementState(this.PART_Root, this.IsActived ? "Active" : "Inactive", true);
                this.SetSpeedRatio(this.PART_Root, this.SpeedRatio);
            }
        }
        #endregion

        #region Private方法
        private void SetSpeedRatio(FrameworkElement element, double speedRatio)
        {
            foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(element))
            {
                if (group.Name == "ActiveStates")
                {
                    foreach (VisualState state in group.States)
                    {
                        if (state.Name == "Active")
                        {
                            state.Storyboard.SetSpeedRatio(element, speedRatio);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
