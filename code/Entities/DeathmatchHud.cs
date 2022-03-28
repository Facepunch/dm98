public partial class DeathmatchHud : HudEntity<HudRootPanel>
{
	[ClientRpc]
	public void OnPlayerDied( DeathmatchPlayer player )
	{
		Host.AssertClient();

		RootPanel.Scoreboard.SortingDirty = true;
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}
