using System.Collections.Generic;
using UnityEngine;

namespace Blaze.Runtime.DependencyInjection
{
    [DefaultExecutionOrder(int.MinValue)]
    public class QSceneContext : ManagedBehaviour
    {
        public List<QMonoInstaller> monoInstallers = new();
        
        public void Awake()
        {
            var sceneContext = FindAnyObjectByType<QSceneContext>();
            if (sceneContext)
            {
                foreach (var monoInstaller in sceneContext.monoInstallers)
                {
                    monoInstaller.Container.installedBindings = false;
                    monoInstaller.InstallBindings();
                    monoInstaller.Container.installedBindings = true;
                }
                foreach (var monoBehaviour in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    foreach (var monoInstaller in sceneContext.monoInstallers)
                    {
                        monoInstaller.Container.Inject(monoBehaviour, monoBehaviour.GetType());
                    }                    
                }
            }
        }
    }
}