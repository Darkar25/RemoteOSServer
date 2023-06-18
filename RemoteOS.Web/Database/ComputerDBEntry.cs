using Microsoft.EntityFrameworkCore;
using RemoteOS.OpenComputers.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace RemoteOS.Web.Database
{
    public class ComputerDBEntry
    {
        [Key]
        public Guid Address { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Z { get; set; }
        public Sides? Facing { get; set; }
        [NotMapped]
        public Vector3? WorldPosition
        {
            get => X.HasValue && Y.HasValue && Z.HasValue ? new(X.Value, Y.Value, Z.Value) : null;
            set
            {
                X = value?.X;
                Y = value?.Y;
                Z = value?.Z;
            }
        }
    }
}
