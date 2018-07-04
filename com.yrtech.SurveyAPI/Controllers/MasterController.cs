using System.Web.Http;
using com.yrtech.SurveyAPI.Service;

namespace com.yrtech.SurveyAPI.Controllers
{
    [RoutePrefix("survey/api")]
    public class MasterController : ApiController
    {
        MasterService service = new MasterService();
        

    }
}
