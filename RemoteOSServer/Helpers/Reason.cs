using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RemoteOS.Helpers
{
	public readonly record struct Reason(string Message)
	{
		public static explicit operator string(Reason reason) => reason.Message;
		public override string ToString() => Message;
	}
}
