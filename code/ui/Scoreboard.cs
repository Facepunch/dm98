using Sandbox.UI.Construct;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{
	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "player", "name" );
		Header.Add.Label( "kills", "kills" );
		Header.Add.Label( "deaths", "deaths" );
		Header.Add.Label( "ping", "ping" );
	}

	RealTimeSince timeSinceSorted;

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible ) return;

		if ( timeSinceSorted > 0.1f )
		{
			timeSinceSorted = 0;

			//
			// Sort by number of kills, then number of deaths
			//
			Canvas.SortChildren<ScoreboardEntry>( ( x ) => (-x.Client.GetInt( "kills" ) * 1000) + x.Client.GetInt( "deaths" ) );
		}
	}

	public override bool ShouldBeOpen()
	{
		if ( DeathmatchGame.CurrentState == DeathmatchGame.GameStates.GameEnd )
			return true;

		return base.ShouldBeOpen();
	}
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{

}
