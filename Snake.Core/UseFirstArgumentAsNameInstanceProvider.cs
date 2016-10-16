using System.Linq;
using System.Reflection;

using Ninject.Parameters;
using Ninject.Extensions.Factory;

namespace Snake.Core
{
    public class UseFirstArgumentAsNameInstanceProvider : StandardInstanceProvider
    {
        protected override string GetName(MethodInfo methodInfo, object[] arguments)
        {
            return (string)arguments[0];
        }

        protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
        {
            return base.GetConstructorArguments(methodInfo, arguments).Skip(1).ToArray();
        }
    }
}
