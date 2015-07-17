using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Oracle.ManagedDataAccess.Client;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Dapper;

namespace PCITC.MES.MM.Tapper.Svc.Controllers
{
    public class NotifyController : ApiController
    {
        private ApiSetting Setting { get; }

        public NotifyController()
        {
            Setting = new ApiSetting();
        }

        [HttpGet]
        public HttpResponseMessage GetAllNotifyEntities([FromUri] string beg, [FromUri] string end)
        {
            DateTime begTime;
            DateTime endTime;
            HttpResponseMessage resp;
            if (!DateTime.TryParse(beg, out begTime) || !DateTime.TryParse(end, out endTime))
            {
                resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "beg or end param invalid"
                };
            }
            else
            {
                IEnumerable<NotifyEntity> notifies;
                using (var connection = new OracleConnection(Setting.ConnectionStr))
                {
                    var sql = string.Format(@"select * from {0} where notifytime>=:BegTime and notifyTime<:EndTime", Setting.LogEntityTable);
                    var condition = new
                    {
                        BegTime = begTime.Date,
                        EndTime = endTime.Date.AddDays(1)
                    };
                    connection.Open();
                    notifies = connection.QueryList<NotifyEntity>(sql, condition, null, null);
                    connection.Close();
                }
                if (notifies.Any())
                {
                    resp = Request.CreateResponse(HttpStatusCode.OK, notifies);
                    resp.Content.Headers.Add("Content-Encoding", "gzip,deflate");
                    //resp.Headers.Add("Transfer-Encoding", "chunked");
                }
                else
                    resp=new HttpResponseMessage(HttpStatusCode.NoContent)
                    {
                        ReasonPhrase = "no match notifies"
                    };
            }
            
            return resp;
        }

        [HttpGet]
        public HttpResponseMessage GetNotifyEntity(string id, [FromUri] string beg, [FromUri] string end)
        {
            IEnumerable<NotifyEntity> notifies;
            using (var connection = new OracleConnection(Setting.ConnectionStr))
            {
                var sql = string.Format("select * from {0} where topic=:Topic", Setting.LogEntityTable);
                var condition = new
                {
                    Topic = id.ToUpper()
                };
                connection.Open();
                notifies = connection.QueryList<NotifyEntity>(sql, condition, null, null);
                connection.Close();
            }
            HttpResponseMessage resp;
            if (notifies == null||!notifies.Any())
            {
                resp = new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    ReasonPhrase = "no resource found"
                };
            }
            else
            {
                resp = Request.CreateResponse(HttpStatusCode.OK, notifies);
            }
            return resp;
        }

        [HttpPost]
        public HttpResponseMessage PostNotifyEntity([FromBody]NotifyEntity value)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            resp.Content.Headers.Allow.Add("GET");
            return resp;
            //throw new HttpResponseException(ForbiddenActions.Post);
        }

        [HttpPut]
        public HttpResponseMessage PutNotifyEntity([FromBody] NotifyEntity value)
        {
            var resp=new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            resp.Content.Headers.Allow.Add("GET");
            return resp;
            //throw new HttpResponseException(ForbiddenActions.Put);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteNotifyEntity(string id)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            resp.Content.Headers.Allow.Add("GET");
            return resp;
            //throw new HttpResponseException(ForbiddenActions.Delete);
        }
    }
}
