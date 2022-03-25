using Sandbox.UI;
using Sandbox.UI.Construct;

public class HealthHud : Panel
{
	public IconPanel Icon;
	public Label Value;

	public HealthHud()
	{
		Icon = Add.Icon( "add_box", "icon" );
		Value = Add.Label( "0", "label" );
	}

	public override void Tick()
	{
		var player = Local.Pawn as DeathmatchPlayer;
		if ( player == null ) return;

		Value.Text = $"{player.Health.CeilToInt()}";

		SetClass( "low", player.Health < 40.0f );
		SetClass( "empty", player.Health <= 0.0f );
	}
}

public class ArmourHud : Panel
{
	public IconPanel Icon;
	public Label Value;

	public ArmourHud()
	{
		Icon = Add.Icon( "shield", "icon" );
		Value = Add.Label( "0", "label" );

	}

	public override void Tick()
	{
		var player = Local.Pawn as DeathmatchPlayer;
		if ( player == null ) return;

		Value.Text = $"{player.Armour.CeilToInt()}";

		SetClass( "low", player.Armour < 40.0f );
		SetClass( "empty", player.Armour <= 0.0f );
	}
}
