using Hammer;

[Library( "dm_357", Title = ".357 Magnum Revolver" )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
[EntityTool( ".357 Magnum Revolver", "DM98", ".357 Magnum Revolver Weapon." )]
partial class Python : DeathmatchWeapon
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 2.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 7.0f;
	public override int ClipSize => 6;
	public override AmmoType AmmoType => AmmoType.Python;

	public override int Bucket => 1;

	[Net]
	public bool Zoomed { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		AmmoClip = 6;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "rust_pistol.shoot" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.2f, 1.5f, 40.0f, 3.0f );

		if ( IsClient && IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 2, 2, 3, 0 );
		}

	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Zoomed = Input.Down( InputButton.Attack2 );
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( Zoomed )
		{
			camSetup.FieldOfView = 40;
		}
	}

}
