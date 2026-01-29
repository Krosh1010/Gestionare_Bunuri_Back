using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DbTables;

namespace Domain.Space
{
    public class SpaceCreateDto
    {
        public string Name { get; set; } = null!;
        public SpaceType Type { get; set; }
        public int? ParentSpaceId { get; set; }
    }
}
