﻿using Gk_01.DI;
using Gk_01.Services.Interfaces;
using Gk_01.Services.Services;
using System.Runtime.InteropServices;
using System.Windows;
using Unity;
using Unity.Lifetime;

namespace Gk_01
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer _container = DIContainer.GetContainer();
            _container.RegisterType<IFileService, FileService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IBezierCurveCalculatorService, BezierCurveCalculatorService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDrawingService, DrawingService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ITransformations2DService, Transformations2DService>(new ContainerControlledLifetimeManager());
            AllocConsole();
        }
    }

}
