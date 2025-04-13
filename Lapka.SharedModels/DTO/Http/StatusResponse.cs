using System.Net;

namespace Lapka.SharedModels.DTO.Http
{
    public class StatusResponse<T>
        where T : class
    {
        public T Response { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}

