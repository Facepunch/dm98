
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

namespace Sandbox.UI
{
	public partial class KillFeed : Panel
	{
		public static KillFeed Current;

		public KillFeed()
		{
			Current = this;

			StyleSheet.Load( "/ui/killfeed/KillFeed.scss" );
		}

		public virtual Panel AddEntry( long lsteamid, string left, long rsteamid, string right, string method )
		{
			var e = Current.AddChild<KillFeedEntry>();

			e.Left.Text = left;
			e.Left.SetClass( "me", lsteamid == (Local.Client?.SteamId) );

			e.Method.Text = method;

			e.Right.Text = right;
			e.Right.SetClass( "me", rsteamid == (Local.Client?.SteamId) );

			return e;
		}
	}
}
