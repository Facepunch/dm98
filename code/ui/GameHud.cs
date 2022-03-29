
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

		Timer.Text = TimeSpan.FromSeconds( game.StateTimer ).ToString( @"m\:ss" );
		State.Text = game.GameState.ToString();
	}

}

