using Sandbox.UI;
using Sandbox.UI.Construct;

public class InventoryColumn : Panel
{
	public int Column;
	public bool IsSelected;
	public Label Header;
	public int SelectedIndex;

	internal List<InventoryIcon> Icons = new();

	public InventoryColumn( int i, Panel parent )
	{
		Parent = parent;
		Column = i;
		Header = Add.Label( $"{i + 1}", "slot-number" );
	}

	internal void UpdateWeapon( DeathmatchWeapon weapon )
	{
		var icon = ChildrenOfType<InventoryIcon>().FirstOrDefault( x => x.Weapon == weapon );
		if ( icon == null )
		{
			icon = new InventoryIcon( weapon );
			icon.Parent = this;
			Icons.Add( icon );
		}


	}

	internal void TickSelection( DeathmatchWeapon selectedWeapon )
	{
		SetClass( "active", selectedWeapon?.Bucket == Column );

		for ( int i = 0; i < Icons.Count; i++ )
		{
			Icons[i].TickSelection( selectedWeapon );
		}

		SortChildren( p =>
		{
			if ( p is InventoryIcon icon )
			{
				return icon.Weapon?.BucketWeight ?? 0;
			}

			return 0;
		} );

	}
}
