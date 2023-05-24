﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class UserRole
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UserRolePermission> Permissions { get; set; }
    }
}