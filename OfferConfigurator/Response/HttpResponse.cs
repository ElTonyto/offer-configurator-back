using System;
namespace OfferConfigurator.Response
{
    public class HttpResponse
    {
        public int Status { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
