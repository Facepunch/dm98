
using Sandbox.UI;
using Sandbox.UI.Construct;

internal class GameHud : Panel
{
	public Label Timer;
	public Label State;

	public GameHud()
	{
		State = Add.Label( string.Empty, "game-state" );
		Timer = Add.Label( string.Empty, "game-timer" );
	}

	public override void Tick()
	{
		base.Tick();

		var game = Game.Current as DeathmatchGame;
		if ( !game.IsValid() ) return;

		var span = TimeSpan.FromSeconds( (game.StateTimer * 60).Clamp( 0, float.MaxValue ) );

		Timer.Text = span.ToString( @"hh\:mm\:ss" );
		State.Text = game.GameState.ToString();
	}

}

