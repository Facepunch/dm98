partial class SMGGrenade : BasePhysics
{
	public static readonly Model WorldModel = Model.Load( "models/dm_grenade.vmdl" );

	public SMGGrenade()
	{
		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Tags.Add( "grenade" );
	}

	[Event.Tick.Server]
	public void Simulate()
	{
		var trace = Trace.Ray( Position, Position )
			//.HitLayer( CollisionLayer.Water, true )
			.Size( 24 )
			.Ignore( this )
			.Ignore( Owner )
			.Run();

		Position = trace.EndPosition;

		if ( trace.Hit == true )
		{
			BlowUp();
		}
	}

	public void BlowUp()
	{
		DeathmatchGame.Explosion( this, Owner, Position, 250, 100, 1.0f );
		Delete();
	}
}
