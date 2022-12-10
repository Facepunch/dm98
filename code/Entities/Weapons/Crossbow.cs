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
			var aim = Owner.AimRay;
			var bolt = new CrossbowBolt();
			bolt.Position = aim.Position;
			bolt.Rotation = Rotation.LookAt( aim.Forward );
			bolt.Owner = Owner;
			bolt.Velocity = aim.Forward * 100;
		}
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Zoomed = Input.Down( InputButton.SecondaryAttack );
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Game.AssertClient();

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairLastShoot = 0;
	}

	public override void UpdateCamera()
	{
		if ( Zoomed )
		{
			Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 30 );
		}
	}

}
