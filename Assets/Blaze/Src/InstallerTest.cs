using Blaze.Runtime;
using Blaze.Runtime.DependencyInjection;

namespace Blaze.Test
{
    public class InstallerTest : QMonoInstaller
    {
        public override void InstallBindings()
        {
            QDebugBase<InternalLogChannel>.VerboseLog(InternalLogChannel.System, "Installing bindings");

            Container.Bind<ImpsTest>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IMonoObject>().To<MonoObject2>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Object2>().FromNew().AsSingle();
        }
    }
}