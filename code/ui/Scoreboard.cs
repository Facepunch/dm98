
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{
	/// <summary>
	/// Does the scoreboard need to be resorted? e.g if a player has died.
	/// </summary>
	public bool SortingDirty { get; set; } = true;

	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "player", "name" );
		Header.Add.Label( "kills", "kills" );
		Header.Add.Label( "deaths", "deaths" );
		Header.Add.Label( "ping", "ping" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible ) return;

		// Only sort when dirty as this can get noticeably laggy called every frame.
		if ( SortingDirty ) Canvas.SortChildren<ScoreboardEntry>( ( x ) => -x.Client.GetInt( "kills" ) );
		SortingDirty = false;
	}
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{

}
