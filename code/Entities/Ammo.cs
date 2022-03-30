﻿using Hammer;

partial class BaseAmmo : ModelEntity, IRespawnableEntity
{
	public static readonly Model WorldModel = Model.Load( "models/dm_battery.vmdl" );

	public virtual AmmoType AmmoType => AmmoType.None;
	public virtual int AmmoAmount => 17;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );
	}
}


[Library( "dm_ammo9mmclip" )]
[EditorModel( "models/dm_ammo_9mmclip.vmdl" )]
[EntityTool( "Ammo - 9mm Clip", "DM98" )]
partial class Ammo9mmClip : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 17;
}

[Library( "dm_ammo9mmbox" )]
[EditorModel( "models/dm_ammo_9mmbox.vmdl" )]
[EntityTool( "Ammo - 9mm Box", "DM98" )]
partial class Ammo9mmBox : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 200;
}



[Library( "dm_ammobuckshot" )]
[EditorModel( "models/dm_ammo_buckshot.vmdl" )]
[EntityTool( "Ammo - Buckshot", "DM98" )]
partial class AmmoBuckshot : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Buckshot;
	public override int AmmoAmount => 12;
}

[Library( "dm_ammo357" )]
[EditorModel( "models/dm_ammo_357.vmdl" )]
[EntityTool( "Ammo - 357", "DM98" )]
partial class Ammo357 : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Python;
	public override int AmmoAmount => 6;
}

[Library( "dm_ammocrossbow" )]
[EditorModel( "models/dm_ammo_crossbow.vmdl" )]
[EntityTool( "Ammo - Crossbow", "DM98" )]
partial class AmmoCrossbow : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Crossbow;
	public override int AmmoAmount => 5;
}
