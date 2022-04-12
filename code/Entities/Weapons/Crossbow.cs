using System.ComponentModel.DataAnnotations;

[Library( "dm_crossbow", Title = "Crossbow" )]
[Hammer.EditorModel( "weapons/rust_crossbow/rust_crossbow.vmdl" )]
[Display( Name = "Crossbow" )]
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

		Zoomed = Input.Down( InputButton.Attack2 );
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

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin( 0.5f, 4.0f, 1.0f, 0.5f );
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}
}
