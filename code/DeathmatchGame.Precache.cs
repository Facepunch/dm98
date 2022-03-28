partial class DeathmatchGame : Game
{
	/// <summary>
	/// Precache particles since there's no way to lazy precache them like Model or SoundEvent yet.
	/// </summary>
	public static void PrecacheParticles()
	{
		Precache.Add( "particles/explosion/barrel_explosion/explosion_barrel.vpcf" );
		Precache.Add( "particles/tripmine_laser.vpcf" );
		Precache.Add( "particles/pistol_muzzleflash.vpcf" );
		Precache.Add( "particles/pistol_ejectbrass.vpcf" );
	}
}
