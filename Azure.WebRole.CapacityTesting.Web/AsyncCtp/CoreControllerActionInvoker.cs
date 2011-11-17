using System.Web.Mvc;

namespace Azure.WebRole.CapacityTesting.AsyncCtp
{
    /// <summary>
    ///   <para>Core Controller Action Invoker</para>
    /// </summary>
    public class CoreControllerActionInvoker : ControllerActionInvoker
    {
        private readonly ControllerDescriptor _descriptor = new CoreControllerDescriptor();

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            return _descriptor;
        }
    }
}