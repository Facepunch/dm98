[Library( "dm_357" ), HammerEntity]
[EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
[Title( ".357 Magnum Revolver" ), Category( "Weapons" )]
partial class Python : DeathmatchWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/rust_pistol/rust_pistol.vmdl" );
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 2.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 7.0f;
	public override int ClipSize => 6;
	public override AmmoType AmmoType => AmmoType.Python;

	public override int Bucket => 1;
	public override int BucketWeight => 200;

	[Net, Predicted]
	public bool Zoomed { get; set; }

	private float? LastFov;
	private float? LastViewmodelFov;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 6;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.PrimaryAttack );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();

			if ( AvailableAmmo() > 0 )
			{
				Reload();
			}
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
		ShootBullet( 0.01f, 1.5f, 40.0f, 2.0f );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Zoomed = Input.Down( InputButton.SecondaryAttack );
	}

	public override void UpdateCamera()
	{
		base.UpdateCamera();

		if ( Zoomed )
		{
			Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 40 ); ;
		}
	}

	public override void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.3f, 0.0f ) );
		var color = Color.Lerp( Color.Red, Color.Yellow, lastReload.LerpInverse( 0.0f, 0.4f ) );

		//Graphics.BlendMode = BlendMode.Lighten;
		//Graphics.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

		var length = 3.0f + shootEase * 5.0f;

		//Graphics.Ring( center, length, length - 3.0f );
	}

}
