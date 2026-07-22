using System;
using UnityEngine;

namespace Blaze.Runtime.DependencyInjection
{
    public interface IQInstaller
    {
        public QDiContainer Container { get; set; }

        public void InstallBindings();    
    }

    public abstract class QMonoInstaller : ManagedBehaviour, IQInstaller
    {
        public QDiContainer Container { get; set; }
        
        public abstract void InstallBindings();
    }
}