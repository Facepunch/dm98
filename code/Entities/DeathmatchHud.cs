public partial class DeathmatchHud : HudEntity<HudRootPanel>
{
	[ClientRpc]
	public void OnPlayerDied( DeathmatchPlayer player )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}
