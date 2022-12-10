[Library( "dm_tripmine" ), HammerEntity]
[EditorModel( "models/dm_tripmine.vmdl" )]
[Title(  "Tripmine" ), Category( "Weapons" )]
partial class TripmineWeapon : DeathmatchWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/dm_tripmine.vmdl" );
	public override string ViewModelPath => "models/v_dm_tripmine.vmdl";

	public override float PrimaryRate => 100.0f;
	public override float SecondaryRate => 100.0f;
	public override float ReloadTime => 0.1f;
	public override AmmoType AmmoType => AmmoType.Tripmine;
	public override int ClipSize => 1;
	public override int Bucket => 5;
	public override int BucketWeight => 200;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 1;
	}

	public override bool CanPrimaryAttack()
	{
		return Input.Released( InputButton.PrimaryAttack );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( Owner is not DeathmatchPlayer player ) return;

		// woosh sound
		// screen shake
		var aim = Owner.AimRay;

		Game.SetRandomSeed( Time.Tick );

		var tr = Trace.Ray( aim, 150 )
				.Ignore( Owner )
				.Run();

		if ( !tr.Hit )
			return;

		if ( !tr.Entity.IsWorld )
			return;

		if ( IsServer )
		{
			var grenade = new Tripmine
			{
				Position = tr.EndPosition,
				Rotation = Rotation.LookAt( tr.Normal, Vector3.Up ),
				Owner = Owner
			};

			_ = grenade.Arm( 1.0f );
		}

		TakeAmmo( 1 );
		Reload();

		if ( IsServer && AmmoClip == 0 && player.AmmoCount( AmmoType.Tripmine ) == 0 )
		{
			Delete();
			player.SwitchToBestWeapon();
		}
	}

	public override void SimulateAnimator( CitizenAnimationHelper anim )
	{
		anim.HoldType = CitizenAnimationHelper.HoldTypes.Shotgun;
		anim.AimBodyWeight = 1;
	}
}
