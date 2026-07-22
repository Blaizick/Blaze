using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blaze.Runtime.DependencyInjection
{
    [DefaultExecutionOrder(int.MinValue)]
    public class QSceneContext : Singleton<QSceneContext>
    {
        private QDiContainer m_Container;
        [SerializeField]
        private List<QMonoInstaller> m_MonoInstallers = new();
        // private List<IQInstaller> m_CustomInstallers = new();
        private decimal m_InitializationTime;

        public QDiContainer Container
        {
            get
            {
                return m_Container;
            }
        }

        public List<QMonoInstaller> MonoInstallers
        {
            get
            {
                return m_MonoInstallers;
            }
        }

        public decimal InitializationTime
        {
            get
            {
                return m_InitializationTime;
            }
        }

        // public List<IQInstaller> CustomInstallers
        // {
        //     get
        //     {
        //         return m_CustomInstallers;
        //     }
        // }

        public void Awake()
        {
            long startTick = DateTime.Now.Ticks;

            m_Container = new();
            
            Container.InstalledBindings = false;
            
            foreach (var monoInstaller in m_MonoInstallers)
            {
                monoInstaller.Container = m_Container;
                monoInstaller.InstallBindings();
            }
            
            // Type iInstallerType = typeof(IQInstaller);
            // Type monoInstallerType = typeof(QMonoInstaller);
            // var types = AppDomain.
            //     CurrentDomain.
            //     GetAssemblies().
            //     SelectMany(assembly => assembly.GetTypes()).
            //     Where(type => iInstallerType.IsAssignableFrom(type) && 
            //         !type.IsAbstract && 
            //         !monoInstallerType.IsAssignableFrom(type));
            // foreach (var type in types)
            // {
            //     var installer = (IQInstaller)Activator.CreateInstance(type);
            //     m_CustomInstallers.Add(installer);
            //     installer.Container = m_Container;
            //     installer.InstallBindings();
            // }
            
            Container.Flush();
            Container.InstalledBindings = true;

            foreach (var monoBehaviour in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Container.Inject(monoBehaviour);
            }
 
            long endTick = DateTime.Now.Ticks;
            m_InitializationTime = (endTick - startTick) / (decimal)TimeSpan.TicksPerSecond;
        }
    }
}