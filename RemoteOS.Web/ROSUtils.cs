using EasyJSON;
using Microsoft.EntityFrameworkCore;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;
using RemoteOS.OpenComputers.Exceptions;
using RemoteOS.Helpers;
using RemoteOS.Web.Database;
using System.Drawing;
using System.Numerics;

namespace RemoteOS.Web
{
    public static class ROSUtils
    {
		public static Vector3 RelativeSidePosition(Sides agentFacing = Sides.Front, Sides targetFacing = Sides.Front)
		{
			targetFacing = targetFacing.UnAlias();
			if (targetFacing == Sides.Bottom) return new(0, -1, 0);
			if (targetFacing == Sides.Top) return new(0, 1, 0);
			agentFacing = agentFacing.UnAlias();
			var tmp = agentFacing switch
			{
				Sides.Back => 3,
				Sides.Right => 0,
				Sides.Front => 1,
				Sides.Left => 2,
				_ => throw new InvalidSideException("Agent cannot face downwards of upwards")
			};
			var tmp2 = targetFacing switch
			{
				Sides.Back => 3,
				Sides.Right => 0,
				Sides.Front => 1,
				Sides.Left => 2,
				_ => throw new Exception("This should never happen, bottom and top sides are already handled earlier")
			};
			return ((tmp + tmp2) % 4) switch
			{
				0 => new(0, 0, -1),
				1 => new(-1, 0, 0),
				2 => new(0, 0, 1),
				3 => new(1, 0, 0),
				_ => default,
			};
		}

		public static Vector3 SidePosition(Vector3 agentPos, Sides agentFacing = Sides.Front, Sides targetFacing = Sides.Front) => Vector3.Add(agentPos, RelativeSidePosition(agentFacing, targetFacing));

		/// <summary>
		/// Gets world position for the block on the side relative to agent facing
		/// </summary>
		/// <param name="comp">Agent</param>
		/// <param name="targetFacing">The face to ge tthe block for</param>
		public static Vector3 SidePosition(this ComputerDBEntry comp, Sides targetFacing = Sides.Front) => SidePosition(comp.WorldPosition!.Value, comp.Facing!.Value, targetFacing);

		/// <summary>
		/// Analyzes multiple sides in one request.
		/// </summary>
		/// <remarks>
		/// The order of the returned list matches the order of sides passed in arguments
		/// </remarks>
		/// <param name="geolyzer">Geolyzer that will perform the scanning</param>
		/// <param name="sides">Which sides to analyze</param>
		/// <returns>List of geolyzer results</returns>
		public static async Task<IEnumerable<GeolyzerResult>> BulkAnalyze(this GeolyzerComponent geolyzer, params Sides[] sides)
		{
			var handle = await geolyzer.GetHandle();
			var query = string.Join(",", sides.Select(x => $"{handle}.analyze({x.Luaify()})"));
			var exec = await geolyzer.Parent.Execute(query);
			return exec.Linq.Select(x => new GeolyzerResult()
			{
				Color = Color.FromArgb(x.Value["color"]),
				Hardness = x.Value["hardness"],
				HarvestLevel = x.Value["harvestLevel"],
				HarvestTool = x.Value["harvestTool"],
				Meta = x.Value["metadata"],
				Name = x.Value["name"],
				Properties = new(x.Value["properties"].Linq.ToDictionary(x => x.Key, x => x.Value))
			});
		}

		public static Sides UnAlias(this Sides side) => (Sides)(int)side;

		public static Vector3 scanOrigin = new(-2, -2, -2);
		public static Vector3 scanSize = new(4, 4, 4);
	}
}
