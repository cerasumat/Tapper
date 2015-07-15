using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Console
{
    public class WebApiDataAccess
    {
        private readonly Uri _svcUri;

        public WebApiDataAccess()
        {
            var setting = new ConsoleSetting();
            _svcUri= new UriBuilder("HTTP", setting.ServiceIp, 7777).Uri;
        }

        public async Task<IEnumerable<TopicModel>> GetTopics()
        {
            using (var httpClient = new HttpClient())
            {
                var topicUri = string.Format("{0}api/{1}", _svcUri, "topic");
                var response = await httpClient.GetAsync(topicUri);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var topicsStr = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<TopicModel>>(topicsStr);
                }
                else
                    return null;
            }
        }

        public async Task<BrokerStatisticInfo> GetStatisticInfo()
        {
            using (var httpClient = new HttpClient())
            {
                var infoUri=string.Format("{0}api/{1}", _svcUri, "info");
                var response = await httpClient.GetAsync(infoUri);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var infoStr = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BrokerStatisticInfo>(infoStr);
                }
                else
                    return new BrokerStatisticInfo();
            }
        }

        public async Task<IEnumerable<NotifyEntity>> GetNotifies(DateTime begDate,DateTime endDate)
        {
            using (var httpClient = new HttpClient())
            {
                var paramStr = string.Format("beg={0}&end={1}", begDate.ToString("yyyy-MM-dd"),
                    endDate.ToString("yyyy-MM-dd"));
                var notifyUri = string.Format("{0}api/{1}/?{2}", _svcUri, "notify", paramStr);
                var response = await httpClient.GetAsync(notifyUri);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var notifiesStr = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<NotifyEntity>>(notifiesStr);
                }
                else
                    return new List<NotifyEntity>();
            }
        }

    }
}
