public partial class DeathmatchHud : HudEntity<HudRootPanel>
{
	[ClientRpc]
	public void OnPlayerDied( DeathmatchPlayer player )
	{
		Game.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Game.AssertClient();
	}
}
