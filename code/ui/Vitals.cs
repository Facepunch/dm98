using Sandbox.UI;
using Sandbox.UI.Construct;

public class Vitals : Panel
{
	public Label Health;
	public Label Armour;

	public Vitals()
	{
		Health = Add.Label( "100", "health" );
		Armour = Add.Label( "100", "armour" );
	}

	public override void Tick()
	{
		var player = Local.Pawn as DeathmatchPlayer;
		if ( player == null ) return;

		Health.Text = $"{player.Health.CeilToInt()}";
		Health.SetClass( "danger", player.Health < 40.0f );

		Armour.Text = $"{player.Armour.CeilToInt()}";
		Armour.SetClass( "danger", player.Health < 40.0f );
	}
}
