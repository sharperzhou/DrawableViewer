using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms.Integration;

#if AUTOCAD2015_TO_2024
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.DatabaseServices;
#elif GSTARCAD2024_TO_2025
using Gssoft.Gscad.GraphicsInterface;
using Gssoft.Gscad.DatabaseServices;
#elif GSTARCAD2017_TO_2023
using GrxCAD.GraphicsInterface;
using GrxCAD.DatabaseServices;
#endif

#if AUTOCAD2015_TO_2024
namespace Sharper.AutoCAD.DrawableViewer
#else
namespace Sharper.GstarCAD.Extensions
#endif
{
    /// <summary>
    /// 用于WPF控件的附加属性行为类，通过对 <see cref="WindowsFormsHost"/> 应用附加属性行为，间接对 <see cref="DrawableViewer"/> 进行数据绑定
    /// </summary>
    public static class DrawableViewerBehavior
    {
        /// <summary>
        /// 可绑定的绘图对象的属性
        /// </summary>
        public static readonly DependencyProperty DrawableObjectProperty =
            DependencyProperty.RegisterAttached("DrawableObject", typeof(Drawable), typeof(DrawableViewerBehavior),
                new FrameworkPropertyMetadata(OnDrawableObjectChanged)
                {
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, BindsTwoWayByDefault = false
                });

        /// <summary>
        /// 绘图对象更改的事件处理器
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <param name="e">绑定事件</param>
        /// <exception cref="NotSupportedException"></exception>
        private static void OnDrawableObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is WindowsFormsHost host) || !(host.Child is DrawableViewer viewer))
                return;

            if (e.Property != DrawableObjectProperty)
                return;

            viewer.EraseAll();
            if (!(e.NewValue is Drawable drawable))
            {
                viewer.Regenerate();
                return;
            }

            switch (drawable)
            {
                case BlockTableRecord blockTableRecord:
                    viewer.Add(blockTableRecord);
                    break;
                case Entity entity:
                    viewer.Add(entity);
                    break;
                default:
                    throw new NotSupportedException($"不支持的预览对象: {drawable.GetRXClass().Name}");
            }

            if (GetAutoZoomingWhenDrawableChanged(d))
                viewer.ZoomExtents();

            viewer.Regenerate();
        }

        /// <summary>
        /// 绘图对象改变时是否进行视图自适应缩放的属性
        /// </summary>
        public static readonly DependencyProperty AutoZoomingWhenDrawableChangedProperty =
            DependencyProperty.RegisterAttached("AutoZoomingWhenDrawableChanged", typeof(bool),
                typeof(DrawableViewerBehavior), new PropertyMetadata(true));

        /// <summary>
        /// <see cref="DrawableViewer"/> 控件能否响应鼠标事件
        /// </summary>
        public static readonly DependencyProperty CanMouseOperationProperty =
            DependencyProperty.RegisterAttached("CanMouseOperation", typeof(bool),
                typeof(DrawableViewerBehavior), new PropertyMetadata(true, OnCanMouseOperationChanged));

        /// <summary>
        /// 响应控件能否支持鼠标操作的事件处理器
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <param name="e">绑定事件</param>
        private static void OnCanMouseOperationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is WindowsFormsHost host) || !(host.Child is DrawableViewer viewer))
                return;

            if (e.Property != CanMouseOperationProperty)
                return;

            viewer.CanMouseOperation = Equals(e.NewValue, true);
        }

        /// <summary>
        /// 可绑定的外部源属性
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(object), 
                typeof(DrawableViewerBehavior), new PropertyMetadata(OnSourceChanged));

        /// <summary>
        /// 响应外部源更改的事件处理器
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <param name="e">绑定事件参数</param>
        /// <exception cref="NotSupportedException"></exception>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (!(d is WindowsFormsHost host) || !(host.Child is DrawableViewer viewer))
                return;

            if (e.Property != SourceProperty)
                return;

            switch (e.NewValue)
            {
                case null:
                    viewer.Source = null;
                    break;
                case string dwgPath:
                    viewer.Source = dwgPath;
                    break;
                case Database database:
                    viewer.Database = database;
                    break;
                default:
                    throw new NotSupportedException("无效的数据类型，只支持DWG文件路径或Database对象");
            }

            if (GetAutoZoomingWhenDrawableChanged(d))
                viewer.ZoomExtents();

            viewer.Regenerate();
        }

        /// <summary>
        /// 设置绘图对象
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <param name="drawable">绘图对象</param>
        public static void SetDrawableObject(DependencyObject d, Drawable drawable)
        {
            d.SetValue(DrawableObjectProperty, drawable);
        }

        /// <summary>
        /// 获取绘图对象
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <returns>绘图对象</returns>
        public static Drawable GetDrawableObject(DependencyObject d)
        {
            return (Drawable)d.GetValue(DrawableObjectProperty);
        }

        /// <summary>
        /// 设置绘图对象改变时是否进行视图自适应缩放
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <param name="autoZoomingWhenDrawableChanged">是否自适应缩放</param>
        public static void SetAutoZoomingWhenDrawableChanged(DependencyObject d, bool autoZoomingWhenDrawableChanged)
        {
            d.SetValue(AutoZoomingWhenDrawableChangedProperty, autoZoomingWhenDrawableChanged);
        }

        /// <summary>
        /// 获取绘图对象改变时是否进行视图自适应缩放
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <returns>是否自适应缩放</returns>
        public static bool GetAutoZoomingWhenDrawableChanged(DependencyObject d)
        {
            return (bool)d.GetValue(AutoZoomingWhenDrawableChangedProperty);
        }

        /// <summary>
        /// 设置控件是否支持鼠标操作
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <param name="canMouseOperation">是否支持鼠标操作</param>
        public static void SetCanMouseOperation(DependencyObject d, bool canMouseOperation)
        {
            d.SetValue(CanMouseOperationProperty, canMouseOperation);
        }

        /// <summary>
        /// 获取控件是否支持鼠标操作
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <returns>是否支持鼠标操作</returns>
        public static bool GetCanMouseOperation(DependencyObject d)
        {
            return (bool)d.GetValue(CanMouseOperationProperty);
        }

        /// <summary>
        /// 设置显示源
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <param name="source">DWG路径或Database对象</param>
        public static void SetSource(DependencyObject d, object source)
        {
            d.SetValue(SourceProperty, source);
        }

        /// <summary>
        /// 获取显示源
        /// </summary>
        /// <param name="d">被绑定的 <see cref="WindowsFormsHost"/> 对象</param>
        /// <returns>DWG路径或Database对象</returns>
        public static object GetSource(DependencyObject d)
        {
            return d.GetValue(SourceProperty);
        }
    }
}