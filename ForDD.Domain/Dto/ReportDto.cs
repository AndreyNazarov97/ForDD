using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.Domain.Dto
{
    public record ReportDto(long Id, string Name, string Description, string DateCreated);
}
