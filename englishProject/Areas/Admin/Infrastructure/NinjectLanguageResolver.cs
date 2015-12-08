using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace englishProject.Areas.Admin.Infrastructure
{
    public class NinjectLanguageResolver : IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectLanguageResolver(IKernel kernel)
        {
            _kernel = kernel;
            _kernel.Bind<ILevel>().To<LevelAction>().InRequestScope();
            _kernel.Bind<IWord>().To<WordAction>().InRequestScope();
        }

        public NinjectLanguageResolver()
            : this(new StandardKernel())
        {
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}