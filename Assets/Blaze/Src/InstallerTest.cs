

using Blaze.Runtime;
using Blaze.Runtime.DependencyInjection;

namespace Blaze.Test
{
    public class InstallerTest : QMonoInstaller
    {
        public override void InstallBindings()
        {
            QDebugBase<InternalLogChannel>.VerboseLog(InternalLogChannel.System, "Installing bindings", gameObject);

            Container.Bind<ImpsTest>().FromInstance(Singleton<ImpsTest>.Instance);
            Container.Bind<MonoObject2>().FromInstance(Singleton<MonoObject2>.Instance);
            Container.Bind<Object2>().AsSingle();
        }
    }
}