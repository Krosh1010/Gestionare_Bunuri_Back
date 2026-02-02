using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DbTables;

namespace Domain.Space
{
    public class SpaceUpdateDto
    {
        public string? Name { get; set; }
        public SpaceType? Type { get; set; }
        public int? ParentSpaceId { get; set; }
        public bool ParentSpaceIdIsSet { get; set; }
    }
}
