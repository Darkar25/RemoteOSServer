using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace RemoteOS.Web.Database
{
    [PrimaryKey(nameof(X), nameof(Y), nameof(Z))]
    [JsonDerivedType(typeof(AnalyzedBlock))]
    [JsonDerivedType(typeof(ScannedBlock))]
    public abstract class IBlock
    {
        [Column(Order = 0)]
        public int X { get; set; }
        [Column(Order = 1)]
        public int Y { get; set; }
        [Column(Order = 2)]
        public int Z { get; set; }
        [NotMapped]
        public Vector3 WorldPosition
        {
            get => new(X, Y, Z);
            set
            {
                X = (int)value.X;
                Y = (int)value.Y;
                Z = (int)value.Z;
            }
        }
    }
}
