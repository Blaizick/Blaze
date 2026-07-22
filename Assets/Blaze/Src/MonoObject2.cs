using System;
using Blaze.Runtime;
using Blaze.Runtime.DependencyInjection;

namespace Blaze.Test
{
    public interface IMonoObject
    {
        
    }

    public class MonoObject2 : ManagedBehaviour, IMonoObject
    {
        [QInject, NonSerialized]
        public Object2 object2;
    }

    public class Object2
    {
        public int a = 123;
    }
}