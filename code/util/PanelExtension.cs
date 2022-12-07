namespace Sandbox.UI
{
	public static class PanelExtension
	{
		public static void PositionAtCrosshair( this Panel panel )
		{
			panel.PositionAtCrosshair( Local.Pawn );
		}

		public static void PositionAtCrosshair( this Panel panel, Entity player )
		{
			if ( !player.IsValid() ) return;

			var tr = Trace.Ray( player.AimRay, 2000.0f )
							.Size( 1.0f )
							.Ignore( player )
							.UseHitboxes()
							.Run();

			panel.PositionAtWorld( tr.EndPosition );

		}

		public static void PositionAtWorld( this Panel panel, Vector3 pos )
		{
			var screenpos = pos.ToScreen();

			if ( screenpos.z < 0 )
				return;

			panel.Style.Left = Length.Fraction( screenpos.x );
			panel.Style.Top = Length.Fraction( screenpos.y );
			panel.Style.Dirty();
		}
	}

}
