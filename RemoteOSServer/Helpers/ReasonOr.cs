using OneOf;
using OneOf.Types;

namespace RemoteOS.Helpers
{
	[GenerateOneOf]
	public partial class ReasonOr<T0> : OneOfBase<Reason, T0> {
		public static implicit operator ReasonOr<T0>(string _) => new(new Reason(_));
	}
}
