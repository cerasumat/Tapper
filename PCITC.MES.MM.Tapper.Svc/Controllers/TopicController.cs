using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Oracle.ManagedDataAccess.Client;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Dapper;

namespace PCITC.MES.MM.Tapper.Svc.Controllers
{
    public class TopicController : ApiController
    {
        private ApiSetting Setting { get; }

        public TopicController()
        {
            Setting = new ApiSetting();
        }

        [HttpGet]
        public IEnumerable<TopicModel> GetAllTopics()
        {
            IEnumerable<TopicModel> topics;
            using (var connection = new OracleConnection(Setting.ConnectionStr))
            {
                connection.Open();
                topics = connection.QueryList<TopicModel>(null, Setting.TopicModelTable, "*");
                if (topics == null)
                {
                    return null;
                }
                connection.Close();
            }
            return topics;
        }

        [HttpGet]
        public HttpResponseMessage GetTopic(int id)
        {
            TopicModel topic;
            using (var connection = new OracleConnection(Setting.ConnectionStr))
            {
                var sql = string.Format("select * from {0} where topicid=:TopicId", Setting.TopicModelTable);
                var condition = new
                {
                    TopicId = id
                };
                connection.Open();
                topic = connection.QueryList<TopicModel>(sql, condition, null, null).FirstOrDefault();
                connection.Close();
            }
            HttpResponseMessage resp;
            if (topic == null)
            {
                resp = new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    ReasonPhrase = "no resource found"
                };
            }
            else
            {
                resp = Request.CreateResponse(HttpStatusCode.OK, topic);
            }
            return resp;
        }

        [HttpPost]
        public void PostTopic([FromBody]TopicModel value)
        {
            throw new HttpResponseException(ForbiddenActions.Post);
        }

        [HttpPut]
        public void PutTopic([FromBody] TopicModel value)
        {
            throw new HttpResponseException(ForbiddenActions.Put);
        }

        [HttpDelete]
        public void DeleteTopic(int id)
        {
            throw new HttpResponseException(ForbiddenActions.Delete);
        }
    }
}
