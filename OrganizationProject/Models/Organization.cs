using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrganizationProject.Controllers
{
    public class Organization
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int BlackListTotal { get; set; }
        public int TotalCount { get; set; }

        public List<User> Users { get; set; }
    }
}
