using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.Domain.Enum
{
    public enum ErrorCodes
    {
        // 0 - 10 Reports
        // 11 - 20 User
        ReportsNotFound = 0,
        ReportNotFound = 1,
        ReportAlreadyExist = 2,
        

        UserNotFound = 11,

        InternalServerError = 10,
    }
}
