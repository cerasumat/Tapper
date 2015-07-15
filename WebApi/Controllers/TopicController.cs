using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PCITC.MES.MM.Tapper.Engine.Entities;
using Oracle.ManagedDataAccess.Client;
using PCITC.MES.MM.Tapper.Framework.Dapper;

namespace WebApi.Controllers
{
    public class TopicController :ApiController
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
                topic = connection.QueryList<TopicModel>(sql,condition, null,null).FirstOrDefault();
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
            var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Not allowed to post resource"),
                ReasonPhrase = "forbidden"
            };
            throw new HttpResponseException(resp);
        }

        [HttpPut]
        public void PutTopic([FromBody] TopicModel value)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Not allowed to put resource"),
                ReasonPhrase = "forbidden"
            };
            throw new HttpResponseException(resp);
        }

        [HttpDelete]
        public void DeleteTopic(int id)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Not allowed to delete this resource"),
                ReasonPhrase = "forbidden"
            };
            throw new HttpResponseException(resp);
        }
    }
}
