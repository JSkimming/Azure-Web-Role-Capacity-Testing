using System.Web.Mvc;
using Azure.WebRole.CapacityTesting.AsyncCtp;
using Azure.WebRole.CapacityTesting.Services;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Azure.WebRole.CapacityTesting.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Azure.WebRole.CapacityTesting.App_Start.NinjectMVC3), "Stop")]

namespace Azure.WebRole.CapacityTesting.App_Start
{
    using System.Reflection;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;

    public static class NinjectMVC3 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IControllerFactory>()
                .To<CoreControllerFactory>()
                .InSingletonScope();

            kernel.Bind<IActionInvoker>()
                .To<CoreControllerActionInvoker>()
                .InRequestScope();

            kernel.Bind<ILogger>()
                .To<ElmahLogger>()
                .InSingletonScope();

            kernel.Bind<ITableStorageWriterService>()
                .To<TableStorageWriterService>()
                .InSingletonScope();
        }        
    }
}
