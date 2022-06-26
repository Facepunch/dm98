[Library( "dm_crossbow" ), HammerEntity]
[EditorModel( "weapons/rust_crossbow/rust_crossbow.vmdl" )]
[Title( "Crossbow" ), Category( "Weapons" )]
partial class Crossbow : DeathmatchWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/rust_crossbow/rust_crossbow.vmdl" );
	public override string ViewModelPath => "weapons/rust_crossbow/v_rust_crossbow.vmdl";

	public override float PrimaryRate => 1;
	public override int Bucket => 3;
	public override AmmoType AmmoType => AmmoType.Crossbow;
	public override int ClipSize => 5;

	[Net, Predicted]
	public bool Zoomed { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		AmmoClip = 5;
		Model = WorldModel;
	}

	public override void AttackPrimary()
	{
		if ( !TakeAmmo( 1 ) )
		{
			DryFire();

			if ( AvailableAmmo() > 0 )
			{
				Reload();
			}
			return;
		}

		ShootEffects();
		PlaySound( "rust_crossbow.shoot" );

		// TODO - if zoomed in then instant hit, no travel, 120 damage


		if ( IsServer )
		{
			var bolt = new CrossbowBolt();
			bolt.Position = Owner.EyePosition;
			bolt.Rotation = Owner.EyeRotation;
			bolt.Owner = Owner;
			bolt.Velocity = Owner.EyeRotation.Forward * 100;
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Zoomed = Input.Down( InputButton.SecondaryAttack );
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( Zoomed )
		{
			camSetup.FieldOfView = 20;
			camSetup.ViewModel.FieldOfView = 40;
		}
	}

	public override void BuildInput( InputBuilder owner )
	{
		if ( Zoomed )
		{
			owner.ViewAngles = Angles.Lerp( owner.OriginalViewAngles, owner.ViewAngles, 0.2f );
		}
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairLastShoot = 0;
	}

	TimeSince timeSinceZoomed;

	public override void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var draw = Render.Draw2D;

		if ( Zoomed )
			timeSinceZoomed = 0;

		var zoomFactor = timeSinceZoomed.Relative.LerpInverse( 0.4f, 0 );

		var color = Color.Lerp( Color.Red, Color.Yellow, lastReload.LerpInverse( 0.0f, 0.4f ) );
		draw.BlendMode = BlendMode.Lighten;
		draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

		// outer lines
		{
			var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.4f, 0.0f ) );
			var length = 10.0f;
			var gap = 40.0f + shootEase * 50.0f;

			gap -= zoomFactor * 20.0f;


			draw.Line( 0, center + Vector2.Up * gap, length, center + Vector2.Up * (gap + length) );
			draw.Line( 0, center - Vector2.Up * gap, length, center - Vector2.Up * (gap + length) );

			draw.Color = draw.Color.WithAlpha( draw.Color.a * zoomFactor );

			for ( int i = 0; i < 4; i++ )
			{
				gap += 40.0f;

				draw.Line( 0, center - Vector2.Left * gap, length, center - Vector2.Left * (gap + length) );
				draw.Line( 0, center + Vector2.Left * gap, length, center + Vector2.Left * (gap + length) );

				draw.Color = draw.Color.WithAlpha( draw.Color.a * 0.5f );
			}
		}
	}
}
