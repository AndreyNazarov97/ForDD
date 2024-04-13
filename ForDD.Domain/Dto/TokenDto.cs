using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.Domain.Dto
{
    public record TokenDto
    {
        public string AccesToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
