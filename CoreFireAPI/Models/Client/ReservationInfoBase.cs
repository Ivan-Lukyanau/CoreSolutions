using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace CoreFireAPI.Models.Client
{
    public class ReservationInfoBase: ClientInfoBase
    {
        public string Time { get; set; }
    }
}
