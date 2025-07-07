using CacheSmartProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSmartProject.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public int? ParentId { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsActive { get; set; }
    }
}
