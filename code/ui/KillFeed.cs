using Sandbox.UI;

public partial class KillFeed : Sandbox.UI.KillFeed
{
	public override Panel AddEntry( long lsteamid, string left, long rsteamid, string right, string method )
	{
		Log.Info( $"{left} killed {right} using {method}" );

		var e = Current.AddChild<KillFeedEntry>();

		e.AddClass( method );

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == Local.SteamId );

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == Local.SteamId );

		return e;
	}
}
