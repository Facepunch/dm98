public partial class DeathmatchPlayer
{
	public ClothingContainer Clothing { get; protected set; }

	/// <summary>
	/// Set the clothes to whatever the player is wearing
	/// </summary>
	public void UpdateClothes( IClient cl )
	{
		Clothing ??= new();
		Clothing.LoadFromClient( cl );
	}
}
