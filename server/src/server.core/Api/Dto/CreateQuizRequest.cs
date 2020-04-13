using System;
using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class CreateQuizRequest
    {
        public List<Guid> Tasks { get; set; }
    }
}
