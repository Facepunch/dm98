using System.Threading.Tasks;

partial class Tripmine : ModelEntity
{
	Particles LaserParticle;

	public Tripmine()
	{
		Model = Model.Load( "models/dm_tripmine.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	public async Task Arm( float seconds )
	{
		// arming noise

		await GameTask.DelaySeconds( seconds );

		LaserParticle = Particles.Create( "particles/tripmine_laser.vpcf", this, "laser", true );
		LaserParticle.SetPosition( 0, Position );

		var tr = Trace.Ray( Position, Position + Rotation.Forward * 4000.0f )
			.Ignore( this )
			.Run();

		LaserParticle.SetPosition( 1, tr.EndPosition );

		// armed chirp

		if ( tr.Entity != null && tr.Entity is not WorldEntity )
		{
			await Explode( 0.5f );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		_ = Explode( 0.4f );
	}

	async Task Explode( float delay )
	{
		await Task.DelaySeconds( delay );

		if ( !IsValid ) return;

		Sound.FromWorld( "rust_pumpshotgun.shootdouble", Position );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", Position );

		LaserParticle?.Destroy( true );
		LaserParticle = null;

		Delete();
	}
}
