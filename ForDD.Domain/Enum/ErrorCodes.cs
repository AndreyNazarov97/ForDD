﻿using System;
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

        InternalServerError = 10,

        UserNotFound = 11,
        UserUnauthorizedAccess = 12,

        PasswordsNotEqual = 21,
        UserAlreadyExist = 22,
        WrongPassword = 23,

        RoleAlreadyExist = 31,
        RoleNotExist = 32,
        RoleNotFound = 33,
    }
}
