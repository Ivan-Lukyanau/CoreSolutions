using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace CoreFireAPI.Models
{
    public class NextMonthCreate
    {
        public byte NextMonth { get; set; }
        public byte FirstWorkingDay { get; set; }
    }
}
