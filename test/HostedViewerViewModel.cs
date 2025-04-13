using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

#if AUTOCAD2015_TO_2024
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#elif GSTARCAD2024_TO_2025
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.GraphicsInterface;
using Application = Gssoft.Gscad.ApplicationServices.Core.Application;
#elif GSTARCAD2017_TO_2023
using GrxCAD.DatabaseServices;
using GrxCAD.Geometry;
using GrxCAD.GraphicsInterface;
using Application = GrxCAD.ApplicationServices.Application;
#endif


namespace DrawableViewer.Test
{
    public class HostedViewerViewModel : ViewModelBase, IDisposable
    {
        private readonly Circle _circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 100);

        private readonly BlockTableRecord _currentSpace;

        private object _source;

        public HostedViewerViewModel()
        {
            _currentSpace = GetCurrentSpace();
        }

        public object Source
        {
            get => _source;
            set => SetProperty(ref _source, value, nameof(Source));
        }


        private ICommand _drawCircleCommand;

        public ICommand DrawCircleCommand => _drawCircleCommand ?? (_drawCircleCommand = new DelegateCommand(() =>
        {
            try
            {
                Source = _circle;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }));

        private ICommand _drawCurrentSpaceCommand;

        public ICommand DrawCurrentSpaceCommand =>
            _drawCurrentSpaceCommand ?? (_drawCurrentSpaceCommand = new DelegateCommand(() =>
            {
                try
                {
                    Source = _currentSpace;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }));

        private ICommand _drawExternalDwgCommand;

        public ICommand DrawExternalDwgCommand =>
            _drawExternalDwgCommand ?? (_drawExternalDwgCommand = new DelegateCommand(() =>
            {
                try
                {
                    string path = PromptGetDwgPath();
                    if (string.IsNullOrEmpty(path))
                        return;
                    
                    Source = path;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }));

        private ICommand _clearCommand;

        public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new DelegateCommand(() =>
        {
            Source = null;
        }));

        public void Dispose()
        {
            _circle.Dispose();
        }

        private static BlockTableRecord GetCurrentSpace()
        {
            var database = Application.DocumentManager.MdiActiveDocument.Database;
            var currentSpace = (BlockTableRecord)database.CurrentSpaceId.GetObject(OpenMode.ForRead);

            return currentSpace;
        }

        private static string PromptGetDwgPath()
        {
            var dlg = new OpenFileDialog { Filter = "图纸文件(*.dwg)|*.dwg" };
            return dlg.ShowDialog() != true ? null : dlg.FileName;
        }
    }
}