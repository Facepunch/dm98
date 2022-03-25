using Sandbox.UI;
using Sandbox.UI.Construct;

public class Ammo : Panel
{
	public Label Inventory;
	public Panel AmmoBar;

	List<Panel> BulletPanels = new List<Panel>();

	public Ammo()
	{
		AmmoBar = Add.Panel( "ammobar" );
		Inventory = Add.Label( "100", "inventory" );
	}

	int weaponHash;

	public override void Tick()
	{
		var player = Local.Pawn as Player;
		if ( player == null ) return;

		var weapon = player.ActiveChild as DeathmatchWeapon;
		SetClass( "active", weapon != null );

		if ( weapon == null ) return;

		var inv = weapon.AvailableAmmo();
		Inventory.Text = $"{inv}";
		Inventory.SetClass( "active", inv >= 0 );

		var hash = HashCode.Combine( player, weapon );
		if ( weaponHash != hash )
		{
			weaponHash = hash;
			RebuildAmmoBar( weapon );
		}

		UpdateAmmoBar( weapon );
	}

	void RebuildAmmoBar( DeathmatchWeapon weapon )
	{
		AmmoBar.DeleteChildren();
		BulletPanels.Clear();

		AmmoBar.SetClass( "is-crossbow", weapon is Crossbow );
		AmmoBar.SetClass( "is-shotgun", weapon is Shotgun );

		for ( int i = 0; i < weapon.ClipSize; i++ )
		{
			var bullet = AmmoBar.Add.Panel( "bullet" );
			BulletPanels.Add( bullet );
		}
	}

	void UpdateAmmoBar( DeathmatchWeapon weapon )
	{
		for ( int i = 0; i < BulletPanels.Count; i++ )
		{
			BulletPanels[i].SetClass( "empty", i >= weapon.AmmoClip );
		}
	}

	public override void DrawContent( ref RenderState state )
	{
		base.DrawContent( ref state );


	}

	public override void DrawBackground( ref RenderState state )
	{
		base.DrawBackground( ref state );

		return;

		var player = Local.Pawn as Player;
		if ( player == null ) return;

		var weapon = player.ActiveChild as DeathmatchWeapon;
		if ( weapon == null ) return;

		var w = 10 * ScaleToScreen;
		var h = 40 * ScaleToScreen;
		var gap = 2 * ScaleToScreen;
		var x = Box.Rect.right - w;
		var y = Box.Rect.top;

		Render.Draw2D.BlendMode = BlendMode.Lighten;

		for ( int i = 0; i < weapon.ClipSize; i++ )
		{
			var rect = new Rect( x, y, w, h );

			bool used = i < weapon.AmmoClip;
			var color = Color.Yellow.WithAlpha( used ? 1.0f : 0.1f );
			if ( weapon.AmmoClip == 0 ) color = Color.Red.WithAlpha( 0.2f );

			Render.Draw2D.Box( rect, color, new( 0, 8, 0, 8 ) );

			x -= w + gap;
		}
	}
}
