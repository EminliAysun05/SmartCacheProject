using CacheSmartProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSmartProject.Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Preferences { get; set; } = "{}";
        public DateTime LastModified { get; set; }
    }
}
