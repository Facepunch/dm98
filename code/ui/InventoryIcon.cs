
using Sandbox;
using Sandbox.UI;

class InventoryIcon : Panel
{
	public DeathmatchWeapon Weapon;
	public Panel Icon;

	public InventoryIcon( DeathmatchWeapon weapon )
	{
		Weapon = weapon;
		Icon = Add.Panel( "icon" );
		AddClass( weapon.ClassInfo.Name );
	}

	internal void TickSelection( DeathmatchWeapon selectedWeapon )
	{
		SetClass( "active", selectedWeapon == Weapon );
		SetClass( "empty", !Weapon?.IsUsable() ?? true );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !Weapon.IsValid() || Weapon.Owner != Local.Pawn )
			Delete();
	}
}
