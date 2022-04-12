using System.ComponentModel.DataAnnotations;

[Library( "dm_pistol", Title = "Pistol" )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
[Display( Name = "Pistol" )]
partial class Pistol : DeathmatchWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/rust_pistol/rust_pistol.vmdl" );
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 12.0f;
	public override float SecondaryRate => 4.5f;
	public override float ReloadTime => 3.0f;

	public override int Bucket => 1;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 12;
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
		ShootBullet( 0.05f, 1, 12.0f, 2.0f );

	}

	public override void AttackSecondary()
	{
		base.AttackSecondary();

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
		ShootBullet( 0.4f, 1.5f, 8.0f, 3.0f );
	}
}
