using System;
using System.Windows;
using System.Windows.Forms.Integration;

#if AUTOCAD2015_TO_2024
using Autodesk.AutoCAD.GraphicsSystem;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
#elif GSTARCAD2024_TO_2025
using Gssoft.Gscad.GraphicsSystem;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using Exception = Gssoft.Gscad.Runtime.Exception;
#elif GSTARCAD2017_TO_2023
using GrxCAD.GraphicsSystem;
using GrxCAD.DatabaseServices;
using GrxCAD.Geometry;
using Exception = GrxCAD.Runtime.Exception;
#endif

#if AUTOCAD2015_TO_2024
namespace Sharper.AutoCAD.DrawableViewer
#else
namespace Sharper.GstarCAD.Extensions
#endif
{
    /// <summary>
    /// 基于 WindowsFormsHost 的CAD图形可视化控件对象
    /// </summary>
    public class DrawableViewerHost : WindowsFormsHost
    {
        /// <summary>
        /// Windows Forms 控件的可视化对象
        /// </summary>
        private readonly DrawableViewer _viewer = new DrawableViewer();

        /// <summary>
        /// 构造器
        /// </summary>
        public DrawableViewerHost()
        {
            Child = _viewer;
        }

        /// <summary>
        /// 可绑定的源属性
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object),
                typeof(DrawableViewerHost), new PropertyMetadata(OnSourceChanged));

        /// <summary>
        /// 响应外部源更改的事件处理器
        /// </summary>
        /// <param name="d">被绑定的对象</param>
        /// <param name="e">绑定事件参数</param>
        /// <exception cref="NotSupportedException"></exception>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawableViewerHost host) || e.Property != SourceProperty)
                return;

            var viewer = host._viewer;
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
                case BlockTableRecord record:
                    viewer.EraseAll();
                    viewer.Add(record);
                    break;
                case Entity entity:
                    viewer.EraseAll();
                    viewer.Add(entity);
                    break;
                default:
                    throw new NotSupportedException("无效的数据类型，只支持DWG文件路径、Database、BlockTableRecord或Entity对象");
            }

            host.SetValue(DatabasePropertyKey, viewer.Database);
            if (host.AutoZooming)
                viewer.ZoomExtents();

            viewer.Regenerate();
        }

        /// <summary>
        /// 绘图对象改变时是否进行视图自适应缩放的属性
        /// </summary>
        public static readonly DependencyProperty AutoZoomingProperty =
            DependencyProperty.Register(nameof(AutoZooming), typeof(bool),
                typeof(DrawableViewerHost), new PropertyMetadata(true));

        /// <summary>
        /// <see cref="DrawableViewer"/> 控件能否响应鼠标事件
        /// </summary>
        public static readonly DependencyProperty CanMouseOperationProperty =
            DependencyProperty.Register(nameof(CanMouseOperation), typeof(bool),
                typeof(DrawableViewerHost), new PropertyMetadata(true, OnCanMouseOperationChanged));

        /// <summary>
        /// 响应控件能否支持鼠标操作的事件处理器
        /// </summary>
        /// <param name="d">被绑定的对象</param>
        /// <param name="e">绑定事件参数</param>
        private static void OnCanMouseOperationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawableViewerHost host) || e.Property != CanMouseOperationProperty)
                return;

            host._viewer.CanMouseOperation = Equals(e.NewValue, true);
        }

        /// <summary>
        /// 外部数据库对象
        /// </summary>
        private static readonly DependencyPropertyKey DatabasePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Database), typeof(Database), typeof(DrawableViewerHost),
                new PropertyMetadata(default(Database)));

        /// <summary>
        /// 外部数据库对象
        /// </summary>
        public static readonly DependencyProperty DatabaseProperty = DatabasePropertyKey.DependencyProperty;

        /// <summary>
        /// 源改变时是否进行视图自适应缩放
        /// </summary>
        public bool AutoZooming
        {
            get => (bool)GetValue(AutoZoomingProperty);
            set => SetValue(AutoZoomingProperty, value);
        }

        /// <summary>
        /// 控件是否支持鼠标操作
        /// </summary>
        public bool CanMouseOperation
        {
            get => (bool)GetValue(CanMouseOperationProperty);
            set => SetValue(CanMouseOperationProperty, value);
        }

        /// <summary>
        /// 显示源
        /// </summary>
        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// 外部数据库对象
        /// </summary>
        public Database Database => (Database)GetValue(DatabaseProperty);

        /// <summary>
        /// 是否来源于外部DWG文件
        /// </summary>
        public bool IsFromSource => _viewer.IsFromSource;

        /// <summary>
        /// 外部DWG文件的路径
        /// </summary>
        public string SourceFilePath => _viewer.Source;

        /// <summary>
        /// 可视化图形的包围盒
        /// </summary>
        public Extents3d Extents => _viewer.Extents;

        /// <summary>
        /// 添加实体可视化对象
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <exception cref="ArgumentNullException">对象为空</exception>
        /// <exception cref="Exception">对象的包围盒无效</exception>
        public void Add(Entity entity) => _viewer.Add(entity);

        /// <summary>
        /// 添加块表记录可视化对象
        /// </summary>
        /// <param name="blockTableRecord">块表记录对象</param>
        /// <exception cref="ArgumentNullException">对象为空</exception>
        /// <exception cref="Exception">对象的包围盒无效</exception>
        public void Add(BlockTableRecord blockTableRecord) => _viewer.Add(blockTableRecord);

        /// <summary>
        /// 重新生成View
        /// </summary>
        public void Regenerate() => _viewer.Regenerate();

        /// <summary>
        /// 删除所有加入的可视化对象
        /// </summary>
        public void EraseAll() => _viewer.EraseAll();

        /// <summary>
        /// 缩放至俯视图下的最大范围
        /// </summary>
        public void ZoomExtents() => _viewer.ZoomExtents();

        /// <summary>
        /// 设置视图的相机参数
        /// </summary>
        /// <param name="position">相机位置</param>
        /// <param name="target">相机目标</param>
        /// <param name="upVector">相机的上向量</param>
        /// <param name="fieldWidth">投影平面的宽度</param>
        /// <param name="fieldHeight">投影平面的高度</param>
        /// <param name="projection">投影方式</param>
        public void SetView(Point3d position, Point3d target, Vector3d upVector,
            double fieldWidth, double fieldHeight, Projection projection = Projection.Parallel)
        {
            _viewer.SetView(position, target, upVector, fieldWidth, fieldHeight, projection);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _viewer.Dispose();
        }
    }
}