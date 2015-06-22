namespace BetHelper.Web.Controllers
{
    using System.Web.Http;

    using BetHelper.Data;

    public class BaseApiController : ApiController
    {
        public BaseApiController(IBetHelperData data)
        {
            this.Data = data;
        }

        protected IBetHelperData Data { get; private set; }
    }
}