using System.Net;
using System.Net.Http;

namespace PCITC.MES.MM.Tapper.Svc.Controllers
{
    public static class ForbiddenActions
    {
        public static HttpResponseMessage Get
        {
            get {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Not allowed to GET resource"),
                    ReasonPhrase = "forbidden"
                };
                return resp;
            }
        }

        public static HttpResponseMessage Post
        {
            get
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Not allowed to POST resource"),
                    ReasonPhrase = "forbidden"
                };
                return resp;
            }
        }

        public static HttpResponseMessage Put
        {
            get
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Not allowed to PUT resource"),
                    ReasonPhrase = "forbidden"
                };
                return resp;
            }
        }

        public static HttpResponseMessage Delete
        {
            get
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Not allowed to DELETE resource"),
                    ReasonPhrase = "forbidden"
                };
                return resp;
            }
        }
    }
}
