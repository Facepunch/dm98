using System.Threading.Tasks;

partial class HandGrenade : BasePhysics
{
	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/dm_grenade.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public async Task BlowIn( float seconds )
	{
		await GameTask.DelaySeconds( seconds );

		Sound.FromWorld( "rust_pumpshotgun.shootdouble", Position );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", Position );

		var Radius = 500;
		var Damage = 100;

		var ents = Entity.FindInSphere( Position, Radius );
		foreach ( var ent in ents )
		{
			if ( ent is not ModelEntity modelEnt || !modelEnt.IsValid() )
				continue;

			if ( ent is WorldEntity ) continue;
			if ( ent == this ) continue;
			if ( ent.LifeState != LifeState.Alive ) continue;
			if ( !modelEnt.PhysicsBody.IsValid() ) continue;
			if ( ent.IsWorld ) continue;

			var targetPos = modelEnt.PhysicsBody.MassCenter;
			var dist = targetPos.Distance( Position );
			var distanceMul = 1.0f - Math.Clamp( dist / Radius, 0.0f, 1.0f );
			var dmg = Damage * distanceMul;
			var force = (1 * distanceMul) * modelEnt.PhysicsBody.Mass;
			var forceDir = (targetPos - Position).Normal;

			//
			// TODO check through wall
			// TODO lets make a utility class for this
			// because there's gonna be a shit ton of repeated code
			//

			var damage = DamageInfo.Explosion( Position, forceDir * force, dmg )
				.WithAttacker( Owner )
				.WithWeapon( this );

			ent.TakeDamage( damage );
		}

		Delete();
	}
}
