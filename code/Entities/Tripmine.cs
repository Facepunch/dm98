using System.Threading.Tasks;

partial class Tripmine : ModelEntity
{
	Particles LaserParticle;
	LaserTrigger LaserTrigger;

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

		LaserTrigger = new LaserTrigger();
		LaserTrigger.SetParent( this, "laser", Transform.Zero );
		LaserTrigger.CreateTrigger( tr.Distance );
		LaserTrigger.OnTriggered = ( e ) => _ = Explode( 0.2f );

		// armed chirp

		if ( tr.Entity != null && tr.Entity is not WorldEntity )
		{
			await Explode( 0.5f );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		_ = Explode( 0.3f );
	}

	bool exploding = false;

	async Task Explode( float delay )
	{
		if ( exploding ) return;

		exploding = true;
		await Task.DelaySeconds( delay );

		if ( !IsValid ) return;

		Sound.FromWorld( "rust_pumpshotgun.shootdouble", Position );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", Position );

		LaserParticle?.Destroy( true );
		LaserParticle = null;

		Delete();
	}
}

public class LaserTrigger : ModelEntity
{
	public Action<Entity> OnTriggered { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		// Client doesn't need to know about this ;)
		Transmit = TransmitType.Never;
	}

	public void CreateTrigger( float length )
	{
		SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, new Capsule( Vector3.Zero, Rotation.Forward * length, 0.2f ) );
		CollisionGroup = CollisionGroup.Trigger;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is WorldEntity ) return;

		OnTriggered?.Invoke( other );
	}
}
