﻿using ForDD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.Domain.Entity
{
    public class User : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }

        public string Login {  get; set; }

        public string Password { get; set; }

        public List<Report> Reports { get; set; }

        public UserToken Token { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
