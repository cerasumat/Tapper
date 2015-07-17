using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using PCITC.MES.MM.Tapper.Engine.Broker;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Svc.Controllers
{
    public class InfoController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetBrokerStatisticInfos()
        {
            var broker = Broker.Instance;
            HttpResponseMessage resp;
            if (broker == null)
            {
                resp = new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    ReasonPhrase = "broker not created"
                };
            }
            else
            {
                var contentStr = JsonConvert.SerializeObject(broker.GetBrokerStatisticInfo());
                resp =new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(contentStr)
                };
                //resp = Request.CreateResponse(HttpStatusCode.OK, broker.GetBrokerStatisticInfo());
            }
            return resp;
        }

        [HttpGet]
        public HttpResponseMessage GetBrokerStatisticInfo(int id)
        {
            return GetBrokerStatisticInfos();
        }

        [HttpPost]
        public void PostBrokerStatisticInfo([FromBody]BrokerStatisticInfo value)
        {
            throw new HttpResponseException(ForbiddenActions.Post);
        }

        [HttpPut]
        public void PutBrokerStatisticInfo([FromBody] BrokerStatisticInfo value)
        {
            throw new HttpResponseException(ForbiddenActions.Put);
        }

        [HttpDelete]
        public void DeleteBrokerStatisticInfo(int id)
        {
            throw new HttpResponseException(ForbiddenActions.Delete);
        }
    }
}
