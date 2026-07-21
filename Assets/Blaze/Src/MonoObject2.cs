using System;
using Blaze.Runtime;
using Blaze.Runtime.DependencyInjection;

namespace Blaze.Test
{
    public class MonoObject2 : ManagedBehaviour
    {
        [QInject, NonSerialized]
        public Object2 object2;
    }

    public class Object2
    {
        public int a = 123;
    }
}